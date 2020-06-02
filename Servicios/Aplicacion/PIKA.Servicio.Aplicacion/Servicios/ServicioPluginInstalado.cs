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
       public class ServicioPluginInstalado : ContextoServicioAplicacion
      , IServicioInyectable, IServicioPluginInstalado

    {
        private const string DEFAULT_SORT_COL = "PLuginId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PluginInstalado> repo;
        private ICompositorConsulta<PluginInstalado> compositor;
        private UnidadDeTrabajo<DbContextAplicacionPlugin> UDT;

        public ServicioPluginInstalado(
         IProveedorOpcionesContexto<DbContextAplicacionPlugin> proveedorOpciones,
         ICompositorConsulta<PluginInstalado> compositorConsulta,
         ILogger<ServicioPluginInstalado> Logger,
         IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextAplicacionPlugin>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<PluginInstalado>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<PluginInstalado, bool>> predicado)
        {
            List<PluginInstalado> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<PluginInstalado> CrearAsync(PluginInstalado entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.PLuginId.Equals(entity.PLuginId, StringComparison.InvariantCultureIgnoreCase)
            && x.VersionPLuginId.Equals(entity.VersionPLuginId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.PLuginId);
            }
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
          

            return entity;
        }

        public async Task ActualizarAsync(PluginInstalado entity)
        {

            PluginInstalado o = await this.repo.UnicoAsync(x => x.VersionPLuginId == entity.VersionPLuginId);
         
            if (o == null)
            {
               
                throw new EXNoEncontrado(entity.VersionPLuginId);
            }

            if (await Existe(x =>
            x.VersionPLuginId != entity.VersionPLuginId
            && x.VersionPLuginId.Equals(entity.VersionPLuginId, StringComparison.InvariantCultureIgnoreCase))) 
            {
                throw new ExElementoExistente(entity.VersionPLuginId);
            }

            o.PLuginId = entity.PLuginId;
            o.VersionPLuginId = entity.VersionPLuginId;
            o.FechaInstalacion = entity.FechaInstalacion;
            o.Activo = entity.Activo;
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
        public async Task<IPaginado<PluginInstalado>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PluginInstalado>, IIncludableQueryable<PluginInstalado, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<PluginInstalado>> CrearAsync(params PluginInstalado[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PluginInstalado>> CrearAsync(IEnumerable<PluginInstalado> entities, CancellationToken cancellationToken = default)
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            PluginInstalado m;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                m = await this.repo.UnicoAsync(x => x.VersionPLuginId == Id) ;
                if (m != null)
                {
                    UDT.Context.Entry(m).State = EntityState.Deleted;
                    listaEliminados.Add(m.VersionPLuginId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<PluginInstalado>> ObtenerAsync(Expression<Func<PluginInstalado, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<PluginInstalado>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<PluginInstalado>> ObtenerPaginadoAsync(Expression<Func<PluginInstalado, bool>> predicate = null, Func<IQueryable<PluginInstalado>, IOrderedQueryable<PluginInstalado>> orderBy = null, Func<IQueryable<PluginInstalado>, IIncludableQueryable<PluginInstalado, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public  Task<IEnumerable<string>>  Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<PluginInstalado> UnicoAsync(Expression<Func<PluginInstalado, bool>> predicado = null, Func<IQueryable<PluginInstalado>, IOrderedQueryable<PluginInstalado>> ordenarPor = null, Func<IQueryable<PluginInstalado>, IIncludableQueryable<PluginInstalado, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            PluginInstalado d = await this.repo.UnicoAsync(predicado);

            return d.CopiaPluginInstalado();
        }


    }
}
