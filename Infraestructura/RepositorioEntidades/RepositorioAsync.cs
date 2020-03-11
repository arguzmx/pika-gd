using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    public class RepositorioAsync<T> : IRepositorioAsync<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        private ICompositorConsulta<T> _compositor;

        public RepositorioAsync(DbContext dbContext, ICompositorConsulta<T> compositor)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _compositor = compositor;
        }

        public async Task<T> UnicoAsync(Expression<Func<T, bool>> predicado = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null, bool inhabilitarSeuimiento = true)
        {
            IQueryable<T> query = _dbSet;
            if (inhabilitarSeuimiento) query = query.AsNoTracking();

            if (incluir != null) query = incluir(query);

            if (predicado != null) query = query.Where(predicado);

            if (ordenarPor != null)
                return await ordenarPor(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }




        public Task<IPaginado<T>> ObtenerPaginadoAsync(Consulta consulta,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
           bool inhabilitarSeguimiento = true,
           CancellationToken tokenCancelacion = default(CancellationToken))
        {
            try
            {


                IQueryable<T> query = _dbSet;

                Console.WriteLine($"{consulta.tamano} {consulta.columna_ordenamiento} {consulta.direccion_ordenamiento}");

                if (inhabilitarSeguimiento) query = query.AsNoTracking();

                if (incluir != null) query = incluir(query);

                if (consulta.Filtros.Count > 0)
                {
                    var type = typeof(T);
                    ParameterExpression pe = Expression.Parameter(type, "search");


                    Expression predicateBody = _compositor.Componer(pe, consulta);

     
                    if (predicateBody != null)
                    {
                        MethodCallExpression whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { query.ElementType },
                        query.Expression,
                        Expression.Lambda<Func<T, bool>>(predicateBody, pe));

                        query = query.Provider.CreateQuery<T>(whereCallExpression);
                    }

                }



                query = query.OrdenarPor(consulta.columna_ordenamiento, consulta.direccion_ordenamiento.ToLower() == "desc" ? false : true);

                return query.PaginadoAsync(consulta.indice, consulta.tamano, 0, tokenCancelacion);
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public Task<IPaginado<T>> ObtenerPaginadoAsync(Expression<Func<T, bool>> predicado = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
            int indice = 0,
            int tamano = 20,
            bool inhabilitarSeguimiento = true,
            CancellationToken tokenCancelacion = default(CancellationToken))
        {
            IQueryable<T> query = _dbSet;
            if (inhabilitarSeguimiento) query = query.AsNoTracking();

            if (incluir != null) query = incluir(query);

            if (predicado != null) query = query.Where(predicado);

            if (ordenarPor != null)
                return ordenarPor(query).PaginadoAsync(indice, tamano, 0, tokenCancelacion);

            return query.PaginadoAsync(indice, tamano, 0, tokenCancelacion);
        }

        public Task CrearAsync(T entity, CancellationToken cancellationToken = default(CancellationToken))
        {

            return _dbSet.AddAsync(entity, cancellationToken).AsTask();
        }

        public Task CrearAsync(params T[] entities)
        {
            return _dbSet.AddRangeAsync(entities);
        }


        public Task CrearAsync(IEnumerable<T> entities,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return _dbSet.AddRangeAsync(entities, cancellationToken);
        }



        public Task<List<T>> ObtenerAsync(Expression<Func<T, bool>> predicado)
        {
            IQueryable<T> query = _dbSet;
            if (predicado != null) query = query.Where(predicado);

            return query.ToListAsync();
        }

        public void ActualizarAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public Task CrearAsync(T entity)
        {
            return CrearAsync(entity, new CancellationToken());
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            await _dbContext.Database.ExecuteSqlRawAsync(sqlCommand);
        }

        public async Task EjecutarSqlBatch(List<string> sqlBatch)
        {
            foreach (string s in sqlBatch)
            {
                await _dbContext.Database.ExecuteSqlRawAsync(s);
            }
        }

        public async Task Eliminar(T entity)
        {
            _dbContext.Remove<T>(entity);
            await Task.Delay(1);

        }

        public async Task<IEnumerable<T>> ObtenerListaAsync(string comandoSql)
        {


            await Task.Delay(1);
            return _dbSet.FromSqlInterpolated<T>($"{comandoSql}");

        }

 
    }
}
