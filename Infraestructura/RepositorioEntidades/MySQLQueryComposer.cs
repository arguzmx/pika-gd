using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RepositorioEntidades
{
    public static class MySQLQueryComposer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NombreCampo"></param>
        /// <param name="Operador"></param>
        /// <param name="Value"></param>
        /// <param name="negar"></param>
        /// <returns></returns>
        private static string GetStringExpression(string NombreCampo, string Operador, string Value, bool negar)
        {

            string final = null;

            if( Value==null 
                && Operador!= FiltroConsulta.OP_ISNULL
                && Operador != FiltroConsulta.OP_ISNOTNULL)
            {
                return null;
            }

            switch (Operador)
            {
                case FiltroConsulta.OP_ISNULL:
                    final = $" {NombreCampo} is null ";
                    break;

                case FiltroConsulta.OP_ISNOTNULL:
                    final = $"  {NombreCampo} is not null ";
                    break;

                case FiltroConsulta.OP_NEQ:
                    final = $" {NombreCampo} <> '{ Value }' ";
                    break;

                case FiltroConsulta.OP_EQ:
                    final = $" {NombreCampo} = '{ Value }' ";
                    break;

                case FiltroConsulta.OP_CONTAINS:
                    final = $" {NombreCampo} like '%{ Value }%' ";
                    break;

                case FiltroConsulta.OP_STARTS:
                    final = $" {NombreCampo} like '{ Value }%' ";
                    break;

                case FiltroConsulta.OP_ENDS:
                    final = $" {NombreCampo} like '%{ Value }' ";
                    break;

            }

            return final == null ? null : (negar ? $" not ({final})" : final);

        }

        private static string GetNumExpression(string NombreCampo, string Operador, string Value, bool negar)
        {

            string final = null;
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
                if(
                    Operador!= FiltroConsulta.OP_ISNOTNULL 
                    && Operador != FiltroConsulta.OP_ISNULL)
                {
                    return null;
                }
            }
            else
            {
                switch (Operador)
                {
                    case FiltroConsulta.OP_BETWEN:
                        if (numeros.Count < 2) {
                            return null;
                        } else
                        {
                            if(decimal.Parse(numeros[0])> decimal.Parse(numeros[1]))
                            {
                                string tmp = numeros[0];
                                numeros[0] = numeros[1];
                                numeros[1] = tmp;
                            }
                        }
                        break;
                }
            }


            switch (Operador)
            {
                case FiltroConsulta.OP_ISNULL:
                    final = $" {NombreCampo} is null ";
                    break;

                case FiltroConsulta.OP_ISNOTNULL:
                    final = $" {NombreCampo} is not null ";
                    break;

                case FiltroConsulta.OP_BETWEN:
                    final = $" {NombreCampo} between {numeros[0]} and {numeros[1]} ";
                    break;

                case FiltroConsulta.OP_NEQ:
                    final = $" {NombreCampo} <> {numeros[0]} ";
                    break;

                case FiltroConsulta.OP_EQ:
                    final = $" {NombreCampo} = {numeros[0]} ";
                    break;

                case FiltroConsulta.OP_GT:

                    break;

                case FiltroConsulta.OP_GTE:
                    final = $" {NombreCampo} >= {numeros[0]} ";
                    break;

                case FiltroConsulta.OP_LT:
                    final = $" {NombreCampo} < {numeros[0]} ";
                    break;

                case FiltroConsulta.OP_LTE:
                    final = $" {NombreCampo} <= {numeros[0]} ";
                    break;

                //case FiltroConsulta.OP_EXIST:
                //    break;
            }


            return final ==null ? null : (  negar ? $" not ({final})": final);
        }

        private static string GetDateTimeExpression(string NombreCampo, string Operador, string Value, bool negar)
        {

            //2020-07-14T12:33:12-05:00,2020-11-27T12:43:08-06:00
            string[] valores = Value.Split(',');
            string final = null;
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

                if (
                    Operador != FiltroConsulta.OP_ISNOTNULL
                    && Operador != FiltroConsulta.OP_ISNULL)
                {
                    return null;
                }
            }
            else
            {
                switch (Operador)
                {
                    case FiltroConsulta.OP_BETWEN:
                        if (fechas.Count < 2)
                        {
                            return null;
                        }
                        else
                        {
                            if ( fechas[0].Ticks > fechas[1].Ticks)
                            {
                                DateTime tmp = fechas[0];
                                fechas[0] = fechas[1];
                                fechas[1] = tmp;
                            }
                        }
                        break;
                }
            }


 
            switch (Operador)
            {

                case FiltroConsulta.OP_ISNULL:
                    final = $" {NombreCampo} is null ";
                    break;

                case FiltroConsulta.OP_ISNOTNULL:
                    final = $" {NombreCampo} is not null ";
                    break;

                case FiltroConsulta.OP_BETWEN:
                    final = $" {NombreCampo} between {fechas[0].ReemplazoFecha()} and {fechas[1].ReemplazoFecha()} ";
                    break;

                case FiltroConsulta.OP_NEQ:
                    final = $" {NombreCampo} <> {fechas[0].ReemplazoFecha()}";
                    break;

                case FiltroConsulta.OP_EQ:
                    final = $" {NombreCampo} = {fechas[0].ReemplazoFecha()}";
                    break;

                case FiltroConsulta.OP_GT:
                    final = $" {NombreCampo} > {fechas[0].ReemplazoFecha()}";
                    break;

                case FiltroConsulta.OP_GTE:
                    final = $" {NombreCampo} >= {fechas[0].ReemplazoFecha()}";
                    break;

                case FiltroConsulta.OP_LT:
                    final = $" {NombreCampo} < {fechas[0].ReemplazoFecha()}";
                    break;

                case FiltroConsulta.OP_LTE:
                    final = $" {NombreCampo} <= {fechas[0].ReemplazoFecha()}";
                    break;

            }

            return final == null ? null : (negar ? $" not ({final})" : final);

        }

        private static string  GetBoolExpression(string NombreCampo, string Operador, string Value, bool negar)
        {

            string final = null;

            if(string.IsNullOrEmpty(Value) 
                && Operador!= FiltroConsulta.OP_ISNULL
                && Operador != FiltroConsulta.OP_ISNOTNULL)
            {
                return null;
            } else
            {
                if("1,true".IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    Value = "1";
                } else
                {
                    Value = "0";
                }
            }

            switch (Operador)
            {
                case FiltroConsulta.OP_ISNULL:
                    final = $" {NombreCampo} is null ";
                    break;

                case FiltroConsulta.OP_ISNOTNULL:
                    final = $" {NombreCampo} is not null ";
                    break;

                case FiltroConsulta.OP_EQ:
                    final = $" {NombreCampo} = {Value}";
                    break;
            }


            return final == null ? null : (negar ? $" not ({final})" : final);

        }


        public static List<string> Condiciones<T>(Consulta q, string tableId = "")
        {
            Type type = typeof(T);
            var Props = type.GetProperties().ToList();
            List<string> condiciones = new List<string>();


            foreach (var f in q.Filtros)
            {
                f.Operador = f.Operador.TrimEnd('}');
                
                if (!string.IsNullOrEmpty(f.ValorString))
                {
                    f.Valor = f.ValorString.TrimEnd('}');
                }
               

                var p = Props.Where(x => x.Name == f.Propiedad).FirstOrDefault();
                if (p == null) continue;

                if (p != null)
                {
                    string condicion = null;
                    switch (p.PropertyType)
                    {
                        case Type intType when intType == typeof(int):
                        case Type longype when longype == typeof(long):
                        case Type floatype when floatype == typeof(float):
                        case Type decimalType when decimalType == typeof(decimal):
                            condicion = GetNumExpression(tableId + p.ObtieneNombreCampo(), 
                                f.Operador, f.Valor, f.Negacion);
                            break;


                        case Type datetimeTypeNull when datetimeTypeNull == typeof(DateTime?):
                        case Type datetimeType when datetimeType == typeof(DateTime):
                            condicion= GetDateTimeExpression(tableId + p.ObtieneNombreCampo(), 
                                f.Operador, f.Valor, f.Negacion);
                            break;

                        case Type boolType when boolType == typeof(bool):
                            condicion = GetBoolExpression(tableId + p.ObtieneNombreCampo(), 
                                f.Operador, f.Valor, f.Negacion);
                            break;


                        case Type stringType when stringType == typeof(string):
                            condicion = GetStringExpression(tableId + p.ObtieneNombreCampo(),
                                f.Operador, f.Valor, f.Negacion);
                            break;

                        default:
                            break;
                    }

                    if (condicion != null)
                    {
                        condiciones.Add(condicion);
                    }
                }
            }

            return condiciones;
        }

        private static string ReemplazoFecha(this DateTime d)
        {
            return $"CAST('{d.ToString("o")}' AS DATETIME)";

        }

        private static string ObtieneNombreCampo(this PropertyInfo p)
        {
            return p.Name;
        }


    }
}
