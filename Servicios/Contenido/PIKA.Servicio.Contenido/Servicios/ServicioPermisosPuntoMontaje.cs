using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioPermisosPuntoMontaje : ContextoServicioContenido,
        IServicioInyectable, IServicioPermisosPuntoMontaje
    {

        private const string DEFAULT_SORT_COL = "DestinatarioId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PermisosPuntoMontaje> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        
        public ServicioPermisosPuntoMontaje(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<PermisosPuntoMontaje>(new QueryComposer<PermisosPuntoMontaje>());
        }

        public async Task<bool> Existe(Expression<Func<PermisosPuntoMontaje, bool>> predicado)
        {
            List<PermisosPuntoMontaje> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<PermisosPuntoMontaje> CrearAsync(PermisosPuntoMontaje entity, CancellationToken cancellationToken = default)
        {
            try
            {
                var pm = await this.repo.UnicoAsync(x => x.PuntoMontajeId == entity.PuntoMontajeId && x.DestinatarioId == entity.DestinatarioId);
                if (pm != null)
                {
                    entity.Id = pm.Id;
                    UDT.Context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    entity.Id = Guid.NewGuid().ToString();
                    await this.repo.CrearAsync(entity);
                }

                UDT.SaveChanges();
                return entity.Copia();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                throw;
            }
           
        }


        public async Task ActualizarAsync(PermisosPuntoMontaje entity)
        {
            var pm = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (pm != null)
            {
                entity.Id = pm.Id;
                entity.PuntoMontajeId = pm.PuntoMontajeId;
                entity.DestinatarioId = pm.DestinatarioId;
                UDT.Context.Entry(entity).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            PermisosPuntoMontaje d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                Console.WriteLine(Id);
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    this.UDT.Context.Entry(d).State = EntityState.Deleted;
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
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
        public async Task<IPaginado<PermisosPuntoMontaje>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PermisosPuntoMontaje>, IIncludableQueryable<PermisosPuntoMontaje, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }
        public async  Task<PermisosPuntoMontaje> UnicoAsync(Expression<Func<PermisosPuntoMontaje, bool>> predicado = null, Func<IQueryable<PermisosPuntoMontaje>, IOrderedQueryable<PermisosPuntoMontaje>> ordenarPor = null, Func<IQueryable<PermisosPuntoMontaje>, IIncludableQueryable<PermisosPuntoMontaje, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            PermisosPuntoMontaje d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);

            return d.Copia();
        }

        #region NO Implementados


        public Task<IEnumerable<PermisosPuntoMontaje>> CrearAsync(params PermisosPuntoMontaje[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PermisosPuntoMontaje>> CrearAsync(IEnumerable<PermisosPuntoMontaje> entities, CancellationToken cancellationToken = default)
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




        public Task<List<PermisosPuntoMontaje>> ObtenerAsync(Expression<Func<PermisosPuntoMontaje, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermisosPuntoMontaje>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<PermisosPuntoMontaje>> ObtenerPaginadoAsync(Expression<Func<PermisosPuntoMontaje, bool>> predicate = null, Func<IQueryable<PermisosPuntoMontaje>, IOrderedQueryable<PermisosPuntoMontaje>> orderBy = null, Func<IQueryable<PermisosPuntoMontaje>, IIncludableQueryable<PermisosPuntoMontaje, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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

