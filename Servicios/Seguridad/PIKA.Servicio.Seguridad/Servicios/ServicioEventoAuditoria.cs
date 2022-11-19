using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Validadores;
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
        private UnidadDeTrabajo<DbContextSeguridad> UDT;

        public UsuarioAPI usuario { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ContextoRegistroActividad RegistroActividad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public PermisoAplicacion permisos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ServicioEventoAuditoria(
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioEventoAuditoria> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EventoAuditoria>(new QueryComposer<EventoAuditoria>());

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
                throw new ExElementoExistente(entity.Id);
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
            throw new NotImplementedException();
        }

        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {
            throw new NotImplementedException();
        }

        public Task<EventoAuditoria> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }



        #endregion


    }
}
