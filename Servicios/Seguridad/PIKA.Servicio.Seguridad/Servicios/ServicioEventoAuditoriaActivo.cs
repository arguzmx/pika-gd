using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{

    public class ServicioEventoAuditoriaActivo : ContextoServicioSeguridad
        , IServicioInyectable, IServicioEventoAuditoriaActivo

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EventoAuditoriaActivo> repo;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;

        public UsuarioAPI usuario { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ContextoRegistroActividad RegistroActividad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public PermisoAplicacion permisos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ServicioEventoAuditoriaActivo(
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioEventoAuditoriaActivo> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EventoAuditoriaActivo>(new QueryComposer<EventoAuditoriaActivo>());

        }

        public async Task<bool> Existe(Expression<Func<EventoAuditoriaActivo, bool>> predicado)
        {
            List<EventoAuditoriaActivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<EventoAuditoriaActivo> CrearAsync(EventoAuditoriaActivo entity, CancellationToken cancellationToken = default)
        {
            EventoAuditoriaActivo tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }


        public async Task ActualizarAsync(EventoAuditoriaActivo entity)
        {
            throw new NotImplementedException();
        }
        
        


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            EventoAuditoriaActivo o;
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


        public Task<List<EventoAuditoriaActivo>> ObtenerAsync(Expression<Func<EventoAuditoriaActivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<EventoAuditoriaActivo> UnicoAsync(Expression<Func<EventoAuditoriaActivo, bool>> predicado = null, Func<IQueryable<EventoAuditoriaActivo>, IOrderedQueryable<EventoAuditoriaActivo>> ordenarPor = null, Func<IQueryable<EventoAuditoriaActivo>, IIncludableQueryable<EventoAuditoriaActivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EventoAuditoriaActivo d = await this.repo.UnicoAsync(predicado);
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

        public async Task<IPaginado<EventoAuditoriaActivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EventoAuditoriaActivo>, IIncludableQueryable<EventoAuditoriaActivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }



        #region sin implementar

        public Task<IEnumerable<EventoAuditoriaActivo>> CrearAsync(params EventoAuditoriaActivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventoAuditoriaActivo>> CrearAsync(IEnumerable<EventoAuditoriaActivo> entities, CancellationToken cancellationToken = default)
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


        public Task<List<EventoAuditoriaActivo>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<EventoAuditoriaActivo>> ObtenerPaginadoAsync(Expression<Func<EventoAuditoriaActivo, bool>> predicate = null, Func<IQueryable<EventoAuditoriaActivo>, IOrderedQueryable<EventoAuditoriaActivo>> orderBy = null, Func<IQueryable<EventoAuditoriaActivo>, IIncludableQueryable<EventoAuditoriaActivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {
            throw new NotImplementedException();
        }

        public Task<EventoAuditoriaActivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }



        #endregion


    }
}
