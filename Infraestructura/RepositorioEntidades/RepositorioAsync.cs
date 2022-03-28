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
        public void DetachAllEntities()
        {
            var changedEntriesCopy = this._dbContext.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
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
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (inhabilitarSeuimiento) query = query.AsNoTracking();

            if (incluir != null) query = incluir(query);

            if (predicado != null) query = query.Where(predicado);

            if (ordenarPor != null)
                return await ordenarPor(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }


        public Task<IPaginado<T>> ObtenerPaginadoAsync(Expression<Func<T, bool>> predicado, Consulta consulta,
               Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
               bool inhabilitarSeguimiento = true,
               CancellationToken tokenCancelacion = default(CancellationToken))
        {


                IQueryable<T> query = _dbSet.AsNoTracking();


                if (inhabilitarSeguimiento) query = query.AsNoTracking();

                if (incluir != null) query = incluir(query);

                if (predicado != null) query = query.Where(predicado);

                query = query.OrdenarPor(consulta.ord_columna, consulta.ord_direccion.ToLower() == "desc" ? false : true);

                return query.PaginadoAsync(consulta.indice, consulta.tamano, tokenCancelacion);

        }


      

        public Task<IPaginado<T>> ObtenerPaginadoAsync(Consulta consulta,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
           List<Expression<Func<T, bool>>> filtros = null,
           bool inhabilitarSeguimiento = true,
           CancellationToken tokenCancelacion = default(CancellationToken))
        {

                if (filtros == null) filtros = new List<Expression<Func<T, bool>>>();

                IQueryable<T> query = _dbSet.AsNoTracking();


                if (inhabilitarSeguimiento) query = query.AsNoTracking();

                if (incluir != null) query = incluir(query);
       
                if (consulta.Filtros.Count > 0 || filtros.Count > 0)
                {

                    var type = typeof(T);
                    ParameterExpression pe = Expression.Parameter(type, "search");


                    Expression predicateBody = _compositor.Componer(pe, consulta);
                    Expression<Func<T, bool>> lambdaPredicado = null;
                    if (predicateBody != null) {
                        lambdaPredicado = Expression.Lambda<Func<T, bool>>(predicateBody, pe);
                        filtros.Insert(0, lambdaPredicado);
                    };

    
                    if (filtros.Count > 0)
                    {
                        var filtro = filtros[0];
                       for(int i = 1; i < filtros.Count; i++)
                       {
                            filtro = filtro.AndAlso(filtros[i]);
                       }

                        MethodCallExpression whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { query.ElementType },
                        query.Expression,
                        filtro);

                        query = query.Provider.CreateQuery<T>(whereCallExpression);
                    }
                }

                query = query.OrdenarPor(consulta.ord_columna, consulta.ord_direccion.ToLower() == "desc" ? false : true);

                return query.PaginadoAsync(consulta.indice, consulta.tamano,  tokenCancelacion);
        }



        public Task<IPaginado<T>> ObtenerPaginadoDesdeAsync(Consulta consulta, int desde,
   Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
   List<Expression<Func<T, bool>>> filtros = null,
   bool inhabilitarSeguimiento = true,
   CancellationToken tokenCancelacion = default(CancellationToken))
        {


            if (filtros == null) filtros = new List<Expression<Func<T, bool>>>();

            IQueryable<T> query = _dbSet.AsNoTracking();


            if (inhabilitarSeguimiento) query = query.AsNoTracking();

            if (incluir != null) query = incluir(query);

            if (consulta.Filtros.Count > 0 || filtros.Count > 0)
            {
                var type = typeof(T);
                ParameterExpression pe = Expression.Parameter(type, "search");


                Expression predicateBody = _compositor.Componer(pe, consulta);
                Expression<Func<T, bool>> lambdaPredicado = null;
                if (predicateBody != null)
                {
                    lambdaPredicado = Expression.Lambda<Func<T, bool>>(predicateBody, pe);
                    filtros.Insert(0, lambdaPredicado);
                };


                if (filtros.Count > 0)
                {
                    var filtro = filtros[0];
                    for (int i = 1; i < filtros.Count; i++)
                    {
                        filtro = filtro.AndAlso(filtros[i]);
                    }

                    MethodCallExpression whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { query.ElementType },
                    query.Expression,
                    filtro);

                    query = query.Provider.CreateQuery<T>(whereCallExpression);
                }
            }

            query = query.OrdenarPor(consulta.ord_columna, consulta.ord_direccion.ToLower() == "desc" ? false : true);
            return query.PaginadoAsync(desde, consulta.indice, consulta.tamano, tokenCancelacion);
        }

        public Task<IPaginado<T>> ObtenerConteoAsync(Consulta consulta,
          CancellationToken tokenCancelacion = default(CancellationToken))
        {

            List<Expression<Func<T, bool>>> filtros = null;
            if (filtros == null) filtros = new List<Expression<Func<T, bool>>>();

            IQueryable<T> query = _dbSet;
            query = query.AsNoTracking();

            if (consulta.Filtros.Count > 0 || filtros.Count > 0)
            {

                var type = typeof(T);
                ParameterExpression pe = Expression.Parameter(type, "search");


                Expression predicateBody = _compositor.Componer(pe, consulta);
                Expression<Func<T, bool>> lambdaPredicado = null;
                if (predicateBody != null)
                {
                    lambdaPredicado = Expression.Lambda<Func<T, bool>>(predicateBody, pe);
                    filtros.Insert(0, lambdaPredicado);
                };

                if (filtros.Count > 0)
                {
                    var filtro = filtros[0];
                    for (int i = 1; i < filtros.Count; i++)
                    {
                        filtro = filtro.AndAlso(filtros[i]);
                    }

                    MethodCallExpression whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { query.ElementType },
                    query.Expression,
                    filtro);

                    query = query.Provider.CreateQuery<T>(whereCallExpression);
                }
            }
            
            return query.ConteoAsync(consulta.indice, consulta.tamano, tokenCancelacion);
        }



        public Task<IPaginado<T>> ObtenerPaginadoAsync(Expression<Func<T, bool>> predicado = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
            int indice = 0,
            int tamano = 20,
            bool inhabilitarSeguimiento = true,
            CancellationToken tokenCancelacion = default(CancellationToken))
        {
            IQueryable<T> query = _dbSet.AsNoTracking();
            if (inhabilitarSeguimiento) query = query.AsNoTracking();

            if (incluir != null) query = incluir(query);

            if (predicado != null) query = query.Where(predicado);

            if (ordenarPor != null)
                return ordenarPor(query).PaginadoAsync(indice, tamano,  tokenCancelacion);

            return query.PaginadoAsync(indice, tamano,  tokenCancelacion);
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



        public async Task<List<T>> ObtenerAsync(Expression<Func<T, bool>> predicado,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (incluir != null) query = incluir(query);

            if (predicado != null) query = query.Where(predicado);

            if (ordenarPor != null)
                return await ordenarPor(query).ToListAsync();

            return query.ToList();
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

        public async Task EliminarRango(List<T> entities)
        {
            _dbContext.RemoveRange(entities);
            await Task.Delay(1);

        }

        public async Task<List<T>> ObtenerAsync(string comandoSql)
        {


            await Task.Delay(1);
            return _dbSet.FromSqlInterpolated<T>($"{comandoSql}").ToList();

        }


    }
}
