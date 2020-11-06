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


        public static ValoresPlantilla Valores(dynamic datos, Plantilla p)
        {
            dynamic d = datos._source;
            ValoresPlantilla v = new ValoresPlantilla
            {
                PlantillaId = p.Id,
                DatoId = d["DatoId"],
                Id = d["Id"],
                IndiceFiltrado = d["IndiceFiltrado"],
                OrigenId = d["OrigenId"],
                TipoDatoId = d["TipoDatoId"],
                TipoOrigenId = d["TipoOrigenId"],
                Valores = new List<ValorPropiedad>()
            };

            foreach(var campo in p.Propiedades)
            {
                var valor = ValorPropiedad(campo, d);
                if (valor != null)
                {
                    v.Valores.Add(valor);
                } else
                {
                    v.Valores.Add(new Modelo.Metadatos.ValorPropiedad() { PropiedadId = campo.Id, Valor = "" });
                }
            }
            return v;
        }

        public static ValorPropiedad ValorPropiedad(this PropiedadPlantilla p, dynamic data)
        {
            try
            {
                switch (p.TipoDatoId)
                {
                    case TipoDato.tBoolean:
                        bool b = (bool)data[p.Id];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = b? "true": "false" };
                        

                    case TipoDato.tTime:
                    case TipoDato.tDateTime:
                    case TipoDato.tDate:
                        DateTime d = (DateTime)data[p.Id];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = d.ToString("o") };

                    case TipoDato.tDouble:
                        float dec = (float)data[p.Id];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = dec.ToString() };


                    case TipoDato.tInt32:
                        int i = (int)data[p.Id];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = i.ToString() };



                    case TipoDato.tInt64:
                        long i64 = (long)data[p.Id];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = i64.ToString() };
                        

                    case TipoDato.tList:
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = (string)data[p.Id] };

                    case TipoDato.tString:
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = (string)data[p.Id] };

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                
            }

            return null;

        }


        #region plantillas elastic

        public static string BuscarId(this string id)
        {
            var q = @$"¡'query': ¡'match': ¡
                              '_id': '{id}'
                        !!!".ToElasticString();

            return q;
        }

        public static string ActualizarDocumento(this string json)
        {
            var q = $"¡'doc': {json}!".ToElasticString();
            return q;
        }

        public static string ActualizarDocumento(this ValoresPlantilla valores, Plantilla plantilla)
        {
            string json = valores.ObtieneJSONValores(plantilla);
            var q = $"¡'doc': {json}!".ToElasticString();
            return q;
        }

        #endregion

    }
}
