﻿using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    public interface IRepositorioAsync<T>
    {

        void DetachAllEntities();
        Task<T> UnicoAsync(Expression<Func<T, bool>> predicado = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> incluir = null,
            bool inhabilitarSegumiento = true);

        Task<List<T>> ObtenerAsync(Expression<Func<T, bool>> predicado,
              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<List<T>> ObtenerAsync(string SqlCommand);

        
        Task<IPaginado<T>> ObtenerPaginadoAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int index = 0,
            int size = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IPaginado<T>> ObtenerPaginadoAsync(Consulta Query,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
           List<Expression<Func<T, bool>>> filtros = null,
           bool disableTracking = true,
           CancellationToken cancellationToken = default(CancellationToken));

        Task<IPaginado<T>> ObtenerPaginadoDesdeAsync(Consulta Query, int desde,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
           List<Expression<Func<T, bool>>> filtros = null,
           bool disableTracking = true,
           CancellationToken cancellationToken = default(CancellationToken));

        Task<IPaginado<T>> ObtenerConteoAsync(Consulta Query,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IPaginado<T>> ObtenerPaginadoAsync(Expression<Func<T, bool>> predicate , Consulta Query,
                Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                bool disableTracking = true,
                CancellationToken cancellationToken = default(CancellationToken));

        Task CrearAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));

        Task CrearAsync(params T[] entities);

        Task CrearAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default(CancellationToken));


        void ActualizarAsync(T entity);

        Task Eliminar(T enntity);

        Task EliminarRango(List<T> entities);

        Task EjecutarSql(string sqlCommand);

        Task EjecutarSqlBatch(List<string> sqlCommand);

    }
}
