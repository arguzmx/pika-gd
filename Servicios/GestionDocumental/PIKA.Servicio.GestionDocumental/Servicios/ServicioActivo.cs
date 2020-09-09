using LazyCache;
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
        private IRepositorioAsync<EntradaClasificacion> repoEC;
        private IRepositorioAsync<Archivo> repoA;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly IAppCache cache;
        IOptions<ConfiguracionServidor> Config;
        public ServicioActivo(
            IAppCache cache,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioActivo> Logger, IOptions<ConfiguracionServidor> Config
           ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoEC = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.cache = cache;
            this.Config = Config;
            
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

            Archivo archivo = await this.repoA.UnicoAsync(x => x.Id.Equals(a.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (archivo==null || archivo.Eliminada ) throw new ExErrorRelacional(a.ArchivoId);

            if (actualizar)
            {
                if (!string.IsNullOrEmpty(a.IDunico) && await Existe(x => x.Id != a.Id && x.Eliminada == false &&
                    x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ExElementoExistente(a.IDunico);
                var tmp = await this.repo.UnicoAsync(x => x.Id == a.Id);
                if ( tmp == null)
                {
                    throw new EXNoEncontrado(a.Id);
                }
               
                a.ArchivoOrigenId = tmp.ArchivoOrigenId;
            }
            else {
           

                if (!string.IsNullOrEmpty(a.IDunico) && await Existe(x => x.Eliminada == false && 
                    x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ExElementoExistente(a.IDunico);
                a.ArchivoOrigenId = a.ArchivoId;
            }

            a.TipoArchivoId = archivo.TipoArchivoId;

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
       || await Existe(x=>x.IDunico.Equals(a.IDunico,StringComparison.InvariantCultureIgnoreCase)))
            {
                a = new Activo();
            }
            else 
            {
                await CrearAsync(a);
            }

            return a;
        }
        public async Task<byte[]> ImportarActivos(byte[] file, string ArchivId, string TipoId, string OrigenId,string formatoFecha)
        {
            IoImportarActivos importador = new IoImportarActivos(this, this.logger,
                proveedorOpciones, Config);
            byte[] archivo= await  importador.ImportandoDatos(file,ArchivId,TipoId,OrigenId, formatoFecha);
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

            return listaEliminados;

        }

        public async Task<IPaginado<Activo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
                 Query = GetDefaultQuery(Query);

            List<string> especiales = new List<string>() { "Vencidos" };
            List<FiltroConsulta> filtrosespeciales = new List<FiltroConsulta>();
            List<Expression<Func<Activo, bool>>> filtros = new List<Expression<Func<Activo, bool>>>();

            foreach(var f in Query.Filtros)
            {
                if (especiales.IndexOf(f.Propiedad) >= 0)
                {
                    filtrosespeciales.Add(f);
                }
            }
            
            foreach(var f in filtrosespeciales)
            {
                Query.Filtros.Remove(f);
                switch (f.Propiedad)
                {
                    case "Vencidos":
                        filtros.Add(ObtieneFiltroVencimientos(f));
                        break;
                }
            }

            
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null, filtros);
            await ObtieneVencimientos(respuesta);

            return respuesta;
        }


        private async Task ObtieneVencimientos(IPaginado<Activo> respuesta)
        {
            // Busca las entradas de cuaddro qu eno esten en caché para el calculo de vencimientos
            List<string> idsEntrada = new List<string>();
            respuesta.Elementos.GroupBy(x => x.EntradaClasificacionId)
                .Select(x => new { id = x.Key }).ToList().ForEach(e =>
                {
                    EntradaClasificacion ec = this.cache.Get<EntradaClasificacion>($"ec-{e.id}");
                    if (ec == null)
                    {
                        idsEntrada.Add(e.id);
                    }
                });


            if (idsEntrada.Count > 0)
            {
                List<EntradaClasificacion> l = await this.repoEC.ObtenerAsync(x => idsEntrada.Contains(x.Id.Trim()));
                foreach (var ec in l)
                {
                    cache.Add<EntradaClasificacion>($"ec-{ec.Id}", ec, TimeSpan.FromMinutes(5));
                }
            }

            for (int i = 0; i < respuesta.Elementos.Count; i++)
            {
                EntradaClasificacion ec = this.cache.Get<EntradaClasificacion>($"ec-{respuesta.Elementos[i].EntradaClasificacionId}");
                if (ec != null)
                {
                    if (respuesta.Elementos[i].FechaCierre.HasValue)
                    {
                        switch (respuesta.Elementos[i].TipoArchivoId)
                        {
                            case TipoArchivo.IDARCHIVO_CONSERVACION:
                                var lapsoc = ((DateTime)respuesta.Elementos[i].FechaRetencionAC) - DateTime.UtcNow;
                                if (lapsoc.TotalDays >= 0) respuesta.Elementos[i].Vencidos = (int)lapsoc.TotalDays;
                                break;

                            case TipoArchivo.IDARCHIVO_TRAMITE:
                                var lapsoa = ((DateTime)respuesta.Elementos[i].FechaRetencionAC) - DateTime.UtcNow;
                                if (lapsoa.TotalDays >= 0) respuesta.Elementos[i].Vencidos = (int)lapsoa.TotalDays;
                                break;
                        }
                    }

                }
            }
        }

        private Expression<Func<Activo, bool>> ObtieneFiltroVencimientos(FiltroConsulta f)
        {
            DateTime fechaLimite = DateTime.Now.AddDays(int.Parse(f.Valor));
            return x => (x.TipoArchivoId == TipoArchivo.IDARCHIVO_TRAMITE
                           && x.FechaRetencionAT < fechaLimite)
                           ||
                           (x.TipoArchivoId == TipoArchivo.IDARCHIVO_CONSERVACION
                           && x.FechaRetencionAC < fechaLimite
                           );
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

        private async Task<string> RestaurarNombre(string nombre, string ArchivoId, string id,string IdElemento)
        {

            if (await Existe(x =>
        x.Id != id && x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)
        && x.Eliminada == false
         && x.ArchivoId.Equals( ArchivoId,StringComparison.InvariantCultureIgnoreCase)
         && x.EntradaClasificacionId.Equals( IdElemento,StringComparison.InvariantCultureIgnoreCase)))
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
            return listaEliminados;
        }
        public Task<List<Activo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
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
