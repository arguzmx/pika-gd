using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace RepositorioEntidades
{
    public  class QueryComposer<T>: ICompositorConsulta<T>
    {



        #region expression composer

        private  Expression GetDateTimeExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value)
        {

            Expression exp = Expression.Property(x, p.Name);
            Expression comparison = null;
            BinaryExpression nullCheck = Expression.NotEqual(exp, Expression.Constant(null, typeof(object)));
            BinaryExpression IsNUll = Expression.Equal(exp, Expression.Constant(null, typeof(object)));


            if (DateTime.TryParse(Value, out DateTime t))
            {

                switch (Operador)
                {

                    case FiltroConsulta.OP_ISNULL:
                        comparison = IsNUll;
                        break;

                    case FiltroConsulta.OP_ISNOTNULL:
                        comparison = nullCheck;
                        break;

                    case FiltroConsulta.OP_NEQ:
                        comparison = Expression.AndAlso(nullCheck, Expression.NotEqual(exp, Expression.Constant(t)));
                        break;

                    case FiltroConsulta.OP_EQ:
                        comparison = Expression.AndAlso(nullCheck, Expression.Equal(exp, Expression.Constant(t)));
                        break;

                    case FiltroConsulta.OP_GT:
                        comparison = Expression.AndAlso(nullCheck, Expression.GreaterThan(exp, Expression.Constant(t)));
                        break;

                    case FiltroConsulta.OP_GTE:
                        comparison = Expression.AndAlso(nullCheck, Expression.GreaterThanOrEqual(exp, Expression.Constant(t)));
                        break;

                    case FiltroConsulta.OP_LT:
                        comparison = Expression.AndAlso(nullCheck, Expression.LessThan(exp, Expression.Constant(t)));
                        break;

                    case FiltroConsulta.OP_LTE:
                        comparison = Expression.AndAlso(nullCheck, Expression.LessThanOrEqual(exp, Expression.Constant(t)));
                        break;

                    case FiltroConsulta.OP_EXIST:
                        break;

                    default:
                        return null;
                }
            }


            return comparison;
        }



        private  Expression GetNumExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value)
        {

            Expression pe = Expression.Property(x, p.Name);
            BinaryExpression isNotNull = Expression.NotEqual(pe, Expression.Constant(null, typeof(object)));
            BinaryExpression IsNUll = Expression.Equal(pe, Expression.Constant(null, typeof(object)));
            Expression constantExpression = null;
            Expression final = null;


            switch (p.PropertyType)
            {

                case Type intType when intType == typeof(int):
                    if (int.TryParse(Value, out int i))
                    {
                        constantExpression = Expression.Constant(i);
                    }
                    break;

                case Type longype when longype == typeof(long):
                    if (long.TryParse(Value, out long l))
                    {
                        constantExpression = Expression.Constant(l);
                    }
                    break;

                case Type floatype when floatype == typeof(float):
                    if (float.TryParse(Value, out float f))
                    {
                        constantExpression = Expression.Constant(f);
                    }
                    break;

                case Type decimalType when decimalType == typeof(decimal):
                    if (decimal.TryParse(Value, out decimal d))
                    {
                        constantExpression = Expression.Constant(d);
                    }
                    break;
            }

            if (constantExpression != null)
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
                        final = Expression.AndAlso(isNotNull, Expression.NotEqual(pe, constantExpression));
                        break;

                    case FiltroConsulta.OP_EQ:
                        final = Expression.AndAlso(isNotNull, Expression.Equal(pe, constantExpression));
                        break;

                    case FiltroConsulta.OP_GT:
                        final = Expression.AndAlso(isNotNull, Expression.GreaterThan(pe, constantExpression));
                        break;

                    case FiltroConsulta.OP_GTE:
                        final = Expression.AndAlso(isNotNull, Expression.GreaterThanOrEqual(pe, constantExpression));
                        break;

                    case FiltroConsulta.OP_LT:
                        final = Expression.AndAlso(isNotNull, Expression.LessThan(pe, constantExpression));
                        break;

                    case FiltroConsulta.OP_LTE:
                        final = Expression.AndAlso(isNotNull, Expression.LessThanOrEqual(pe, constantExpression));
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
                                final = Expression.AndAlso(isNotNull, Expression.Call(valsint, miContainsint, pe));
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
                                final = Expression.AndAlso(isNotNull, Expression.Call(valslong, miContainslong, pe));
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
                                final = Expression.AndAlso(isNotNull, Expression.Call(valsfloat, miContainsfloat, pe));
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
                                final = Expression.AndAlso(isNotNull, Expression.Call(valsdecimal, miContainsdecimal, pe));
                                break;
                        }

                        break;

                    default:
                        return null;
                }
            }


            return final;
        }

        private  Expression GetStringExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value)
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

                    default:
                        return null;
                }
            }


            return final;
        }


        private  Expression GetBoolExpression(ParameterExpression x, PropertyInfo p, string Operador, string Value)
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


            return final;
        }

        #endregion

        public  Expression Componer(ParameterExpression pe, Consulta q)
        {
            Expression final = null;


            if (q.Filtros.Count > 0)
            {
                Type type = typeof(T);
                var Props = type.GetProperties().ToList();
                List<Expression> expressions = new List<Expression>();

                foreach (var f in q.Filtros)
                {
                    Expression e = null;
                    var p = Props.Where(x => x.Name.ToLower() == f.Propiedad.ToLower()).FirstOrDefault();
                    if (p != null)
                    {

                        switch (p.PropertyType)
                        {
                            case Type intType when intType == typeof(int):
                            case Type longype when longype == typeof(long):
                            case Type floatype when floatype == typeof(float):
                            case Type decimalType when decimalType == typeof(decimal):
                                e = GetNumExpression(pe, p, f.Operador, f.Valor);
                                break;


                            case Type datetimeType when datetimeType == typeof(DateTime):
                                e = GetDateTimeExpression(pe, p, f.Operador, f.Valor);
                                break;

                            case Type boolType when boolType == typeof(bool):
                                e = GetBoolExpression(pe, p, f.Operador, f.Valor);
                                break;


                            case Type stringType when stringType == typeof(string):
                                e = GetStringExpression(pe, p, f.Operador, f.Valor);
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

    }
}
