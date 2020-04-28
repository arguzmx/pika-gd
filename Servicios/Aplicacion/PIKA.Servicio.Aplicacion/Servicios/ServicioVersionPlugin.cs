using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public class ServicioVersionPlugin : ContextoServicioAplicacion, IServicioInyectable, IServicioVersionPlugin
    {
        private const string DEFAULT_SORT_COL = "id";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<VersionPlugin> repo;
        private ICompositorConsulta<VersionPlugin> compositor;
        private UnidadDeTrabajo<DbContextAplicacionPlugin> UDT;


        public ServicioVersionPlugin(
             IProveedorOpcionesContexto<DbContextAplicacionPlugin> proveedorOpciones,
         ICompositorConsulta<VersionPlugin> compositorConsulta,
         ILogger<ServicioVersionPlugin> Logger,
         IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextAplicacionPlugin>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<VersionPlugin>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<VersionPlugin, bool>> predicado)
        {
            List<VersionPlugin> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<VersionPlugin> CrearAsync(VersionPlugin entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }
            
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
         
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(VersionPlugin entity)
        {

            VersionPlugin o = await this.repo.UnicoAsync(x => x.Id == entity.Id);
           
            
            if (o == null)
            {
                
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.WriteLine("fail");
                throw new ExElementoExistente(entity.PluginId);
            }

            o.PluginId = entity.PluginId;
            o.RequiereConfiguracion = entity.RequiereConfiguracion;
            o.URL = entity.URL;
            o.Version = entity.Version;
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
        public async Task<IPaginado<VersionPlugin>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<VersionPlugin>, IIncludableQueryable<VersionPlugin, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<VersionPlugin>> CrearAsync(params VersionPlugin[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VersionPlugin>> CrearAsync(IEnumerable<VersionPlugin> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            VersionPlugin m;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                m = await this.repo.UnicoAsync(x => x.Id == Id);
                if (m != null)
                {
                    //UDT.Context.Entry(m).State = EntityState.Deleted;
                    listaEliminados.Add(m.Id);
                }
            }
            //UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<VersionPlugin>> ObtenerAsync(Expression<Func<VersionPlugin, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VersionPlugin>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<VersionPlugin>> ObtenerPaginadoAsync(Expression<Func<VersionPlugin, bool>> predicate = null, Func<IQueryable<VersionPlugin>, IOrderedQueryable<VersionPlugin>> orderBy = null, Func<IQueryable<VersionPlugin>, IIncludableQueryable<VersionPlugin, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<VersionPlugin> UnicoAsync(Expression<Func<VersionPlugin, bool>> predicado = null, Func<IQueryable<VersionPlugin>, IOrderedQueryable<VersionPlugin>> ordenarPor = null, Func<IQueryable<VersionPlugin>, IIncludableQueryable<VersionPlugin, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            VersionPlugin d = await this.repo.UnicoAsync(predicado);
            return d.CopiaVersionPlugin();
        }


    }
}
