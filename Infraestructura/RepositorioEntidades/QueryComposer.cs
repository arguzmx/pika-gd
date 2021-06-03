using LazyCache;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace RepositorioEntidades
{
    public  class QueryComposer<T>: ICompositorConsulta<T>
    {
        private IAppCache cache;
        public QueryComposer()
        {
          

        }

        public QueryComposer(IAppCache cache)
        {

            this.cache = cache;
        }

        #region expression composer

        private  Expression GetDateTimeExpression(ParameterExpression x, PropertyInfo p,
            string Operador, string Value, bool negar)
        {

            //2020-07-14T12:33:12-05:00,2020-11-27T12:43:08-06:00
            string[] valores = Value.Split(',');

            List<DateTime> fechas = new List<DateTime>();
            foreach (string v in valores)
            {
                if (DateTime.TryParse(v, out DateTime f))
                {
                    fechas.Add(f);
                }
            }

            if (fechas.Count == 0)
            {
                return null;
            }else
            {
                switch(Operador)
                {
                    case FiltroConsulta.OP_BETWEN:
                        if (fechas.Count < 2) return null;
                        break;
                }
            }



            Expression exp = Expression.Property(x, p.Name);
            Expression comparison = null;
            Expression hasValueExpression = null;
            Expression valueExpression = null;
            bool nulable = Nullable.GetUnderlyingType(p.PropertyType) != null;
            //UnaryExpression nullCheck = null;
            //BinaryExpression IsNUll = null;

            if (nulable)
            {
                hasValueExpression = Expression.Property(exp, "HasValue");
                valueExpression = Expression.Property(exp, "Value");
            }

                switch (Operador)
                {

                //case FiltroConsulta.OP_ISNULL:
                //    comparison = IsNUll;
                //    break;

                //case FiltroConsulta.OP_ISNOTNULL:
                //    comparison = nullCheck;
                //    break;

                case FiltroConsulta.OP_BETWEN:
                    if (nulable)
                    {
                        Expression despuesE = Expression.GreaterThanOrEqual(valueExpression, Expression.Constant(fechas[0]));
                        Expression anteE = Expression.LessThanOrEqual(valueExpression, Expression.Constant(fechas[1]));
                        Expression despuesEV = Expression.AndAlso(hasValueExpression, despuesE);
                        Expression anteEV = Expression.AndAlso(hasValueExpression, anteE);
                        comparison = Expression.AndAlso(despuesEV, anteEV);
                    }
                    else
                    {
                        Expression despuesE = Expression.GreaterThanOrEqual(exp, Expression.Constant(fechas[0]));
                        Expression anteE = Expression.LessThanOrEqual(exp, Expression.Constant(fechas[1]));
                        comparison = Expression.AndAlso(despuesE, anteE);
                    }
                    break;

                    case FiltroConsulta.OP_NEQ:
                        if (nulable)
                        {
                            Expression tmp = Expression.NotEqual(valueExpression, Expression.Constant(fechas[0]));
                            comparison = Expression.AndAlso(hasValueExpression, tmp);

                        } else
                        {
                            comparison = Expression.NotEqual(exp, Expression.Constant(fechas[0]));
                        }
                        
                        break;

                    case FiltroConsulta.OP_EQ:
                        if (nulable)
                        {
                            Expression tmp = Expression.Equal(valueExpression, Expression.Constant(fechas[0]));
                            comparison = Expression.AndAlso(hasValueExpression, tmp);
                        }
                        else
                        {
                            comparison = Expression.Equal(exp, Expression.Constant(fechas[0]));
                        }
                        break;

                    case FiltroConsulta.OP_GT:
                        if (nulable)
                        {
                            Expression tmp = Expression.GreaterThan(valueExpression, Expression.Constant(fechas[0]));
                            comparison = Expression.AndAlso(hasValueExpression, tmp);
                        }
                        else
                        {
                            comparison = Expression.GreaterThan(exp, Expression.Constant(fechas[0]));
                        }
                        
                        break;

                    case FiltroConsulta.OP_GTE:
                        if (nulable)
                        {
                            Expression tmp = Expression.GreaterThanOrEqual(valueExpression, Expression.Constant(fechas[0]));
                            comparison = Expression.AndAlso(hasValueExpression, tmp);
                        }
                        else
                        {
                            comparison = Expression.GreaterThanOrEqual(exp, Expression.Constant(fechas[0]));
                        }
                        
                        break;

                    case FiltroConsulta.OP_LT:
                        if (nulable)
                        {
                            Expression tmp = Expression.LessThan(valueExpression, Expression.Constant(fechas[0]));
                            comparison = Expression.AndAlso(hasValueExpression, tmp);
                        }
                        else
                        {
                            comparison = Expression.LessThan(exp, Expression.Constant(fechas[0]));
                        }
                        break;

                    case FiltroConsulta.OP_LTE:
                        if (nulable)
                        {
                            Expression tmp = Expression.LessThanOrEqual(valueExpression, Expression.Constant(fechas[0]));
                            comparison = Expression.AndAlso(hasValueExpression, tmp);
                        }
                        else
                        {
                            comparison = Expression.LessThanOrEqual(exp, Expression.Constant(fechas[0]));
                        }
                        break;

                    case FiltroConsulta.OP_EXIST:
                        break;

                    default:
                        return null;
                }



            if (!negar)
            {
                return comparison;
            }
            else
            {
                return Expression.Not(comparison);
            }


        }



        private  Expression GetNumExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value, bool negar)
        {

          

            Expression pe = Expression.Property(x, p.Name);
            
            //BinaryExpression IsNUll = Expression.Equal(pe, Expression.Constant(null, typeof(int)));
            //Expression isNotNull = Expression.Not(IsNUll);
            Expression constantExpression = null;
            Expression constantExpression2 = null;
            Expression final = null;

            string[] valores = Value.Split(',');
            List<string> numeros = new List<string>();
            foreach (string v in valores)
            {
                if (decimal.TryParse(v, out decimal f))
                {
                    numeros.Add(v);
                }
            }

            if (numeros.Count == 0)
            {
                return null;
            }
            else
            {
                switch (Operador)
                {
                    case FiltroConsulta.OP_BETWEN:
                        if (numeros.Count < 2) return null;
                        break;
                }
            }


            switch (p.PropertyType)
            {

                case Type intTypeNull when intTypeNull == typeof(int?):
                case Type intType when intType == typeof(int):
                    constantExpression = Expression.Constant(int.Parse(numeros[0]));
                    constantExpression2 = numeros.Count<2 ? null : Expression.Constant(int.Parse(numeros[1]));
                    break;

                case Type longypeNull when longypeNull == typeof(long?):
                case Type longype when longype == typeof(long):
                    constantExpression = Expression.Constant(long.Parse(numeros[0]));
                    constantExpression2 = numeros.Count < 2 ? null : Expression.Constant(long.Parse(numeros[1]));
                    break;


                case Type floatypeNull when floatypeNull == typeof(float?):
                case Type floatype when floatype == typeof(float):
                    constantExpression = Expression.Constant(float.Parse(numeros[0]));
                    constantExpression2 = numeros.Count < 2 ? null : Expression.Constant(float.Parse(numeros[1]));
                    break;

                case Type decimalTypeNull when decimalTypeNull == typeof(decimal?):
                case Type decimalType when decimalType == typeof(decimal):
                    constantExpression = Expression.Constant(decimal.Parse(numeros[0]));
                    constantExpression2 = numeros.Count < 2 ? null : Expression.Constant(decimal.Parse(numeros[1]));
                    break;
            }

            if (constantExpression != null)
            {

                switch (Operador)
                {
                    //case FiltroConsulta.OP_ISNULL:
                    //    final = IsNUll;
                    //    break;

                    //case FiltroConsulta.OP_ISNOTNULL:
                    //    final = isNotNull;
                    //    break;

                    case FiltroConsulta.OP_BETWEN:
                        Expression despuesE = Expression.GreaterThanOrEqual(pe, constantExpression);
                        Expression anteE = Expression.LessThanOrEqual(pe, constantExpression2);
                        final = Expression.AndAlso(despuesE, anteE);
                        break;

                    //case FiltroConsulta.OP_NEQ:
                    //    final = Expression.AndAlso(isNotNull, Expression.NotEqual(pe, constantExpression));
                    //    break;

                    case FiltroConsulta.OP_EQ:
                        //final = Expression.AndAlso(isNotNull, Expression.Equal(pe, constantExpression));
                            final = Expression.Equal(pe, constantExpression);
                            break;

                    case FiltroConsulta.OP_GT:
                        //final = Expression.AndAlso(isNotNull, Expression.GreaterThan(pe, constantExpression));
                            final = Expression.GreaterThan(pe, constantExpression);
                            break;

                    case FiltroConsulta.OP_GTE:
                        //final = Expression.AndAlso(isNotNull, Expression.GreaterThanOrEqual(pe, constantExpression));
                            final = Expression.GreaterThanOrEqual(pe, constantExpression);
                            break;

                    case FiltroConsulta.OP_LT:
                        //final = Expression.AndAlso(isNotNull, Expression.LessThan(pe, constantExpression));
                            final = Expression.LessThan(pe, constantExpression);
                            break;

                    case FiltroConsulta.OP_LTE:
                        //final = Expression.AndAlso(isNotNull, Expression.LessThanOrEqual(pe, constantExpression));
                            final = Expression.LessThanOrEqual(pe, constantExpression);
                            break;

                    case FiltroConsulta.OP_EXIST:

                        switch (p.PropertyType)
                        {
                            case Type intType when intType == typeof(int):

                                List<int> lint = new List<int>();
                                foreach (string s in Value.Split(',').ToList())
                                {
                                    if (int.TryParse(s, out int i))
                                    {
                                        lint.Add(i);
                                    }
                                }
                                Expression<Func<ICollection<int>>> valsrefint = () => lint;
                                var valsint = valsrefint.Body;
                                var miContainsint = valsrefint.Body.Type.GetMethod("Contains", new[] { typeof(string) });
                                //final = Expression.AndAlso(isNotNull, Expression.Call(valsint, miContainsint, pe));
                                    final = Expression.Call(valsint, miContainsint, pe);
                                    break;

                            case Type longype when longype == typeof(long):
                                List<long> llong = new List<long>();
                                foreach (string s in Value.Split(',').ToList())
                                {
                                    if (long.TryParse(s, out long i))
                                    {
                                        llong.Add(i);
                                    }
                                }
                                Expression<Func<ICollection<long>>> valsreflong = () => llong;
                                var valslong = valsreflong.Body;
                                var miContainslong = valsreflong.Body.Type.GetMethod("Contains", new[] { typeof(string) });
                                //final = Expression.AndAlso(isNotNull, Expression.Call(valslong, miContainslong, pe));
                                    final = Expression.Call(valslong, miContainslong, pe);
                                    break;


                            case Type floatype when floatype == typeof(float):
                                List<float> lfloat = new List<float>();
                                foreach (string s in Value.Split(',').ToList())
                                {
                                    if (float.TryParse(s, out float i))
                                    {
                                        lfloat.Add(i);
                                    }
                                }
                                Expression<Func<ICollection<float>>> valsreffloat = () => lfloat;
                                var valsfloat = valsreffloat.Body;
                                var miContainsfloat = valsreffloat.Body.Type.GetMethod("Contains", new[] { typeof(string) });
                                //final = Expression.AndAlso(isNotNull, Expression.Call(valsfloat, miContainsfloat, pe));
                                    final = Expression.Call(valsfloat, miContainsfloat, pe);
                                    break;

                            case Type decimalType when decimalType == typeof(decimal):
                                List<decimal> ldecimal = new List<decimal>();
                                foreach (string s in Value.Split(',').ToList())
                                {
                                    if (decimal.TryParse(s, out decimal i))
                                    {
                                        ldecimal.Add(i);
                                    }
                                }
                                Expression<Func<ICollection<decimal>>> valsrefdecimal = () => ldecimal;
                                var valsdecimal = valsrefdecimal.Body;
                                var miContainsdecimal = valsrefdecimal.Body.Type.GetMethod("Contains", new[] { typeof(string) });
                                //final = Expression.AndAlso(isNotNull, Expression.Call(valsdecimal, miContainsdecimal, pe));
                                    final = Expression.Call(valsdecimal, miContainsdecimal, pe);
                                    break;
                        }

                        break;

                    default:
                        return null;
                }
            }



            if (!negar)
            {
                return final;
            }
            else
            {
                return Expression.Not(final);
            }
 
        }

        private  Expression GetStringExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value, bool negar)
        {
            Expression pe = Expression.Property(x, p.Name);
            BinaryExpression isNotNull = Expression.NotEqual(pe, Expression.Constant(null, typeof(object)));
            BinaryExpression IsNUll = Expression.Equal(pe, Expression.Constant(null, typeof(object)));
            Expression constantExpression = Expression.Constant(Value.ToLower(), typeof(string));
            Expression toLower = Expression.Call(pe, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
            

            Expression final = null;
   
            if (!string.IsNullOrEmpty(Value))
            {
                switch (Operador)
                {
                    case FiltroConsulta.OP_ISNULL:
                        final = IsNUll;
                        break;

                    case FiltroConsulta.OP_ISNOTNULL:
                        final = isNotNull;
                        break;

                    case FiltroConsulta.OP_NEQ:
                        final = Expression.AndAlso(isNotNull, Expression.NotEqual(toLower, constantExpression));
                        break;

                    case FiltroConsulta.OP_EQ:
                        final = Expression.AndAlso(isNotNull, Expression.Equal(toLower, constantExpression));
                        break;

                    case FiltroConsulta.OP_CONTAINS:
                        MethodInfo methodcontain = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                        final = Expression.AndAlso(isNotNull, Expression.Call(toLower, methodcontain, constantExpression));
                        break;

                    case FiltroConsulta.OP_STARTS:
                        MethodInfo methodstart = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                        final = Expression.AndAlso(isNotNull, Expression.Call(toLower, methodstart, constantExpression));
                        break;

                    case FiltroConsulta.OP_ENDS:
                        MethodInfo methodend = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                        final = Expression.AndAlso(isNotNull, Expression.Call(toLower, methodend, constantExpression));
                        break;

                    case FiltroConsulta.OP_EXIST:
                        List<string> l = Value.Split(',').ToList();
                        Expression<Func<ICollection<string>>> valsref =
                            () => l.Select(x => x.ToLower()).ToList();

                        var vals = valsref.Body;
                        var miContains = valsref.Body.Type.GetMethod("Contains", new[] { typeof(string) });
                        final = Expression.AndAlso(isNotNull, Expression.Call(vals, miContains, toLower));

                        break;

                    case FiltroConsulta.OP_INLIST:
                        var b = this.cache.Get<CacheBusqueda>(Value);
                        if (b == null) b = new CacheBusqueda() { Unicos = new List<string>() };
                        Expression<Func<ICollection<string>>> valsrefcache = () =>  b.Unicos ?? new List<string>();
                        var valscache = valsrefcache.Body;
                        var miContainsCache = valsrefcache.Body.Type.GetMethod("Contains", new[] { typeof(string) });
                        final = Expression.AndAlso(isNotNull, Expression.Call(valscache, miContainsCache, toLower));

                        break;

                    default:
                        return null;
                }
            }

            if(! negar)
            {
                return final;
            } else
            {
                return Expression.Not(final);
            }

            
        }


        private  Expression GetBoolExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value, bool negar)
        {

            Expression pe = Expression.Property(x, p.Name);
            bool boolValue = Value.ToLower() != "true" ? false : true;

            Expression constantExpression = Expression.Constant(boolValue);
            Expression final = null;

            switch (Operador)
            {


                case FiltroConsulta.OP_EQ:
                    final = Expression.Equal(pe, constantExpression);
                    break;


                default:
                    return null;
            }



            if (!negar)
            {
                return final;
            }
            else
            {
                return Expression.Not(final);
            }
        }

        #endregion

        public  Expression Componer(ParameterExpression pe, Consulta q)
        {
            Expression final = null;
            try
            {


            
            if (q.Filtros.Count > 0)
            {
                Type type = typeof(T);
                var Props = type.GetProperties().ToList();
                List<Expression> expressions = new List<Expression>();
                int i = 0;
                foreach (var f in q.Filtros)
                {
                    f.Operador = f.Operador.TrimEnd('}');
                    f.Valor = f.Valor.TrimEnd('}');


                        if (string.IsNullOrEmpty(f.Valor) && !string.IsNullOrEmpty(f.ValorString))
                        {
                            f.ValorString = f.ValorString.TrimEnd('}');
                            f.Valor = f.ValorString;
                        }
                        Expression e = null;


                        

                    var p = Props.Where(x => x.Name == f.Propiedad).FirstOrDefault();
                    if (p == null) continue;

                    if (p != null)
                    {
                        switch (p.PropertyType)
                        {
                            case Type intType when intType == typeof(int):
                            case Type longype when longype == typeof(long):
                            case Type floatype when floatype == typeof(float):
                            case Type decimalType when decimalType == typeof(decimal):
                                e = GetNumExpression(pe, p, f.Operador, f.Valor, f.Negacion);
                                break;


                            case Type datetimeTypeNull when datetimeTypeNull == typeof(DateTime?):
                            case Type datetimeType when datetimeType == typeof(DateTime):
                                e = GetDateTimeExpression(pe, p, f.Operador, f.Valor, f.Negacion);
                                break;

                            case Type boolType when boolType == typeof(bool):
                                e = GetBoolExpression(pe, p, f.Operador, f.Valor, f.Negacion);
                                break;


                            case Type stringType when stringType == typeof(string):
                                e = GetStringExpression(pe, p, f.Operador, f.Valor, f.Negacion);
                                break;

                            default:
                                break;
                        }

                        if (e != null)
                        {
                            expressions.Add(e);
                        }
                        foreach (Expression expression in expressions)
                        {
                            final = (final == null) ? expression : Expression.AndAlso(final, expression);
                        }
                    }
                }

            }
          
            return final;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }
}
