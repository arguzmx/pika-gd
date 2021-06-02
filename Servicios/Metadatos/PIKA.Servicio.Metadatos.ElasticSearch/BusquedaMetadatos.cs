using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using ES = Elasticsearch.Net;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using System.Collections.Generic;
using Elasticsearch.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public static class Extenders
    {
        public static string DblQuotes(this string o)
        {
            return o.Replace("'", "\"");
        }

        public static void LogS(this string o)
        {
            Console.WriteLine(o);
        }

        public static void LogS(this object o)
        {
            Console.WriteLine(o.ToS());
        }

        public static string ToS(this object o)
        {
            return System.Text.Json.JsonSerializer.Serialize(o);
        }
    }

    public partial class RepoMetadatosElasticSearch
    {


        private const string MUSTNOT = "'must_not': [ #Q# ]";
        private const string MUST = "'must': [ #Q# ]";
        private const string FILTER = "'filter': [ #Q# ]";

        private const string MATCH = "{ 'match': { '#F#': { 'query': '#V#', 'fuzziness': #Z# } } }";
        private const string CONTAINS = "{ 'wildcard': { '#F#': { 'value': '*#V#*' } } }";
        private const string STARTS = "{ 'wildcard': { '#F#': { 'value': '*V#*' } } }";
        private const string ENDS = "{ 'wildcard': { '#F#': { 'value': '*#V#' } } }";
        private const string TERM = "{ 'term': { '#F#': {'value': ^#V#^ } } }";
        private const string RANGECLOSED = "{ 'range': { '#F#': { 'gte': ^#V#^, 'lte': ^#W#^ } } }";
        private const string RANGEOPEN = "{ 'range': { '#F#': { 'gt': ^#V#^, 'lt': ^#W#^ } } }";
        private const string RANGEGT = "{ 'range': { '#F#': { 'gt': ^#V#^} } }";
        private const string RANGEGTE = "{ 'range': { '#F#': { 'gte': ^#V#^} } }";
        private const string RANGELT = "{ 'range': { '#F#': { 'lt': ^#V#^} } }";
        private const string RANGELTE = "{ 'range': { '#F#': { 'lte': ^#V#^} } }";

        private const string BOOLQUERY = "'query': { 'bool': { #M# #N# #F# } }";
        private const string PLAINBOOLQUERY = "{ #Q# }";
        private const string IDSQUERY = "{ 'size': 5000, '_source': ['DID'], #Q# }";
        private const string QUERYMETADATOSPORID = "{ 'query': { 'ids': { 'values': #IDS#  } } }";
        private const string QUERYPAGINAMETADATOSPORID = "{ 'from': #SKIP#, 'size': #SIZE#, 'sort' : [ { '#SFIELD#' : {'order' : '#SORD#'}} ], 'query': { 'bool': { 'filter': [ { 'terms' : { '_id' : [#IDS#] } } ] } } }";
        private const string QUERYPAGINAMETADATOS = "{ 'from': #SKIP#, 'size': #SIZE#, 'sort' : [ { '#SFIELD#' : {'order' : '#SORD#'}} ], #Q# }";
        private const string QUERYPAGINAMETADATOSPORIDLISTADID = "{ 'from': #SKIP#, 'size': #SIZE#, 'sort' : [ { '#SFIELD#' : {'order' : '#SORD#'}} ], 'query': { 'bool': { 'filter': [ { 'terms' : { 'DID' : [#IDS#] } } ] } } }";





        public async Task<long> ContarPorConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId, string IdJerarquico)
        {
            string consulta = PLAINBOOLQUERY.Replace("#Q#", CreaConsulta(q, plantilla, PuntoMontajeId, IdJerarquico)).DblQuotes();

            var body = ES.PostData.String(consulta);
            var response = await cliente.LowLevel.CountAsync<CountResponse>(plantilla.Id, body);

            if (response.IsValid)
            {
                return response.Count;
            }
            return 0;
        }

        public async Task<List<string>> IdsrPorConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId, string IdJerarquico)
        {

            List<string> l = new List<string>();
            string consulta = IDSQUERY.Replace("#Q#", CreaConsulta(q, plantilla, PuntoMontajeId, IdJerarquico)).DblQuotes();

            var body = ES.PostData.String(consulta);

            var response = await cliente.LowLevel.SearchAsync<StringResponse>(plantilla.Id, body);
            if (response.Success)
            {
                dynamic r = JObject.Parse(response.Body);
                var hits = r.hits.hits;
                foreach (var h in hits)
                {
                    l.Add((string)h._source.DID + "|" + (string)h._id);
                }
            }
            return l;

        }

        public async Task<Paginado<DocumentoPlantilla>> PaginadoDocumentoPlantilla(ConsultaAPI q, Plantilla plantilla, string PuntoMontajeId, string IdJerarquico)
        {
            try
            {
                CacheBusqueda b = null;

                string consulta = "";
                if(string.IsNullOrEmpty(q.IdCache))
                {
                    consulta = QUERYPAGINAMETADATOS.Replace("#Q#", CreaConsulta(q, plantilla)).DblQuotes();

                } else
                {
                    b = this.appCache.Get<CacheBusqueda>(q.IdCache);
                    StringBuilder sb = new StringBuilder();
                    if (b == null) b = new CacheBusqueda() { Unicos = new List<string>() };
                    foreach(string s in b.Unicos)
                    {
                        sb.Append($"\"{s}\",");
                    }

                    consulta = QUERYPAGINAMETADATOSPORIDLISTADID.Replace("#IDS#", sb.ToString().TrimEnd(',') ).DblQuotes();
                }

                var p = plantilla.Propiedades.Where(x => x.Id == q.ord_columna).FirstOrDefault();


                consulta = consulta.Replace("#SIZE#", q.tamano.ToString());
                consulta = consulta.Replace("#SKIP#", (q.indice * q.tamano).ToString());

                if (p != null)
                {
                    consulta = consulta.Replace("#SORD#", q.ord_direccion);
                    consulta = consulta.Replace("#SFIELD#", $"P{p.IdNumericoPlantilla}");
                }


                var body = ES.PostData.String(consulta);
                var response = await cliente.LowLevel.SearchAsync<StringResponse>(plantilla.Id, body);

                if (response.Success)
                {
                    List<DocumentoPlantilla> resultados = new List<DocumentoPlantilla>();
                    ElasticsearchResult esr = JsonSerializer.Deserialize<ElasticsearchResult>(response.Body);
                    if (esr.hits.hits.Length > 0)
                    {
                        for (int i = 0; i < esr.hits.hits.Length; i++)
                        {
                            JsonElement e = (JsonElement)esr.hits.hits[i];
                            dynamic d = JObject.Parse(e.ToString());
                            resultados.Add(ElasticJSONExtender.Valores(d, plantilla, true));
                        }
                    }


                    // Si es el pagindo basadoe  IDs debe completarse el conteo si existen elementos adicionales en la lista
                    // hasta completar una pagina
                    if (!string.IsNullOrEmpty(q.IdCache) && esr.hits.hits.Length < q.tamano)
                    {
                        int delta = q.tamano - esr.hits.hits.Length;
                        foreach(string s in b.Unicos)
                        {
                            if(!resultados.Any(x=>x.DatoId == s)){
                                resultados.Add(ElasticJSONExtender.ValoresVacios(s, plantilla));
                                delta--;
                                if(delta == 0) break;
                            }
                        }
                    }

                     return new Paginado<DocumentoPlantilla>()
                    {
                        ConteoFiltrado = 0,
                        ConteoTotal = 0,
                        Desde = 0,
                        Elementos = resultados,
                        Indice = q.indice,
                        Paginas = 0,
                        Tamano = q.tamano
                    };

                } else
                {
                    Console.WriteLine(response.OriginalException.Message);
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<List<ValoresEntidad>> ConsultaMetadatosPorListaIds(Plantilla plantilla, List<string> Ids)
        {

            List<ValoresEntidad> l = new List<ValoresEntidad>();
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            Ids.ForEach(id =>
            {
                sb.Append($"'{id}'");
                sb.Append(",");
            });
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            string consulta = QUERYMETADATOSPORID.Replace("#IDS#", sb.ToString()).DblQuotes();

            var body = ES.PostData.String(consulta);

            var response = await cliente.LowLevel.SearchAsync<StringResponse>(plantilla.Id, body);
            if (response.Success)
            {
                var campos = plantilla.Propiedades.ToList().OrderBy(x => x.IndiceOrdenamiento).ToList();


                dynamic r = JObject.Parse(response.Body);
                var hits = r.hits.hits;
                foreach (var h in hits)
                {
                    ValoresEntidad v = new ValoresEntidad()
                    {
                        Id = (string)h._source.DID
                    };

                    campos.ForEach(c =>
                    {

                        var propiedad = plantilla.Propiedades.Where(x => x.IdNumericoPlantilla == c.IdNumericoPlantilla).First();
                        switch (propiedad.TipoDatoId)
                        {

                            case TipoDato.tList:
                                var etiqueta = propiedad.ValoresLista.Where(x => x.Id == (string)h._source[$"P{c.IdNumericoPlantilla}"]).FirstOrDefault();
                                v.Valores.Add(etiqueta == null ? "" : etiqueta.Texto);
                                break;

                            case TipoDato.tDate:
                                if (h._source[$"P{c.IdNumericoPlantilla}"] != null)
                                {
                                    DateTime fecha = (DateTime)h._source[$"P{c.IdNumericoPlantilla}"];
                                    DateTime fecha2 = new DateTime(fecha.Year, fecha.Month, fecha.Day, 0, 0, 0);
                                    v.Valores.Add(fecha2.ToString("o"));

                                }
                                else
                                {
                                    v.Valores.Add(null);
                                }
                                break;

                            case TipoDato.tDateTime:
                                if (h._source[$"P{c.IdNumericoPlantilla}"] != null)
                                {
                                    v.Valores.Add(((DateTime)h._source[$"P{c.IdNumericoPlantilla}"]).ToString("o"));
                                }
                                else
                                {
                                    v.Valores.Add(null);
                                }
                                break;

                            case TipoDato.tTime:

                                if (h._source[$"P{c.IdNumericoPlantilla}"] != null)
                                {
                                    DateTime hora = (DateTime)h._source[$"P{c.IdNumericoPlantilla}"];
                                    DateTime hora2 = new DateTime(2000, 1, 1, hora.Hour, hora.Minute, hora.Second);
                                    v.Valores.Add(hora2.ToString("o"));
                                }
                                else
                                {
                                    v.Valores.Add(null);
                                }
                                break;

                            default:
                                v.Valores.Add((string)h._source[$"P{c.IdNumericoPlantilla}"]);
                                break;
                        }



                    });

                    l.Add(v);
                }
            }

            return l;

        }

        public async Task<List<ValoresEntidad>> ConsultaPaginaMetadatosPorListaIds(Plantilla plantilla, List<string> Ids, Consulta q)
        {

            List<ValoresEntidad> l = new List<ValoresEntidad>();
            StringBuilder sb = new StringBuilder();
            Ids.ForEach(id =>
            {
                sb.Append($"'{id}'");
                sb.Append(",");
            });
            sb.Remove(sb.Length - 1, 1);

            var p = plantilla.Propiedades.Where(x => x.Id == q.ord_columna).FirstOrDefault();

            string consulta = QUERYPAGINAMETADATOSPORID.Replace("#IDS#", sb.ToString()).DblQuotes();
            consulta = consulta.Replace("#SIZE#", q.tamano.ToString());
            consulta = consulta.Replace("#SKIP#", (q.indice * q.tamano).ToString());

            if (p != null)
            {
                consulta = consulta.Replace("#SORD#", q.ord_direccion);
                consulta = consulta.Replace("#SFIELD#", $"P{p.IdNumericoPlantilla}");
            }

            var body = ES.PostData.String(consulta);

            var response = await cliente.LowLevel.SearchAsync<StringResponse>(plantilla.Id, body);
            if (response.Success)
            {
                var campos = plantilla.Propiedades.ToList().OrderBy(x => x.IndiceOrdenamiento).ToList();


                dynamic r = JObject.Parse(response.Body);
                var hits = r.hits.hits;
                foreach (var h in hits)
                {
                    ValoresEntidad v = new ValoresEntidad()
                    {
                        Id = (string)h._source.DID
                    };

                    campos.ForEach(c =>
                    {

                        var propiedad = plantilla.Propiedades.Where(x => x.IdNumericoPlantilla == c.IdNumericoPlantilla).First();
                        switch (propiedad.TipoDatoId)
                        {

                            case TipoDato.tList:
                                var etiqueta = propiedad.ValoresLista.Where(x => x.Id == (string)h._source[$"P{c.IdNumericoPlantilla}"]).FirstOrDefault();
                                v.Valores.Add(etiqueta == null ? "" : etiqueta.Texto);
                                break;

                            case TipoDato.tDate:
                                if (h._source[$"P{c.IdNumericoPlantilla}"] != null)
                                {
                                    DateTime fecha = (DateTime)h._source[$"P{c.IdNumericoPlantilla}"];
                                    DateTime fecha2 = new DateTime(fecha.Year, fecha.Month, fecha.Day, 0, 0, 0);
                                    v.Valores.Add(fecha2.ToString("o"));

                                }
                                else
                                {
                                    v.Valores.Add(null);
                                }
                                break;

                            case TipoDato.tDateTime:
                                if (h._source[$"P{c.IdNumericoPlantilla}"] != null)
                                {
                                    v.Valores.Add(((DateTime)h._source[$"P{c.IdNumericoPlantilla}"]).ToString("o"));
                                }
                                else
                                {
                                    v.Valores.Add(null);
                                }
                                break;

                            case TipoDato.tTime:

                                if (h._source[$"P{c.IdNumericoPlantilla}"] != null)
                                {
                                    DateTime hora = (DateTime)h._source[$"P{c.IdNumericoPlantilla}"];
                                    DateTime hora2 = new DateTime(2000, 1, 1, hora.Hour, hora.Minute, hora.Second);
                                    v.Valores.Add(hora2.ToString("o"));
                                }
                                else
                                {
                                    v.Valores.Add(null);
                                }
                                break;

                            default:
                                v.Valores.Add((string)h._source[$"P{c.IdNumericoPlantilla}"]);
                                break;
                        }



                    });

                    l.Add(v);
                }
            }

            return l;

        }

        private string CreaConsulta(ConsultaAPI q, Plantilla plantilla)
        {

            string must = "";
            string must_not = "";
            string filter = "";

            foreach (var f in q.Filtros)
            {

                if ("OID,TOID,TDID,DID,IF,IJ,LID".IndexOf(f.Propiedad) >= 0)
                {
                    filter += TERM
                        .Replace("#F#", f.Propiedad)
                        .Replace("#V#", f.ValorString)
                        .Replace("^", "'") + ",";

                }
                else
                {
                    f.Valor = f.ValorString;
                    f.ToS();

                    Propiedad p = plantilla.Propiedades.Where(x => x.Id == f.Propiedad).FirstOrDefault();
                    if (p != null)
                    {
                        switch (p.TipoDatoId)
                        {
                            case TipoDato.tString:
                                var (tString, tStringMustNot) = StringQuery(f, p);
                                if (f.Negacion) tStringMustNot = !tStringMustNot;
                                if (tString != null)
                                {
                                    if (tStringMustNot)
                                    {
                                        must_not += $" {tString} ,";
                                    }
                                    else
                                    {
                                        must += $" {tString} ,";
                                    }
                                }
                                break;


                            case TipoDato.tIndexedString:
                                var (tIndexedString, tIndexedStringMustNot) = IndexedStringQuery(f, p);
                                if (f.Negacion) tIndexedStringMustNot = !tIndexedStringMustNot;
                                if (tIndexedString != null)
                                {
                                    if (tIndexedStringMustNot)
                                    {
                                        must_not += $" {tIndexedString} ,";
                                    }
                                    else
                                    {
                                        must += $" {tIndexedString} ,";
                                    }
                                }
                                break;

                            case TipoDato.tBoolean:
                                var (tBoolean, tBooleanMustNot) = BooleanStringQuery(f, p);
                                if (f.Negacion) tBooleanMustNot = !tBooleanMustNot;
                                if (tBoolean != null)
                                {
                                    if (tBooleanMustNot)
                                    {
                                        must_not += $" {tBoolean} ,";
                                    }
                                    else
                                    {
                                        must += $" {tBoolean} ,";
                                    }
                                }
                                break;


                            case TipoDato.tTime:
                            case TipoDato.tDateTime:
                            case TipoDato.tDate:
                                var (tDate, tDateMustNot) = DateQuery(f, p);
                                if (tDate != null)
                                {
                                    if (f.Negacion) tDateMustNot = !tDateMustNot;
                                    if (tDateMustNot)
                                    {
                                        must_not += $" {tDate} ,";
                                    }
                                    else
                                    {
                                        must += $" {tDate} ,";
                                    }
                                }
                                break;


                            case TipoDato.tDouble:
                            case TipoDato.tInt32:
                            case TipoDato.tInt64:
                                var (tNumber, tNumberMustNot) = NumberQuery(f, p);
                                if (tNumber != null)
                                {
                                    if (f.Negacion) tNumberMustNot = !tNumberMustNot;
                                    if (tNumberMustNot)
                                    {
                                        must_not += $" {tNumber} ,";
                                    }
                                    else
                                    {
                                        must += $" {tNumber} ,";
                                    }
                                }
                                break;

                            case TipoDato.tList:
                                var (tList, tListMustNot) = ListQuery(f, p);
                                if (tList != null)
                                {
                                    if (f.Negacion) tListMustNot = !tListMustNot;
                                    if (tListMustNot)
                                    {
                                        must_not += $" {tList} ,";
                                    }
                                    else
                                    {
                                        must += $" {tList} ,";
                                    }
                                }
                                break;
                        }
                    }
                }


            }


            if (must_not.Length > 0)
            {
                must_not = must_not.TrimEnd(',');
                must_not = MUSTNOT.Replace("#Q#", must_not);
            }

            if (must.Length > 0)
            {
                must = must.TrimEnd(',');
                must = MUST.Replace("#Q#", must);
            }

            if (filter.Length > 0)
            {
                filter = filter.TrimEnd(',');
                filter = FILTER.Replace("#Q#", filter);
            }


            string consulta = BOOLQUERY
                .Replace("#M#", must.Length > 0 ? must + "," : "")
                .Replace("#N#", must_not.Length > 0 ? must_not + "," : "")
                .Replace("#F#", filter)
                ;

            return consulta;
        }



        private string CreaConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId, string IndiceJEarquia)
        {

            string must = "";
            string must_not = "";
            string filter = TERM
                        .Replace("#F#", $"IF")
                        .Replace("#V#", PuntoMontajeId)
                        .Replace("^", "'");

            string FilterJerarquira = "";
            if (!string.IsNullOrEmpty(IndiceJEarquia))
            {
                FilterJerarquira = TERM
                        .Replace("#F#", $"IJ")
                        .Replace("#V#", IndiceJEarquia)
                        .Replace("^", "'");
            }

            foreach (var f in q.Filtros)
            {
                f.Valor = f.ValorString;
                f.ToS();

                Propiedad p = plantilla.Propiedades.Where(x => x.Id == f.Propiedad).FirstOrDefault();
                if (p != null)
                {
                    switch (p.TipoDatoId)
                    {
                        case TipoDato.tString:
                            var (tString, tStringMustNot) = StringQuery(f, p);
                            if (f.Negacion) tStringMustNot = !tStringMustNot;
                            if (tString != null)
                            {
                                if (tStringMustNot)
                                {
                                    must_not += $" {tString} ,";
                                }
                                else
                                {
                                    must += $" {tString} ,";
                                }
                            }
                            break;


                        case TipoDato.tIndexedString:
                            var (tIndexedString, tIndexedStringMustNot) = IndexedStringQuery(f, p);
                            if (f.Negacion) tIndexedStringMustNot = !tIndexedStringMustNot;
                            if (tIndexedString != null)
                            {
                                if (tIndexedStringMustNot)
                                {
                                    must_not += $" {tIndexedString} ,";
                                }
                                else
                                {
                                    must += $" {tIndexedString} ,";
                                }
                            }
                            break;

                        case TipoDato.tBoolean:
                            var (tBoolean, tBooleanMustNot) = BooleanStringQuery(f, p);
                            if (f.Negacion) tBooleanMustNot = !tBooleanMustNot;
                            if (tBoolean != null)
                            {
                                if (tBooleanMustNot)
                                {
                                    must_not += $" {tBoolean} ,";
                                }
                                else
                                {
                                    must += $" {tBoolean} ,";
                                }
                            }
                            break;


                        case TipoDato.tTime:
                        case TipoDato.tDateTime:
                        case TipoDato.tDate:
                            var (tDate, tDateMustNot) = DateQuery(f, p);
                            if (tDate != null)
                            {
                                if (f.Negacion) tDateMustNot = !tDateMustNot;
                                if (tDateMustNot)
                                {
                                    must_not += $" {tDate} ,";
                                }
                                else
                                {
                                    must += $" {tDate} ,";
                                }
                            }
                            break;


                        case TipoDato.tDouble:
                        case TipoDato.tInt32:
                        case TipoDato.tInt64:
                            var (tNumber, tNumberMustNot) = NumberQuery(f, p);
                            if (tNumber != null)
                            {
                                if (f.Negacion) tNumberMustNot = !tNumberMustNot;
                                if (tNumberMustNot)
                                {
                                    must_not += $" {tNumber} ,";
                                }
                                else
                                {
                                    must += $" {tNumber} ,";
                                }
                            }
                            break;

                        case TipoDato.tList:
                            var (tList, tListMustNot) = ListQuery(f, p);
                            if (tList != null)
                            {
                                if (f.Negacion) tListMustNot = !tListMustNot;
                                if (tListMustNot)
                                {
                                    must_not += $" {tList} ,";
                                }
                                else
                                {
                                    must += $" {tList} ,";
                                }
                            }
                            break;
                    }
                }
            }


            if (must_not.Length > 0)
            {
                must_not = must_not.TrimEnd(',');
                must_not = MUSTNOT.Replace("#Q#", must_not);
            }

            if (must.Length > 0)
            {
                must = must.TrimEnd(',');
                must = MUST.Replace("#Q#", must);
            }

            if (filter.Length > 0)
            {
                filter = string.IsNullOrEmpty(FilterJerarquira) ? filter : $"{filter},{FilterJerarquira}";
                filter = filter.TrimEnd(',');
                filter = FILTER.Replace("#Q#", filter);
            }


            string consulta = BOOLQUERY
                .Replace("#M#", must.Length > 0 ? must + "," : "")
                .Replace("#N#", must_not.Length > 0 ? must_not + "," : "")
                .Replace("#F#", filter)
                ;

            return consulta;
        }




        private string FechaISO(string fecha, string tipo)
        {
            if (DateTime.TryParse(fecha, out DateTime f))
            {
                switch (tipo)
                {
                    case TipoDato.tDate:
                        f = new DateTime(f.Year, f.Month, f.Day, 0, 0, 0);
                        break;

                    case TipoDato.tTime:
                        f = new DateTime(2000, 1, 1, f.Hour, f.Minute, f.Second);
                        break;
                }

                return f.ToString("o");
            }
            return null;
        }

        private (string query, bool MustNot) NumberQuery(FiltroConsulta f, Propiedad p)
        {
            string query = null;
            bool mustNot = false;

            string ni;
            string nf = "";


            if (f.Operador == FiltroConsulta.OP_BETWEN)
            {
                ni = f.Valor.Split(',')[0];
                nf = f.Valor.Split(',')[1];

                if (!decimal.TryParse(ni, out _))
                {
                    return (null, false);
                }

                if (!decimal.TryParse(nf, out _))
                {
                    return (null, false);
                }

                if (decimal.Parse(ni) > decimal.Parse(nf))
                {
                    string tmp = ni;
                    ni = nf;
                    nf = tmp;
                }

            }
            else
            {
                ni = f.Valor;
                if (!decimal.TryParse(ni, out _))
                {
                    return (null, false);
                }

            }

            switch (f.Operador)
            {

                case FiltroConsulta.OP_NEQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni);
                    mustNot = true;
                    break;

                case FiltroConsulta.OP_EQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni);
                    break;

                case FiltroConsulta.OP_GT:
                    query = RANGEGT
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni);
                    break;

                case FiltroConsulta.OP_GTE:
                    query = RANGEGTE
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni);
                    break;

                case FiltroConsulta.OP_LT:
                    query = RANGELT
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni);
                    break;

                case FiltroConsulta.OP_LTE:
                    query = RANGELTE
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni);
                    break;


                case FiltroConsulta.OP_BETWEN:
                    query = RANGECLOSED
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", ni)
                        .Replace("#W#", nf);
                    break;

            }

            if (query != null) query = query.Replace("^", "");
            return (query, mustNot);
        }




        private (string query, bool MustNot) DateQuery(FiltroConsulta f, Propiedad p)
        {
            string query = null;
            bool mustNot = false;

            string fi = "";
            string ff = "";


            if (f.Operador == FiltroConsulta.OP_BETWEN)
            {
                fi = f.Valor.Split(',')[0];
                ff = f.Valor.Split(',')[1];
                fi = FechaISO(fi, p.TipoDatoId);
                ff = FechaISO(ff, p.TipoDatoId);
                if (ff == null || fi == null) return (null, false);

                if (DateTime.Parse(fi).Ticks > DateTime.Parse(ff).Ticks)
                {
                    string tmp = fi;
                    fi = ff;
                    ff = tmp;
                }

            }
            else
            {
                fi = FechaISO(f.Valor, p.TipoDatoId);
                if (fi == null) return (null, false);
            }

            switch (f.Operador)
            {

                case FiltroConsulta.OP_NEQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi);
                    mustNot = true;
                    break;

                case FiltroConsulta.OP_EQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi);
                    break;

                case FiltroConsulta.OP_GT:
                    query = RANGEGT
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi);
                    break;

                case FiltroConsulta.OP_GTE:
                    query = RANGEGTE
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi);
                    break;

                case FiltroConsulta.OP_LT:
                    query = RANGELT
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi);
                    break;

                case FiltroConsulta.OP_LTE:
                    query = RANGELTE
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi);
                    break;


                case FiltroConsulta.OP_BETWEN:
                    query = RANGECLOSED
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", fi)
                        .Replace("#W#", ff);
                    break;

            }

            if (query != null) query = query.Replace("^", "'");
            return (query, mustNot);
        }

        private (string query, bool MustNot) ListQuery(FiltroConsulta f, Propiedad p)
        {
            string query = null;
            bool mustNot = false;

            switch (f.Operador)
            {

                case FiltroConsulta.OP_NEQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    mustNot = true;
                    break;

                case FiltroConsulta.OP_EQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    break;
            }

            if (query != null) query = query.Replace("^", "'");
            return (query, mustNot);
        }

        private (string query, bool MustNot) BooleanStringQuery(FiltroConsulta f, Propiedad p)
        {
            string query = null;
            bool mustNot = false;
            f.Valor = "1,true,t".Split(',').ToList().IndexOf(f.Valor.ToLower()) >= 0 ? "true" : "false";

            switch (f.Operador)
            {

                case FiltroConsulta.OP_NEQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    mustNot = true;
                    break;

                case FiltroConsulta.OP_EQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    break;
            }

            if (query != null) query = query.Replace("^", "");
            return (query, mustNot);
        }

        private (string query, bool MustNot) IndexedStringQuery(FiltroConsulta f, Propiedad p)
        {
            bool mustNot = false;
            string query = MATCH
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor)
                        .Replace("#Z#", f.NivelFuzzy.ToString());

            if (query != null) query = query.Replace("^", "'");
            return (query, mustNot);
        }


        private (string query, bool MustNot) StringQuery(FiltroConsulta f, Propiedad p)
        {
            string query = null;
            bool mustNot = false;
            switch (f.Operador)
            {

                case FiltroConsulta.OP_NEQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    mustNot = true;
                    break;

                case FiltroConsulta.OP_EQ:
                    query = TERM
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    break;

                case FiltroConsulta.OP_CONTAINS:
                    query = CONTAINS
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    break;

                case FiltroConsulta.OP_STARTS:
                    query = STARTS
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    break;

                case FiltroConsulta.OP_ENDS:
                    query = ENDS
                        .Replace("#F#", $"P{p.IdNumericoPlantilla}")
                        .Replace("#V#", f.Valor);
                    break;

            }

            if (query != null) query = query.Replace("^", "'");
            return (query, mustNot);
        }

    }
}
