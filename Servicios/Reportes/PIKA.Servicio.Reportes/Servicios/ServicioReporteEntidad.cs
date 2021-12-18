using LazyCache;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Reportes;
using PIKA.Servicio.Reportes.Data;
using PIKA.Servicio.Reportes.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Reportes.Servicios
{
   public class ServicioReporteEntidad : ContextoServicioRepoerteEntidad,
        IServicioInyectable, IServicioReporteEntidad
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ReporteEntidad> repo;
        private UnidadDeTrabajo<DbContextReportes> UDT;
        public ServicioReporteEntidad(IProveedorOpcionesContexto<DbContextReportes> proveedorOpciones,
           ILogger<ServicioReporteEntidad> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextReportes>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ReporteEntidad>(new QueryComposer<ReporteEntidad>());
        }
        public async Task<bool> Existe(Expression<Func<ReporteEntidad, bool>> predicado)
        {
            List<ReporteEntidad> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<ReporteEntidad> CrearAsync(ReporteEntidad entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
                entity.Id = Guid.NewGuid().ToString();
                entity.Nombre = entity.Nombre.Trim();
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();

            return entity.Copia();
        }
        public async Task ActualizarAsync(ReporteEntidad entity)
        {
            ReporteEntidad o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            if (o == null)
                throw new EXNoEncontrado(entity.Id);

            if (await Existe(x => x.Id != entity.Id.Trim() && x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
           
                o.Nombre = entity.Nombre.Trim();
                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            
        }
        public Task<List<ReporteEntidad>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
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
        public async Task<IPaginado<ReporteEntidad>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ReporteEntidad>, IIncludableQueryable<ReporteEntidad, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public Task<IPaginado<ReporteEntidad>> ObtenerPaginadoAsync(Expression<Func<ReporteEntidad, bool>> predicate = null, Func<IQueryable<ReporteEntidad>, IOrderedQueryable<ReporteEntidad>> orderBy = null, Func<IQueryable<ReporteEntidad>, IIncludableQueryable<ReporteEntidad, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            return this.repo.ObtenerPaginadoAsync(predicate);
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ReporteEntidad o;
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
        public async Task<ReporteEntidad> UnicoAsync(Expression<Func<ReporteEntidad, bool>> predicado = null, Func<IQueryable<ReporteEntidad>, IOrderedQueryable<ReporteEntidad>> ordenarPor = null, Func<IQueryable<ReporteEntidad>, IIncludableQueryable<ReporteEntidad, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ReporteEntidad t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }

        public Task<List<ReporteEntidad>> ObtenerAsync(Expression<Func<ReporteEntidad, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

       

        #region Sin Implementar

        public Task<IEnumerable<ReporteEntidad>> CrearAsync(params ReporteEntidad[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReporteEntidad>> CrearAsync(IEnumerable<ReporteEntidad> entities, CancellationToken cancellationToken = default)
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




        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        #endregion



    }
}
