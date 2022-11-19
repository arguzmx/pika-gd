using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
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
    public class ServicioAsunto : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioAsunto
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Asunto> repo;
        private IRepositorioAsync<Activo> repoActivo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioAsunto(IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Asunto>(new QueryComposer<Asunto>());
            this.repoActivo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
        }

        public async Task<bool> Existe(Expression<Func<Asunto, bool>> predicado)
        {
            List<Asunto> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteActivo(Expression<Func<Activo, bool>> predicado)
        {
            List<Activo> l = await this.repoActivo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<Asunto> CrearAsync(Asunto entity, CancellationToken cancellationToken = default)
        {
            if (!await ExisteActivo(x => x.Id.Equals(entity.ActivoId.Trim(), StringComparison.InvariantCulture)))
                throw new ExDatosNoValidos(entity.ActivoId);

            if (await Existe(x => x.ActivoId.Equals(entity.ActivoId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.ActivoId);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(Asunto entity)
        {

      
            Asunto o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }


            o.Contenido = entity.Contenido;

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
        public async Task<IPaginado<Asunto>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Asunto>, IIncludableQueryable<Asunto, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        
        public async Task<Asunto> UnicoAsync(Expression<Func<Asunto, bool>> predicado = null, Func<IQueryable<Asunto>, IOrderedQueryable<Asunto>> ordenarPor = null, Func<IQueryable<Asunto>, IIncludableQueryable<Asunto, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Asunto a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }
        public Task<List<Asunto>> ObtenerAsync(Expression<Func<Asunto, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Asunto asunto;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                asunto = await this.repo.UnicoAsync(x => x.Id == Id);
                if (asunto != null)
                {
                    await this.repo.Eliminar(asunto);
                    listaEliminados.Add(asunto.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<Asunto>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);

        }

        #region Sin Implementar
        public Task<IEnumerable<Asunto>> CrearAsync(params Asunto[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Asunto>> CrearAsync(IEnumerable<Asunto> entities, CancellationToken cancellationToken = default)
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
       
        public Task<IPaginado<Asunto>> ObtenerPaginadoAsync(Expression<Func<Asunto, bool>> predicate = null, Func<IQueryable<Asunto>, IOrderedQueryable<Asunto>> orderBy = null, Func<IQueryable<Asunto>, IIncludableQueryable<Asunto, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

