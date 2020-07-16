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
    public class ServicioActivoDeclinado : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivoDeclinado
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ActivoDeclinado> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioActivoDeclinado(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioActivoDeclinado> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ActivoDeclinado>(new QueryComposer<ActivoDeclinado>());
        }

        public async Task<bool> Existe(Expression<Func<ActivoDeclinado, bool>> predicado)
        {
            List<ActivoDeclinado> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ActivoDeclinado> CrearAsync(ActivoDeclinado entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.TransferenciaId == entity.TransferenciaId && x.ActivoId == entity.ActivoId
            && x.Motivo.Equals(entity.Motivo, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Motivo);
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(ActivoDeclinado entity)
        {

            ActivoDeclinado o = await this.repo.UnicoAsync(x => x.ActivoId == entity.ActivoId && x.TransferenciaId == entity.TransferenciaId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ActivoId);
            }

            if (await Existe(x => x.TransferenciaId == entity.TransferenciaId && x.ActivoId == entity.ActivoId
            && x.Motivo.Equals(entity.Motivo, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Motivo);
            }

            o.Motivo = entity.Motivo;

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
        public async Task<IPaginado<ActivoDeclinado>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ActivoDeclinado>, IIncludableQueryable<ActivoDeclinado, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ActivoDeclinado>> CrearAsync(params ActivoDeclinado[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivoDeclinado>> CrearAsync(IEnumerable<ActivoDeclinado> entities, CancellationToken cancellationToken = default)
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
            ActivoDeclinado a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.ActivoId == Id);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.ActivoId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<ICollection<string>> Eliminar(string TransferenciaId, string[] ids)
        {
            ActivoDeclinado a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.ActivoId == Id && x.TransferenciaId == TransferenciaId);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.ActivoId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<ActivoDeclinado>> ObtenerAsync(Expression<Func<ActivoDeclinado, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<ActivoDeclinado>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ActivoDeclinado>> ObtenerPaginadoAsync(Expression<Func<ActivoDeclinado, bool>> predicate = null, Func<IQueryable<ActivoDeclinado>, IOrderedQueryable<ActivoDeclinado>> orderBy = null, Func<IQueryable<ActivoDeclinado>, IIncludableQueryable<ActivoDeclinado, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ActivoDeclinado> UnicoAsync(Expression<Func<ActivoDeclinado, bool>> predicado = null, Func<IQueryable<ActivoDeclinado>, IOrderedQueryable<ActivoDeclinado>> ordenarPor = null, Func<IQueryable<ActivoDeclinado>, IIncludableQueryable<ActivoDeclinado, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ActivoDeclinado t = await this.repo.UnicoAsync(predicado);
            return t.CopiaActivoDeclinado();
        }
    }
}
