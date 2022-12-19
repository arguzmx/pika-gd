using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{

    public class ServicioEventoAuditoria : ContextoServicioSeguridad
        , IServicioInyectable, IServicioEventoAuditoria, IRegistroAuditoria

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EventoAuditoria> repo;
        public ServicioEventoAuditoria(
         IAppCache cache,
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioLog> Logger
         ) : base(null, proveedorOpciones, Logger,
                 cache, ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA)
        {
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<EventoAuditoria>());
        }

        public async Task<bool> Existe(Expression<Func<EventoAuditoria, bool>> predicado)
        {
            List<EventoAuditoria> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<EventoAuditoria> CrearAsync(EventoAuditoria entity, CancellationToken cancellationToken = default)
        {
            EventoAuditoria tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id.ToString());
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }


        public async Task ActualizarAsync(EventoAuditoria entity)
        {
            throw new NotImplementedException();
        }
        
        


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();

        }


        public Task<List<EventoAuditoria>> ObtenerAsync(Expression<Func<EventoAuditoria, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<EventoAuditoria> UnicoAsync(Expression<Func<EventoAuditoria, bool>> predicado = null, Func<IQueryable<EventoAuditoria>, IOrderedQueryable<EventoAuditoria>> ordenarPor = null, Func<IQueryable<EventoAuditoria>, IIncludableQueryable<EventoAuditoria, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EventoAuditoria d = await this.repo.UnicoAsync(predicado);
            return d;
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

        public async Task<IPaginado<EventoAuditoria>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EventoAuditoria>, IIncludableQueryable<EventoAuditoria, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }






        #region sin implementar

        public Task<IEnumerable<EventoAuditoria>> CrearAsync(params EventoAuditoria[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventoAuditoria>> CrearAsync(IEnumerable<EventoAuditoria> entities, CancellationToken cancellationToken = default)
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


        public Task<List<EventoAuditoria>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<EventoAuditoria>> ObtenerPaginadoAsync(Expression<Func<EventoAuditoria, bool>> predicate = null, Func<IQueryable<EventoAuditoria>, IOrderedQueryable<EventoAuditoria>> orderBy = null, Func<IQueryable<EventoAuditoria>, IIncludableQueryable<EventoAuditoria, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<EventoAuditoria> InsertaEvento(EventoAuditoria ev)
        {
            await Task.Delay(0);

            Console.WriteLine("A+");
            ev.Id =  DateTime.UtcNow.Ticks;
            return ev;
        }

        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> Eventos)
        {
            throw new NotImplementedException();
        }

        public Task<EventoAuditoria> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EventoAuditoriaActivo>> EventosAuditables(string DominioId, string OUId)
        {
            //var r = await this.UDT.Context.EventosActivosAuditoria.Where(e=>e.DominioId == DominioId && e.UAId == OUId && e.Auditable == true).ToListAsync();
            //return r;
            List<EventoAuditoriaActivo> l = new List<EventoAuditoriaActivo>();
            return l;

        }



        #endregion


    }
}
