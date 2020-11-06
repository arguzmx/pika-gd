using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nest;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.ElasticSearch.Excepciones;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public static class ElasticJSONExtender
    {

        public static string ToElasticString(this string s)
        {
            return s.Replace("¡", "{").Replace("!", "}").Replace("'", "\"").Replace("\t", "");
        }

        public static bool ValorValido(this PropiedadPlantilla p, string valor)
        {
            Console.WriteLine($"({p.Nombre})");
            Console.WriteLine($"({valor})");
            if (p.Requerido && string.IsNullOrEmpty(valor)) return false;

            switch (p.TipoDatoId)
            {
                case TipoDato.tBoolean:
                    return (valor == "true" || valor == "false") ? true : false;

                case TipoDato.tTime:
                case TipoDato.tDateTime:
                case TipoDato.tDate:
                    DateTime d;
                    return DateTime.TryParse(valor, out d);

                case TipoDato.tDouble:
                    float dec;
                    if(float.TryParse(valor, out dec))
                    {
                        if (p.ValidadorNumero != null)
                        {
                            if(p.ValidadorNumero.UtilizarMin && dec< p.ValidadorNumero.min) return false;
                            if (p.ValidadorNumero.UtilizarMax && dec > p.ValidadorNumero.max) return false;
                            return true;
                        } else
                        {
                            return true;
                        }
                    }
                    break;

                case TipoDato.tInt32:
                    int i;
                    if (int.TryParse(valor, out i))
                    {
                        if (p.ValidadorNumero != null)
                        {
                            if (p.ValidadorNumero.UtilizarMin && i < p.ValidadorNumero.min) return false;
                            if (p.ValidadorNumero.UtilizarMax && i > p.ValidadorNumero.max) return false;
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    break;
                    

                case TipoDato.tInt64:
                    long i64;
                    if (long.TryParse(valor, out i64))
                    {
                        if (p.ValidadorNumero != null)
                        {
                            if (p.ValidadorNumero.UtilizarMin && i64 < p.ValidadorNumero.min) return false;
                            if (p.ValidadorNumero.UtilizarMax && i64 > p.ValidadorNumero.max) return false;
                            return true;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    break;

                case TipoDato.tList:
                    return true;

                case TipoDato.tString:
                    if (p.ValidadorTexto! != null)
                    {
                        if(!string.IsNullOrEmpty(p.ValidadorTexto.regexp))
                        {
                            Regex rgx = new Regex(p.ValidadorTexto.regexp);
                            return rgx.IsMatch(valor);
                        } else
                        {
                            if (valor.Length < p.ValidadorTexto.longmin) return false;
                            if (valor.Length > p.ValidadorTexto.longmax) return false;
                            return true;
                        }

                    }
                    break;
            }

            return false;
        }
 
        public static string ObtieneJSONValores(this ValoresPlantilla valores, Plantilla plantilla) {
            string json = "{  %C% }";
            string baseProp = "'%N%': %V%,";

            StringBuilder sb = new StringBuilder();

            foreach (var item in plantilla.Propiedades)
            {
                var valor = valores.Valores.Where(x => x.PropiedadId == item.Id).SingleOrDefault();
                if (valor == null) valor = new ValorPropiedad() { PropiedadId = item.Id, Valor = "" };

                if (!ValorValido(item, valor.Valor )) 
                    throw new ExMetadatosNoValidos($"valor no válido para {item.Nombre} {(valor.Valor)}") ;

                string Delimitador = "'";

                switch (item.TipoDatoId)
                {
                    case TipoDato.tDouble:
                    case TipoDato.tInt32:
                    case TipoDato.tInt64:
                    case TipoDato.tBoolean:
                        Delimitador = "";
                        break;
                }

                if (valor!=null) sb.Append(baseProp.Replace("%N%", item.Id).Replace("%V%", $"{Delimitador}{valor.Valor}{Delimitador}"));
            }

            if (sb.Length > 0)
            {
                sb.Append($"'Id': '{valores.Id}',");
                sb.Append($"'OrigenId': '{valores.OrigenId}',");
                sb.Append($"'TipoOrigenId': '{valores.TipoOrigenId}',");
                sb.Append($"'TipoDatoId': '{valores.TipoDatoId}',");
                sb.Append($"'DatoId': '{valores.DatoId}',");
                sb.Append($"'IndiceFiltrado': '{valores.IndiceFiltrado}',");

            }
            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                string r =  json.Replace("%C%", campos).Replace('\'', '\"');
                Console.WriteLine(r);
                return r;
            }

            return "";
        }


        public static string ObtieneJSONPlantilla(this Plantilla plantilla)
        {
            string json = "{ 'mappings': { 'properties': { %C% } }}";
            string baseProp = @"'%N%': {'type': '%T%'},";

            StringBuilder sb = new StringBuilder();
            
            

            foreach(var item in plantilla.Propiedades)
            {
                switch (item.TipoDatoId) {
                    
                    case TipoDato.tBinaryData:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "binary"));
                        break;

                    case TipoDato.tBoolean:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "boolean"));
                        break;

                    case TipoDato.tDouble:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "double"));
                        break;

                    case TipoDato.tInt32:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "integer"));
                        break;

                    case TipoDato.tInt64:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "long"));
                        break;

                    case TipoDato.tList:
                    case TipoDato.tString:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "keyword"));
                        break;

                    case TipoDato.tIndexedString:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "text"));
                        break;

                    case TipoDato.tTime:
                    case TipoDato.tDate:
                    case TipoDato.tDateTime:
                        sb.Append(baseProp.Replace("%N%", item.Id).Replace("%T%", "date"));

                        break;

                }
            }

            if (sb.Length > 0)
            {
                sb.Append(baseProp.Replace("%N%", "OrigenId").Replace("%T%", "keyword"));
                sb.Append(baseProp.Replace("%N%", "TipoOrigenId").Replace("%T%", "keyword"));
                sb.Append(baseProp.Replace("%N%", "TipoDatoId").Replace("%T%", "keyword"));
                sb.Append(baseProp.Replace("%N%", "DatoId").Replace("%T%", "keyword"));
                sb.Append(baseProp.Replace("%N%", "IndiceFiltrado").Replace("%T%", "keyword"));
            }

            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'','\"');
            }

            return "";
        }



    }
}
