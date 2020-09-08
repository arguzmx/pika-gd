using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.Servicios.Reporte;
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
        private IRepositorioAsync<CuadroClasificacion> repoCC;
        private IRepositorioAsync<ElementoClasificacion> repoEL;
        private IRepositorioAsync<EntradaClasificacion> repoEC;
        private IRepositorioAsync<Archivo> repoA;
        private IoImportarActivos importador;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly ConfiguracionServidor configuracion;
        private ServicioEstadisticaClasificacionAcervo sevEstadisticas;
        public ServicioActivo(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioActivo> Logger, IOptions<ConfiguracionServidor> Config
           ) : base(proveedorOpciones, Logger)
        {
            this.configuracion = Config.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoEC = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoCC = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.repoEL = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
            this.importador = new IoImportarActivos(this, Logger, proveedorOpciones, Config);
            this.sevEstadisticas = new ServicioEstadisticaClasificacionAcervo(proveedorOpciones, Config, Logger);
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

        public async Task<bool> ExisteElemento(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            List<EntradaClasificacion> l = await this.repoEC.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<Activo> CrearAsync(Activo entity, CancellationToken cancellationToken = default)
        {

            entity = await ValidarActivos(entity, false);
            entity.Id = Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            await this.RegistrarEstadisticas(entity.ArchivoId, entity.EntradaClasificacionId);
            return entity.Copia();

        }

        public async Task ActualizarAsync(Activo entity)
        {
            entity = await ValidarActivos(entity, true);
            UDT.Context.Entry(entity).State = EntityState.Modified;
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
        private async Task<Activo> ValidarActivos(Activo a, bool actualizar)
        {

            EntradaClasificacion ec = await repoEC.UnicoAsync(x => x.Id == a.EntradaClasificacionId);

            if (ec == null || ec.Eliminada == true) throw new ExErrorRelacional(a.EntradaClasificacionId);

            if (!await ExisteArchivo(x => x.Id.Equals(a.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)
                 && x.Eliminada == false))
                throw new ExErrorRelacional(a.ArchivoId);

            if (actualizar)
            {
                if (await Existe(x => x.Id != a.Id && x.Eliminada == false &&
                    x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ExElementoExistente(a.IDunico);
                var tmp = await this.repo.UnicoAsync(x => x.Id == a.Id);
                if (tmp == null)
                {
                    throw new EXNoEncontrado(a.Id);
                }

                a.ArchivoOrigenId = tmp.ArchivoOrigenId;
            }
            else
            {

                if (await Existe(x => x.Eliminada == false &&
                    x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ExElementoExistente(a.IDunico);
                a.ArchivoOrigenId = a.ArchivoId;
            }

            if (a.FechaCierre.HasValue)
            {
                a.FechaRetencionAT = ((DateTime)a.FechaCierre).AddYears(ec.VigenciaTramite);
                a.FechaRetencionAC = ((DateTime)a.FechaCierre).AddYears(ec.VigenciaTramite + ec.VigenciaConcentracion);
            }
            else
            {
                a.FechaRetencionAT = null;
                a.FechaRetencionAC = null;
            }

            return a;
        }

        public async Task<Activo> ValidadorImportador(Activo a)
        {
            if (!await ExisteElemento(x => x.Id.Equals(a.EntradaClasificacionId.Trim(), StringComparison.InvariantCultureIgnoreCase)
       || x.Eliminada != true)
       || !await ExisteArchivo(x => x.Id.Equals(a.ArchivoOrigenId.Trim(), StringComparison.InvariantCultureIgnoreCase)
                  && x.Eliminada != true)
       || await Existe(x => x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
            {
                a = new Activo();
            }
            else
            {
                await CrearAsync(a);
            }

            return a;
        }
        public async Task<byte[]> ImportarActivos(byte[] file, string ArchivId, string TipoId, string OrigenId, string formatoFecha)
        {
            byte[] archivo = await importador.ImportandoDatos(file, ArchivId, TipoId, OrigenId, formatoFecha);
            return archivo;
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
                   
                        c.Eliminada = true;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();

            await this.ListaActivosEliminados(ids,true);

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
            return a.Copia();
        }
        public Task<List<Activo>> ObtenerAsync(Expression<Func<Activo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        private async Task<string> RestaurarNombre(string nombre, string ArchivoId, string id, string IdElemento)
        {

            if (await Existe(x =>
        x.Id != id && x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)
        && x.Eliminada == false
         && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
         && x.EntradaClasificacionId.Equals(IdElemento, StringComparison.InvariantCultureIgnoreCase)))
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
                    c.Nombre = await RestaurarNombre(c.Nombre, c.ArchivoId, c.Id, c.EntradaClasificacionId);
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                    
                }
            }
            UDT.SaveChanges();
            await this.ListaActivosEliminados(ids, false);

            return listaEliminados;
        }
        public Task<List<Activo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public async Task RegistrarEstadisticas(string ArchivoId, string EntradaClasificacionId)
        {
            EntradaClasificacion ec = await this.repoEC.UnicoAsync(x => x.Id.Equals(EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));

            ElementoClasificacion elemento = await this.repoEL.UnicoAsync(x => x.Id.Equals(ec.ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            CuadroClasificacion cc = await this.repoCC.UnicoAsync(x => x.Id.Equals(elemento.CuadroClasifiacionId, StringComparison.InvariantCultureIgnoreCase));
            List<Activo> lisActivo = await this.repo.ObtenerAsync(x => x.EntradaClasificacionId.Equals(EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase)
                                     && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                                     && x.Eliminada == false);
            await sevEstadisticas.RegistroAñadido(ArchivoId, cc.Id, EntradaClasificacionId, lisActivo.Count);

        }
        private async Task ListaActivosEliminados(string[] ids, bool eliminado) 
        {
            List<Activo> listaActivos = await this.repo.ObtenerAsync(x => x.Id!=null);
            List<Activo> ActivosAgrupados = new List<Activo>();
            foreach (Activo activos in listaActivos)
            {
                foreach (string id in ids)
                {
                    if (activos.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase)) 
                    {
                        ActivosAgrupados.Add(activos);
                    }
                }
            }

            foreach (var a in ActivosAgrupados.GroupBy
                (x=>new { x.ArchivoId,x.EntradaClasificacionId})
                .Select(y=>new {ARchivoID=y.Key.ArchivoId,EntradaID=y.Key.EntradaClasificacionId, TotalActivos=y.ToList().Count() }).ToList())
            {
                if (eliminado)
                    await EliminarEstadisticas(a.ARchivoID, a.EntradaID, Convert.ToInt32(a.TotalActivos));
                else
                    await RestaurarEstadisticas(a.ARchivoID, a.EntradaID, Convert.ToInt32(a.TotalActivos));
            }
        }

        public async Task EliminarEstadisticas(string ArchivoId, string EntradaClasificacionId,int cantidad)
        {
            EntradaClasificacion ec = await this.repoEC.UnicoAsync(x => x.Id.Equals(EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            ElementoClasificacion elemento = await this.repoEL.UnicoAsync(x => x.Id.Equals(ec.ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            CuadroClasificacion cc = await this.repoCC.UnicoAsync(x => x.Id.Equals(elemento.CuadroClasifiacionId, StringComparison.InvariantCultureIgnoreCase));
            
            await sevEstadisticas.RegistroEliminado(ArchivoId, cc.Id, EntradaClasificacionId, cantidad);
        }
        public async Task RestaurarEstadisticas(string ArchivoId, string EntradaClasificacionId,int cantidad)
        {
            EntradaClasificacion ec = await this.repoEC.UnicoAsync(x => x.Id.Equals(EntradaClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            ElementoClasificacion elemento = await this.repoEL.UnicoAsync(x => x.Id.Equals(ec.ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));
            CuadroClasificacion cc = await this.repoCC.UnicoAsync(x => x.Id.Equals(elemento.CuadroClasifiacionId, StringComparison.InvariantCultureIgnoreCase));
            
            await sevEstadisticas.RegistroRestaurado(ArchivoId, cc.Id, EntradaClasificacionId, cantidad);
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


        public Task<IPaginado<Activo>> ObtenerPaginadoAsync(Expression<Func<Activo, bool>> predicate = null, Func<IQueryable<Activo>, IOrderedQueryable<Activo>> orderBy = null, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }






        #endregion


    }
}
