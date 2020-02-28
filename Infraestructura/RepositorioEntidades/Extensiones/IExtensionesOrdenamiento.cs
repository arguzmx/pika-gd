using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace RepositorioEntidades
{
    public static class IExtensionesOrdenamiento
    {

        public static IQueryable<T> OrdenarPor<T>(this IQueryable<T> origen, string columna, bool ascendente = true)
        {
            if (String.IsNullOrEmpty(columna))
            {
                return origen;
            }

            ParameterExpression parameter = Expression.Parameter(origen.ElementType, "");

            MemberExpression property = Expression.Property(parameter, columna);
            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = ascendente ? "OrderBy" : "OrderByDescending";

            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                  new Type[] { origen.ElementType, property.Type },
                                  origen.Expression, Expression.Quote(lambda));

            return origen.Provider.CreateQuery<T>(methodCallExpression);
        }


        #region Sort by string
        private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> origen, string propiedad, bool descencente, bool otronivel)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
            MemberExpression property = Expression.PropertyOrField(param, propiedad);
            LambdaExpression sort = Expression.Lambda(property, param);
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                (!otronivel ? "OrderBy" : "ThenBy") + (descencente ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                origen.Expression,
                Expression.Quote(sort));
            return (IOrderedQueryable<T>)origen.Provider.CreateQuery<T>(call);
        }
        public static IOrderedQueryable<T> OrdenarPor<T>(this IQueryable<T> origen, string propiedad)
        {
            return OrderingHelper(origen, propiedad, false, false);
        }
        public static IOrderedQueryable<T> OrdenarPorDesc<T>(this IQueryable<T> origen, string propiedad)
        {
            return OrderingHelper(origen, propiedad, true, false);
        }
        public static IOrderedQueryable<T> LuegoPor<T>(this IOrderedQueryable<T> origen, string propiedad)
        {
            return OrderingHelper(origen, propiedad, false, true);
        }
        public static IOrderedQueryable<T> LuegoPorDesc<T>(this IOrderedQueryable<T> origen, string propiedad)
        {
            return OrderingHelper(origen, propiedad, true, true);
        }
        #endregion
    }
}
