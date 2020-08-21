using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Data.Exportar_Importar;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioCuadroClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioCuadroClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<CuadroClasificacion> repo;
        private IRepositorioAsync<EstadoCuadroClasificacion> repoec;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IOCuadroClasificacion ioCuadroClasificacion;
        public ServicioCuadroClasificacion(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioCuadroClasificacion> Logger,
           IOptions<ConfiguracionServidor> Config
           ) : base(proveedorOpciones, Logger)
        {
            this.ConfiguracionServidor = Config.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.repoec = UDT.ObtenerRepositoryAsync<EstadoCuadroClasificacion>(new QueryComposer<EstadoCuadroClasificacion>());
            this.ioCuadroClasificacion = new IOCuadroClasificacion(Logger, proveedorOpciones);
        }


        public async Task<bool> Existe(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            List<CuadroClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<CuadroClasificacion> CrearAsync(CuadroClasificacion entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)
            &&x.Eliminada!=true))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            
            entity.EstadoCuadroClasificacionId = EstadoCuadroClasificacion.ESTADO_ACTIVO;
            entity.Nombre=entity.Nombre.Trim();
            entity.OrigenId = entity.OrigenId.Trim();
            entity.TipoOrigenId = entity.TipoOrigenId.Trim();

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }

       
        public async Task ActualizarAsync(CuadroClasificacion entity)
        {
            if (!await ValidarRelacionEstado(entity.EstadoCuadroClasificacionId.Trim()))
                throw new ExDatosNoValidos(entity.EstadoCuadroClasificacionId.Trim());

            if (await Existe(x=> x.Id != entity.Id.Trim() && 
            string.Equals(x.Nombre, entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.OrigenId == entity.OrigenId && x.TipoOrigenId == entity.TipoOrigenId && x.Eliminada != true))
            {
                throw new ExElementoExistente(entity.Nombre.Trim());
            }
            
            CuadroClasificacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            o.Nombre = entity.Nombre.Trim();
            o.Eliminada = entity.Eliminada;
            if(!String.IsNullOrEmpty(entity.EstadoCuadroClasificacionId.Trim()))
                o.EstadoCuadroClasificacionId = entity.EstadoCuadroClasificacionId.Trim();

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(entity.Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
        }
        private Consulta GetDefaultQuery(Consulta query)
        {
            if (query != null)
            {
                query.indice = query.indice < 0 ? 0 : query.indice;
                query.tamano = query.tamano <= 0 ? 20 : query.tamano;
                query.ord_columna = string.IsNullOrEmpty(query.ord_columna) ? DEFAULT_SORT_COL : query.ord_columna;
                query.ord_direccion = string.IsNullOrEmpty(query.ord_direccion) ? DEFAULT_SORT_DIRECTION : query.ord_direccion;
            }
            else
            {
                query = new Consulta() { indice = 0, tamano = 20, ord_columna = DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<CuadroClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            CuadroClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (c != null)
                {
                    c.Eliminada = true;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                    await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(c.Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);

                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            CuadroClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    c.Nombre = await RestaurarNombre(c.Nombre); ;
                    c.Eliminada = false;
                    await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }

            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        private async Task<string> RestaurarNombre(string nombre)
        {
              CuadroClasificacion cc = await UnicoAsync(x => x.Nombre == nombre && x.Eliminada != false);
                if (cc != null)
                    nombre = cc.Nombre + " restaurado " + DateTime.Now.Ticks;
           

            return nombre;

        }

        private async Task<bool> ValidarRelacionEstado(string EstadoCuadroClasificacionId)

        {
            EstadoCuadroClasificacion ec = await this.repoec.UnicoAsync(x=>x.Id== EstadoCuadroClasificacionId);
            if (ec != null)
                return true;
            else
                return false;
        }
       
        public async Task<CuadroClasificacion> UnicoAsync(Expression<Func<CuadroClasificacion, bool>> predicado = null, Func<IQueryable<CuadroClasificacion>, IOrderedQueryable<CuadroClasificacion>> ordenarPor = null, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            CuadroClasificacion c = await this.repo.UnicoAsync(predicado);
            return c.Copia();
        }
        public async Task<Array> ExportarCuadroCalsificacionExcel(string id)
        {
            Array a = await ioCuadroClasificacion.ExportarCuadroCalsificacionExcel(id, ConfiguracionServidor.ruta_cache_fisico,ConfiguracionServidor.separador_ruta);
            return a;
        }

        public Task<List<CuadroClasificacion>> ObtenerAsync(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        public Task<List<CuadroClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        #region No implmentados

        public Task<IEnumerable<CuadroClasificacion>> CrearAsync(params CuadroClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CuadroClasificacion>> CrearAsync(IEnumerable<CuadroClasificacion> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<CuadroClasificacion>> ObtenerPaginadoAsync(Expression<Func<CuadroClasificacion, bool>> predicate = null, Func<IQueryable<CuadroClasificacion>, IOrderedQueryable<CuadroClasificacion>> orderBy = null, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
 
        #endregion
    }
}
