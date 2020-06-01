using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nest;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{

    public enum TipoSeccion
    {
        none, filter, must_not
    }

    [Flags]
    public enum TipoError
    {
        ninguno = 0, operador = 1, valor = 2
    }

    public class FiltroElasticSeach
    {

        public FiltroElasticSeach()
        {
            Filtro = "";
            Error = TipoError.ninguno;
            Seccion = TipoSeccion.none;
        }

        public TipoSeccion Seccion { get; set; }
        public string Filtro { get; set; }
        public TipoError Error { get; set; }
    }


    public static class ConsultaES
    {
        /// <summary>
        /// Crea una consulta para elastic serach en base a los paámetros recibidos
        /// </summary>
        /// <param name="plantilla"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string CreaConsulta(this Plantilla plantilla, Consulta query)
        {
            
            List<FiltroElasticSeach> items = new List<FiltroElasticSeach>();
            foreach (var filtro in query.Filtros)
            {
                var propiedad = plantilla.Propiedades.Where(x => x.Id == filtro.Propiedad).SingleOrDefault();
                if (propiedad != null)
                {
                    FiltroElasticSeach item = propiedad.GetQueryItem(filtro);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }

            if (items.Where(x => x.Error != TipoError.ninguno).Any()) {
                return "";
            }

            string consulta = "";
            string consultaFilter = "";
            string consultaMustNot = "";

            foreach (var item in items)
            {
                switch (item.Seccion)
                {
                    case TipoSeccion.filter:
                        consultaFilter += $"{item.Filtro},";
                        break;

                    case TipoSeccion.must_not:
                        consultaMustNot += $"{item.Filtro},";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(consultaFilter))
            {
                consulta += $"'filter': [{consultaFilter.TrimEnd(',')}]";
            }

            if (!string.IsNullOrEmpty(consultaMustNot))
            {
                if (!string.IsNullOrEmpty(consulta)) {
                    consulta += ",";
                }
                consulta += $"'must_not': [{consultaMustNot.TrimEnd(',')}]";
            }

            if (string.IsNullOrEmpty(consulta))
            {
                // la consulta no tien filtros
                consulta = "{ 'query': {'match_all': {} }, [o], [p] }";
                
            }
            else {
                // la consulta es filtrada
                consulta = "{ 'query': { 'bool': { " + consulta + " } }, [o], [p] }";
            }

            string ordenar = @"'sort': [{'" + query.ord_columna + "': {'order': '" + query.ord_direccion + "'}}]";
            string paginar = @" 'size': " + query.tamano.ToString() + ", 'from': " + (query.tamano * query.indice).ToString()  +  "";

            consulta = consulta.Replace("[o]", ordenar);
            consulta = consulta.Replace("[p]", paginar);

            return consulta;
        }

        public static FiltroElasticSeach GetQueryItem(this Propiedad propiedad, FiltroConsulta filtro)
        {
            
            switch (propiedad.TipoDatoId)
            {
                case TipoDato.tBinaryData:
                    //NO hay consulta para binarios
                    break;

                case TipoDato.tBoolean:
                    return  propiedad.QueryItemBoolean(filtro);

                case TipoDato.tDateTime:
                case TipoDato.tDate:
                    return  propiedad.QueryItemDate(filtro);

                case TipoDato.tTime:
                    return propiedad.QueryItemTime(filtro);

                case TipoDato.tDouble:
                    return  propiedad.QueryItemDouble(filtro);

                case TipoDato.tInt32:
                case TipoDato.tInt64:
                    return  propiedad.QueryItemEntero(filtro);

                case TipoDato.tList:
                    return  propiedad.QueryItemLista(filtro);

                case TipoDato.tString:
                    return (propiedad.QueryItemLista(filtro));

            }

            return null;
        
        }


        /// <summary>
        /// Crea un elemento de consulta para valores de tipo lista
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemTexto(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();


            if (string.IsNullOrEmpty(filtro.Valor))
            {
                qitem.Error = TipoError.valor;
            }
            else
            {

                string wildcard = "";

                switch (filtro.Operador)
                {
                    case FiltroConsulta.OP_EQ:
                        qitem.Filtro = "{'term': {'[f]': '[v]'}}"
                        .Replace("[f]", propiedad.Id)
                        .Replace("[v]", filtro.Valor)
                        .Replace("\'", "\"");
                        break;

                    case FiltroConsulta.OP_FULLTEXT:
                        string fuzzy = (filtro.NivelFuzzy == -1) ? "" : 
                            (filtro.NivelFuzzy == 0) ? ", 'fuzziness': 'AUTO'" : 
                            $", 'fuzziness': '{filtro.NivelFuzzy}'";

                        qitem.Filtro = "'match': {'[f]': {'query': '[v]' [z]}} "
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v]", filtro.Valor)
                            .Replace("[z]", fuzzy)
                            .Replace("\'", "\"");
                        break;

                    case FiltroConsulta.OP_CONTAINS:
                        wildcard = "*" + filtro.Valor + "*";
                        break;

                    case FiltroConsulta.OP_ENDS:
                        wildcard = "*" + filtro.Valor;
                        break;

                    case FiltroConsulta.OP_STARTS:
                        wildcard = filtro.Valor + "*";
                        break;

                    default:
                        qitem.Error = TipoError.operador;
                        break;
                }

                if (!string.IsNullOrEmpty(wildcard)){
                    qitem.Filtro = "{'wildcard': {'[f]': {'value': '[v]'}}}"
                        .Replace("[f]", propiedad.Id)
                        .Replace("[v]", wildcard)
                        .Replace("\'", "\"");
                }

                qitem.Seccion = (filtro.Negacion) ?
                            TipoSeccion.must_not :
                            TipoSeccion.filter;


            }

            return qitem;
        }


        /// <summary>
        /// Crea un elemento de consulta para valores de tipo lista
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemLista(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();
            

            if (string.IsNullOrEmpty(filtro.Valor))
            {
                qitem.Error = TipoError.valor;
            }
            else
            {
                switch (filtro.Operador)
                {
                    case FiltroConsulta.OP_EQ:
                        qitem.Seccion = (filtro.Operador == FiltroConsulta.OP_EQ) ?
                                TipoSeccion.filter : TipoSeccion.must_not;

                        qitem.Filtro = "{'term': {'[f]': '[v]'}}"
                        .Replace("[f]", propiedad.Id)
                        .Replace("[v]", filtro.Valor)
                        .Replace("\'", "\"");

                        break;


                    default:
                        qitem.Error = TipoError.operador;
                        break;
                }

                qitem.Seccion = (filtro.Negacion) ?
                           TipoSeccion.must_not :
                           TipoSeccion.filter;


            }

            return qitem;
        }



        /// <summary>
        /// Crea un elemento de consulta para valores de tipo double
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemEntero(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();
            List<long> valores = filtro.Valor.EnteroString();

            if (valores == null)
            {
                qitem.Error = TipoError.valor;
            }
            else
            {
                string filtroComun = "";
                switch (filtro.Operador)
                {
                    case FiltroConsulta.OP_NEQ:
                    case FiltroConsulta.OP_EQ:
                        if (valores.Count != 1)
                        {
                            qitem.Error = TipoError.valor;
                        }
                        else
                        {
                            qitem.Filtro = "{'term': {'[f]': '[v]'}}"
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v]", valores[0].ToString())
                            .Replace("\'", "\"");
                        }
                        break;

                    case FiltroConsulta.OP_BETWEN:
                        if (valores.Count != 2)
                        {
                            qitem.Error = TipoError.valor;
                        }
                        else
                        {
                            qitem.Filtro = "'[f]': {'gte': '[v1]', 'lte': '[v2]'}"
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v1]", valores[0].ToString())
                            .Replace("[v2]", valores[1].ToString())
                            .Replace("\'", "\"");
                        }
                        break;

                    case FiltroConsulta.OP_GT:
                        filtroComun = "gt";
                        break;

                    case FiltroConsulta.OP_GTE:
                        filtroComun = "gte";
                        break;

                    case FiltroConsulta.OP_LT:
                        filtroComun = "lt";
                        break;

                    case FiltroConsulta.OP_LTE:
                        filtroComun = "lte";
                        break;

                    default:
                        qitem.Error = TipoError.operador;
                        break;
                }

                if (!string.IsNullOrEmpty(filtroComun))
                {
                    qitem.Filtro = "'[f]': {'[c]': '[v]'}"
                    .Replace("[c]", filtroComun)
                    .Replace("[f]", propiedad.Id)
                    .Replace("[v]", valores[0].ToString())
                    .Replace("\'", "\"");
                }


                qitem.Seccion = (filtro.Negacion) ?
                            TipoSeccion.must_not :
                            TipoSeccion.filter;


            }

            return qitem;
        }


        /// <summary>
        /// Crea un elemento de consulta para valores de tipo double
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemDouble(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();
            List<double> valores = filtro.Valor.DecimalString();

            if (valores == null)
            {
                qitem.Error = TipoError.valor;
            }
            else
            {
                string filtroComun = "";
                switch (filtro.Operador)
                {
                    case FiltroConsulta.OP_EQ:
                        if (valores.Count != 1)
                        {
                            qitem.Error = TipoError.valor;
                        }
                        else
                        {
                            qitem.Filtro = "{'term': {'[f]': '[v]'}}"
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v]", valores[0].ToString())
                            .Replace("\'", "\"");
                        }
                        break;

                    case FiltroConsulta.OP_BETWEN:
                        if (valores.Count != 2)
                        {
                            qitem.Error = TipoError.valor;
                        }
                        else
                        {
                            qitem.Filtro = "'[f]': {'gte': '[v1]', 'lte': '[v2]'}"
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v1]", valores[0].ToString())
                            .Replace("[v2]", valores[1].ToString())
                            .Replace("\'", "\"");
                        }
                        break;

                    case FiltroConsulta.OP_GT:
                        filtroComun = "gt";
                        break;

                    case FiltroConsulta.OP_GTE:
                        filtroComun = "gte";
                        break;

                    case FiltroConsulta.OP_LT:
                        filtroComun = "lt";
                        break;

                    case FiltroConsulta.OP_LTE:
                        filtroComun = "lte";
                        break;

                    default:
                        qitem.Error = TipoError.operador;
                        break;
                }

                if (!string.IsNullOrEmpty(filtroComun))
                {
                    qitem.Filtro = "'[f]': {'[c]': '[v]'}"
                    .Replace("[c]", filtroComun)
                    .Replace("[f]", propiedad.Id)
                    .Replace("[v]", valores[0].ToString())
                    .Replace("\'", "\"");
                }


                qitem.Seccion = (filtro.Negacion) ?
                            TipoSeccion.must_not :
                            TipoSeccion.filter  ;



            }

            return qitem;
        }




        /// <summary>
        /// Crea un elemento de consulta para valores de tipo Time
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemTime(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();
            List<DateTime> fechas = filtro.Valor.FechaString();

            if (fechas == null)
            {
                qitem.Error = TipoError.valor;
            } else
            {
                // Convierte la fecha para ajustartse al valor base para búsqueda de tiempo
                filtro.Valor = (fechas.Count == 2) ?
                    fechas[0].AFechaBase().ToString("o") + FiltroConsulta.SEPARADOR_VALORES + 
                    fechas[1].AFechaBase().ToString("o") :
                    fechas[0].AFechaBase().ToString("o");
                qitem = propiedad.QueryItemDate(filtro);
            }

            return qitem;
        }


        /// <summary>
        /// Construye un query para los tipos basdos en fecha
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemDate(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();
            List<DateTime> fechas = filtro.Valor.FechaString();

            if (fechas == null)
            {
                qitem.Error = TipoError.valor;
            }
            else
            {
                string filtroComun = "";
                switch (filtro.Operador)
                {
                    case FiltroConsulta.OP_EQ:
                        if (fechas.Count != 1)
                        {
                            qitem.Error = TipoError.valor;
                        }
                        else
                        {
                            qitem.Filtro = "{'term': {'[f]': '[v]'}}"
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v]", fechas[0].ToString("o"))
                            .Replace("\'", "\"");
                        }
                        break;

                    case FiltroConsulta.OP_BETWEN:
                        if (fechas.Count != 2)
                        {
                            qitem.Error = TipoError.valor;
                        }
                        else
                        {
                            qitem.Filtro = "'[f]': {'gte': '[v1]', 'lte': '[v2]'}"
                            .Replace("[f]", propiedad.Id)
                            .Replace("[v1]", fechas[0].ToString("o"))
                            .Replace("[v2]", fechas[1].ToString("o"))
                            .Replace("\'", "\"");
                        }
                        break;

                    case FiltroConsulta.OP_GT:
                        filtroComun = "gt";
                        break;

                    case FiltroConsulta.OP_GTE:
                        filtroComun = "gte";
                        break;

                    case FiltroConsulta.OP_LT:
                        filtroComun = "lt";
                        break;

                    case FiltroConsulta.OP_LTE:
                        filtroComun = "lte";
                        break;

                    default:
                        qitem.Error = TipoError.operador;
                        break;
                }


                if (!string.IsNullOrEmpty(filtroComun))
                {
                    if (fechas.Count != 1)
                    {
                        qitem.Error = TipoError.valor;
                    } else
                    {
                        qitem.Filtro = "'[f]': {'[c]': '[v]'}"
                        .Replace("[c]", filtroComun)
                        .Replace("[f]", propiedad.Id)
                        .Replace("[v]", fechas[0].ToString("o"))
                        .Replace("\'", "\"");
                    }
                }

                if(qitem.Error== TipoError.ninguno)
                    qitem.Seccion = (filtro.Negacion) ?
                        TipoSeccion.must_not :
                        TipoSeccion.filter;

            }

            return qitem;
        }


        /// <summary>
        /// Obtiene el query para un operador boolean
        /// </summary>
        /// <param name="propiedad"></param>
        /// <param name="filtro"></param>
        /// <returns></returns>
        private static FiltroElasticSeach QueryItemBoolean(this Propiedad propiedad, FiltroConsulta filtro)
        {
            FiltroElasticSeach qitem = new FiltroElasticSeach();

            List<string> trueValues = new List<string>() { "true", "1" };
            List<string> falseValues = new List<string>() { "false", "0" };

            if ((trueValues.IndexOf(filtro.Valor.ToLower()) < 0) &&
                        (falseValues.IndexOf(filtro.Valor.ToLower()) < 0))
                qitem.Error = TipoError.valor;


            if (qitem.Error == TipoError.ninguno)
            {
                switch (filtro.Operador)
                {
                    case FiltroConsulta.OP_EQ:

                        qitem.Seccion = (filtro.Negacion) ?
                            TipoSeccion.must_not :
                            TipoSeccion.filter;

                        string valor = (trueValues.IndexOf(filtro.Valor.ToLower()) >= 0) ? "true" : "false";
                        qitem.Filtro = "{'term': {'[f]': '[v]'}}"
                        .Replace("[f]", propiedad.Id)
                        .Replace("[v]", valor)
                        .Replace("\'", "\"");

                        break;

                    default:
                        qitem.Error = TipoError.operador;
                        break;

                }

            }
            return qitem;
        }



        #region Convertidores de tipos


        private static List<long> EnteroString(this string valores)
        {
            List<long> lista = new List<long>();
            List<string> partes = valores.Split(FiltroConsulta.SEPARADOR_VALORES).ToList();
            bool isok = true;
            foreach (string s in partes)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    long valor;
                    if (long.TryParse(s, out valor))
                    {
                        lista.Add(valor);
                    }
                    else
                    {
                        isok = false;
                    }
                }
            }

            if ((lista.Count() == 0) || (isok == false)) return null;

            if (lista.Count == 2)
            {
                if (lista[0] > lista[1])
                {
                    long f = lista[1];
                    lista[1] = lista[0];
                    lista[0] = f;
                }
            }

            return lista;
        }

        private static List<double> DecimalString(this string valores)
        {
            List<double> lista = new List<double>();
            List<string> partes = valores.Split(FiltroConsulta.SEPARADOR_VALORES).ToList();
            bool isok = true;
            foreach (string s in partes)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    double valor;
                    if (double.TryParse(s, out valor))
                    {
                        lista.Add(valor);
                    }
                    else
                    {
                        isok = false;
                    }
                }
            }

            if ((lista.Count() == 0) || (isok == false)) return null;
            if (lista.Count == 2)
            {
                if (lista[0]> lista[1])
                {
                    double f = lista[1];
                    lista[1] = lista[0];
                    lista[0] = f;
                }
            }

            return lista;
        }

        private static List<DateTime> FechaString(this string fechas)
        {
            List<DateTime> lista = new List<DateTime>();
            List<string> partes = fechas.Split(FiltroConsulta.SEPARADOR_VALORES).ToList();
            bool isok = true;
            foreach (string s in partes)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    DateTime fecha;
                    if (DateTime.TryParse(s, out fecha))
                    {
                        lista.Add(fecha);
                    }
                    else
                    {
                        isok = false;
                    }
                }
            }

            if ((lista.Count() == 0) || (isok == false)) return null;

            if (lista.Count == 2)
            {
                if (lista[0].Ticks > lista[1].Ticks)
                {
                    DateTime f = lista[1];
                    lista[1] = lista[0];
                    lista[0] = f;
                }
            }

            return lista;
        }

        private static DateTime AFechaBase(this DateTime fecha)
        {
            DateTime fbase = RepoMetadatosElasticSearch.FechaBaseHora;
            return new DateTime(fbase.Year, fbase.Month, fbase.Day, fecha.Hour, fecha.Minute, fecha.Second, fecha.Millisecond, DateTimeKind.Unspecified);

        }
        #endregion


    }



}
