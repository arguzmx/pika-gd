using Nest;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.ElasticSearch.Excepciones;
using System;
using System.Collections.Generic;
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

    

        #region creador de plantillas


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

                case TipoDato.tIndexedString:
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

                    } else
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
 
        public static string ObtieneJSONValores(this DocumentoPlantilla valores, Plantilla plantilla) {
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

                if (valor!=null) sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%V%", $"{Delimitador}{valor.Valor}{Delimitador}"));
            }

            if (sb.Length > 0)
            {
                sb.Append($"'ID': '{valores.Id}',");
                sb.Append($"'OID': '{valores.OrigenId}',");
                sb.Append($"'TOID': '{valores.TipoOrigenId}',");
                sb.Append($"'TDID': '{valores.TipoDatoId}',");
                sb.Append($"'DID': '{valores.DatoId}',");
                sb.Append($"'IF': '{valores.IndiceFiltrado}',");
                sb.Append($"'IJ': '{valores.IndiceJerarquia}',");
                sb.Append($"'L': {(valores.EsLista ? "true": "false")},");
                sb.Append($"'LID': '{ (string.IsNullOrEmpty(valores.ListaId) ? "" : valores.ListaId)}',");

            }
            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                string r =  json.Replace("%C%", campos).Replace('\'', '\"');
                return r;
            }

            return "";
        }

        public static string ObtieneJSONActualizarPlantilla(this List<PropiedadPlantilla> props) {
            string json = "{ 'properties': { %C% } }";
            string baseProp = @"'%N%': {'type': '%T%'},";
            string analizerProp = @"'%N%': {'type': '%T%'}, 'analyzer': '%A%'";

            StringBuilder sb = new StringBuilder();

            foreach (var item in props)
            {
                switch (item.TipoDatoId)
                {

                    case TipoDato.tBinaryData:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "binary"));
                        break;

                    case TipoDato.tBoolean:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "boolean"));
                        break;

                    case TipoDato.tDouble:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "double"));
                        break;

                    case TipoDato.tInt32:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "integer"));
                        break;

                    case TipoDato.tInt64:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "long"));
                        break;

                    case TipoDato.tList:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "keyword"));
                        break;

                    case TipoDato.tString:
                        sb.Append(analizerProp.Replace("%N%", $"P{item.IdNumericoPlantilla}")
                            .Replace("%T%", "wildcard")
                            .Replace("%A%", "keyword"));
                        break;

                    case TipoDato.tIndexedString:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "text"));
                        break;

                    case TipoDato.tTime:
                    case TipoDato.tDate:
                    case TipoDato.tDateTime:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "date"));

                        break;

                }
            }

            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'', '\"');
            }

            return "";

        }

        public static string ObtieneJSONActualizacionPlantilla(this Plantilla plantilla, List<int> indices)
        {
            string json = "{ 'properties': { %C% } }";
            string baseProp = @"'%N%': {'type': '%T%'},";

            StringBuilder sb = new StringBuilder();

            foreach(int id in indices)
            {
                var item = plantilla.Propiedades.Where(x => x.IdNumericoPlantilla == id).FirstOrDefault();
                if (item != null)
                {
                    switch (item.TipoDatoId)
                    {

                        case TipoDato.tBinaryData:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "binary"));
                            break;

                        case TipoDato.tBoolean:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "boolean"));
                            break;

                        case TipoDato.tDouble:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "double"));
                            break;

                        case TipoDato.tInt32:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "integer"));
                            break;

                        case TipoDato.tInt64:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "long"));
                            break;

                        case TipoDato.tList:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "keyword"));
                            break;

                        case TipoDato.tString:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "wildcard"));

                            break;

                        case TipoDato.tIndexedString:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "text"));
                            break;

                        case TipoDato.tTime:
                        case TipoDato.tDate:
                        case TipoDato.tDateTime:
                            sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "date"));

                            break;

                    }
                }
            }




            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                Console.WriteLine(json.Replace("%C%", campos).Replace('\'', '\"'));
                return json.Replace("%C%", campos).Replace('\'', '\"');
            }

            return "";
        }


        public static string ObtieneJSONPlantilla(this Plantilla plantilla)
        {
            string json = "{ 'mappings': { 'properties': { %C% } }}";
            string baseProp = @"'%N%': {'type': '%T%'},";

            StringBuilder sb = new StringBuilder();

            // Propiedades fija de los metadtaos
            sb.Append(baseProp.Replace("%N%", "OID").Replace("%T%", "keyword"));
            sb.Append(baseProp.Replace("%N%", "TOID").Replace("%T%", "keyword"));
            sb.Append(baseProp.Replace("%N%", "TDID").Replace("%T%", "keyword"));
            sb.Append(baseProp.Replace("%N%", "DID").Replace("%T%", "keyword"));
            sb.Append(baseProp.Replace("%N%", "IF").Replace("%T%", "keyword"));
            sb.Append(baseProp.Replace("%N%", "IJ").Replace("%T%", "keyword"));
            sb.Append(baseProp.Replace("%N%", "L").Replace("%T%", "boolean"));
            sb.Append(baseProp.Replace("%N%", "LID").Replace("%T%", "keyword"));

            
            foreach (var item in plantilla.Propiedades)
            {
                switch (item.TipoDatoId) {
                    
                    case TipoDato.tBinaryData:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "binary"));
                        break;

                    case TipoDato.tBoolean:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "boolean"));
                        break;

                    case TipoDato.tDouble:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "double"));
                        break;

                    case TipoDato.tInt32:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "integer"));
                        break;

                    case TipoDato.tInt64:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "long"));
                        break;

                    case TipoDato.tList:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "keyword"));
                        break;

                    case TipoDato.tString:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "wildcard"));
                        
                        break;

                    case TipoDato.tIndexedString:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "text"));
                        break;

                    case TipoDato.tTime:
                    case TipoDato.tDate:
                    case TipoDato.tDateTime:
                        sb.Append(baseProp.Replace("%N%", $"P{item.IdNumericoPlantilla}").Replace("%T%", "date"));

                        break;

                }
            }

 
            string campos = sb.ToString().TrimEnd(',');

            if (campos != "")
            {
                return json.Replace("%C%", campos).Replace('\'','\"');
            }

            return "";
        }

        public static DocumentoPlantilla ValoresVacios(string Id, Plantilla p)
        {
            
            DocumentoPlantilla v = new DocumentoPlantilla
            {
                DatoId = Id,
                Id = Id,
                IndiceFiltrado = "",
                IndiceJerarquia = "",
                OrigenId = "",
                TipoDatoId = "",
                TipoOrigenId = "",
                Valores = new List<ValorPropiedad>()
            };

            if (p != null)
            {
                v.PlantillaId = p.Id;
                foreach (var campo in p.Propiedades)
                {
                    v.Valores.Add(new ValorPropiedad() { PropiedadId = campo.Id, Valor = "" });
                }
            }

            return v;
        }


        public static DocumentoPlantilla Valores(dynamic datos, Plantilla p, bool reemplazarIdsLista)
        {
            dynamic d = datos._source;
            DocumentoPlantilla v = new DocumentoPlantilla
            {
                DatoId = d["DID"],
                Id = d["ID"],
                IndiceFiltrado = d["IF"],
                IndiceJerarquia = d["IJ"],
                OrigenId = d["OID"],
                TipoDatoId = d["TDID"],
                TipoOrigenId = d["TOID"],
                Valores = new List<ValorPropiedad>()
            };

            if (p != null)
            {
                v.PlantillaId = p.Id;
                foreach (var campo in p.Propiedades)
                {
                    var valor = ValorPropiedad(campo, d);
                    if (valor != null)
                    {
                        if(campo.TipoDatoId == TipoDato.tList && reemplazarIdsLista) {
                            
                           var id = (string)d[$"P{campo.IdNumericoPlantilla}"];

                           var elemento = campo.ValoresLista.Where(x => x.Id == id).FirstOrDefault();
                            if (elemento != null)
                            {
                                valor = new ValorPropiedad() { PropiedadId = campo.Id, Valor = string.IsNullOrEmpty(elemento.Texto) ? "" : elemento.Texto };
                            }
                            v.Valores.Add(valor);
                        } else
                        {
                            v.Valores.Add(valor);
                        }
                    }
                    else
                    {
                        v.Valores.Add(new ValorPropiedad() { PropiedadId = campo.Id, Valor = "" });
                    }
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
                        bool b = (bool)data[$"P{p.IdNumericoPlantilla}"];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = b? "true": "false" };
                        

                    case TipoDato.tTime:
                    case TipoDato.tDateTime:
                    case TipoDato.tDate:
                        DateTime d = (DateTime)data[$"P{p.IdNumericoPlantilla}"];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = d.ToString("o") };

                    case TipoDato.tDouble:
                        float dec = (float)data[$"P{p.IdNumericoPlantilla}"];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = dec.ToString() };


                    case TipoDato.tInt32:
                        int i = (int)data[$"P{p.IdNumericoPlantilla}"];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = i.ToString() };



                    case TipoDato.tInt64:
                        long i64 = (long)data[$"P{p.IdNumericoPlantilla}"];
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = i64.ToString() };
                        

                    case TipoDato.tList:
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = string.IsNullOrEmpty((string)data[$"P{p.IdNumericoPlantilla}"])  ? "" : (string)data[$"P{p.IdNumericoPlantilla}"] };

                    case TipoDato.tIndexedString:
                    case TipoDato.tString:
                        return new ValorPropiedad() { PropiedadId = p.Id, Valor = string.IsNullOrEmpty((string)data[$"P{p.IdNumericoPlantilla}"]) ? "" :  (string)data[$"P{p.IdNumericoPlantilla}"] };

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                
            }

            return null;

        }

        #endregion

        #region plantillas elastic


        public static string BuscarPorLista(this string listaId)
        {
            var q = @$"¡'size': 5000,'query':¡ 'term': ¡ 'LID': ¡ 'value': '{ listaId }', 'boost': 1.0!!!!".ToElasticString();
            return q;
        }

        public static string BuscarPorTipoIds(this List<string> ids, string tipo)
        {
            StringBuilder sb = new StringBuilder();
            ids.ForEach(id =>
            {
                sb.Append($"\"{id}\",");
            });

            var q = $@"¡ 'query': ¡ 'bool': ¡ 'filter': [ ¡ 'term': ¡ 'TDID': '{tipo}' !!, ¡ 'terms': ¡ 'DID': [ {sb.ToString().TrimEnd(',')} ] ! ! ] ! ! !".ToElasticString(); ;

            return q;
        }


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

        public static string ActualizarDocumento(this DocumentoPlantilla valores, Plantilla plantilla)
        {
            string json = valores.ObtieneJSONValores(plantilla);
            var q = $"¡'doc': {json}!".ToElasticString();
            return q;
        }

        #endregion

    }
}
