using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioActivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Activo> repo;
        private IRepositorioAsync<ElementoClasificacion> repoEC;
        private IRepositorioAsync<Archivo> repoA;
        
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        
        public ServicioActivo(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioActivo> Logger
           ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoEC = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
        }

        public async Task<bool> Existe(Expression<Func<Activo, bool>> predicado)
        {
            List<Activo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<bool> ExisteArchivo(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repoA.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<bool> ExisteElemento(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            List<ElementoClasificacion> l = await this.repoEC.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<Activo> CrearAsync(Activo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada!=true))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            if (!await ExisteElemento(x => x.Id.Equals(entity.ElementoClasificacionId.Trim(), StringComparison.InvariantCultureIgnoreCase)
           && x.Eliminada != true))
                throw new ExErrorRelacional(entity.ElementoClasificacionId);
            if (!await ExisteArchivo(x => x.Id.Equals(entity.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada != true))
                throw new ExErrorRelacional(entity.ArchivoId);
           
            
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(Activo entity)
        {
            Activo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            if (!await ExisteElemento(x => x.Id.Equals(entity.ElementoClasificacionId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ExErrorRelacional(entity.ElementoClasificacionId);
            if (!await ExisteArchivo(x => x.Id.Equals(entity.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ExErrorRelacional(entity.ArchivoId);
            
            if (await Existe(x =>
            x.Id != entity.Id & x.TipoOrigenId == entity.TipoOrigenId && x.OrigenId == entity.OrigenId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase) 
            && x.Eliminada != true))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Asunto = entity.Asunto;
            o.FechaApertura = entity.FechaApertura;
            o.FechaCierre = entity.FechaCierre;
            o.EsElectronio = entity.EsElectronio;
            o.CodigoOptico = entity.CodigoOptico;
            o.CodigoElectronico = entity.CodigoElectronico;
            o.EnPrestamo = entity.EnPrestamo;

            o.Reservado = entity.Reservado;
            o.Confidencial = entity.Confidencial;
            o.Ampliado = entity.Ampliado;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Activo c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    try
                    {
                        c.Eliminada = true;
                        UDT.Context.Entry(c).State = EntityState.Modified;
                        listaEliminados.Add(c.Id);
                    }
                    catch (Exception)
                    { }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        public async Task<IPaginado<Activo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<Activo> UnicoAsync(Expression<Func<Activo, bool>> predicado = null, Func<IQueryable<Activo>, IOrderedQueryable<Activo>> ordenarPor = null, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Activo a = await this.repo.UnicoAsync(predicado);
            return a.CopiaActivo();
        }
        public Task<List<Activo>> ObtenerAsync(Expression<Func<Activo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        private async Task<string> RestaurarNombre(string nombre, string ArchivoId, string id,string IdElemento)
        {

            if (await Existe(x =>
        x.Id != id && x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)
        && x.Eliminada == false
         && x.ArchivoId.Equals( ArchivoId,StringComparison.InvariantCultureIgnoreCase)
         && x.ElementoClasificacionId.Equals( IdElemento,StringComparison.InvariantCultureIgnoreCase)))
            {

                nombre = nombre + " restaurado " + DateTime.Now.Ticks;
            }
            else
            {
            }


            return nombre;

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Activo c;
            ICollection<string> listaEliminados = new HashSet<string>();

            foreach (var Id in ids)
            {

                c = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (c != null)
                {
                    c.Nombre = await RestaurarNombre(c.Nombre, c.ArchivoId, c.Id,c.ElementoClasificacionId);
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        #region Sin Implementar

        public Task<IEnumerable<Activo>> CrearAsync(params Activo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Activo>> CrearAsync(IEnumerable<Activo> entities, CancellationToken cancellationToken = default)
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

       

        public Task<List<Activo>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Activo>> ObtenerPaginadoAsync(Expression<Func<Activo, bool>> predicate = null, Func<IQueryable<Activo>, IOrderedQueryable<Activo>> orderBy = null, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


    

        #endregion


    }
}
