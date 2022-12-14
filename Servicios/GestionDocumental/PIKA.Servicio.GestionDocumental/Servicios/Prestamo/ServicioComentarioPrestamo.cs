using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioComentarioPrestamo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioComentarioPrestamo
    {
        private const string DEFAULT_SORT_COL = "Fecha";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ComentarioPrestamo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioComentarioPrestamo(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,  IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger) : base( registroAuditoria, proveedorOpciones, Logger,
               cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ComentarioPrestamo>(new QueryComposer<ComentarioPrestamo>());
        }

        #region  AuditoriaSeguridad

        public enum EventosAuditables
        {
            Crear = 1, Actualiza = 2, Eliminar = 3, Obtener = 4
        }

        public static List<TipoEventoAuditoria> EventosAuditoria()
        {
            return new List<TipoEventoAuditoria>()
                     {
                         new TipoEventoAuditoria() {
                             TipoEvento = (int)EventosAuditables.Crear,
                             Desripción ="Notifica la creación de activos"
                         },
                         new TipoEventoAuditoria() {
                             TipoEvento = (int)EventosAuditables.Actualiza,
                             Desripción ="Notifica la actualización de activos"
                         },
                         new TipoEventoAuditoria() {
                             TipoEvento = (int)EventosAuditables.Eliminar,
                             Desripción ="Notifica la eliminación de activos"
                         },
                     };
        }

        List<string> archivosUsuario = new List<string>();


        public async Task<ComentarioPrestamo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion


        public async Task<bool> Existe(Expression<Func<ComentarioPrestamo, bool>> predicado)
        {
            List<ComentarioPrestamo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ComentarioPrestamo> CrearAsync(ComentarioPrestamo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.PrestamoId == entity.PrestamoId &&x.Comentario.Equals(entity.Comentario, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }

            entity.Id = Guid.NewGuid().ToString();
            entity.Fecha = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(ComentarioPrestamo entity)
        {

            ComentarioPrestamo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.Comentario == entity.Comentario))
            {
                throw new ExElementoExistente(entity.Id);
            }

            o.Comentario = entity.Comentario;

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
        public async Task<IPaginado<ComentarioPrestamo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ComentarioPrestamo>, IIncludableQueryable<ComentarioPrestamo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ComentarioPrestamo>> CrearAsync(params ComentarioPrestamo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ComentarioPrestamo>> CrearAsync(IEnumerable<ComentarioPrestamo> entities, CancellationToken cancellationToken = default)
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
            ComentarioPrestamo o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {

                            await this.repo.Eliminar(o);
                        }
                        this.UDT.SaveChanges();
                        listaEliminados.Add(o.Id);
                    }
                    catch (DbUpdateException)
                    {
                        throw new ExErrorRelacional(Id);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;
        }

        public Task<List<ComentarioPrestamo>> ObtenerAsync(Expression<Func<ComentarioPrestamo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ComentarioPrestamo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<ComentarioPrestamo>> ObtenerPaginadoAsync(Expression<Func<ComentarioPrestamo, bool>> predicate = null, Func<IQueryable<ComentarioPrestamo>, IOrderedQueryable<ComentarioPrestamo>> orderBy = null, Func<IQueryable<ComentarioPrestamo>, IIncludableQueryable<ComentarioPrestamo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ComentarioPrestamo> UnicoAsync(Expression<Func<ComentarioPrestamo, bool>> predicado = null, Func<IQueryable<ComentarioPrestamo>, IOrderedQueryable<ComentarioPrestamo>> ordenarPor = null, Func<IQueryable<ComentarioPrestamo>, IIncludableQueryable<ComentarioPrestamo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ComentarioPrestamo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }
    }
}
