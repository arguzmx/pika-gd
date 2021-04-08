using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using ES = Elasticsearch.Net;
using System.Linq;
using System.Threading.Tasks;
using Nest;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public partial class RepoMetadatosElasticSearch
    {

        
        private const string MUSTNOT = "'must_not': [ #Q# ]";
        private const string MUST = "'must': [ #Q# ]";
        private const string FILTER = "'filter': [ #Q# ]";

        private const string MATCH = "{ 'match': { '#F#': { 'query': '#V#', 'fuzziness': #Z# } } }";
        private const string CONTAINS = "{ 'wildcard': { '#F#': { 'value': '*#V#*' } } }";
        private const string STARTS = "{ 'wildcard': { '#F#': { 'value': '*V#*' } } }";
        private const string ENDS = "{ 'wildcard': { '#F#': { 'value': '*#V#' } } }";
        private const string TERM = "{ 'term': { '#F#': {'value': ^#V#^ }}";
        private const string RANGECLOSED = "{ 'range': { '#F#': { 'gte': ^#V#^, 'lte': ^#W#^ } } }";
        private const string RANGEOPEN = "{ 'range': { '#F#': { 'gt': ^#V#^, 'lt': ^#W#^ } } }";
        private const string RANGEGT = "{ 'range': { '#F#': { 'gt': ^#V#^} } }";
        private const string RANGEGTE = "{ 'range': { '#F#': { 'gte': ^#V#^} } }";
        private const string RANGELT = "{ 'range': { '#F#': { 'lt': ^#V#^} } }";
        private const string RANGELTE = "{ 'range': { '#F#': { 'lte': ^#V#^} } }";

        private const string BOOLQUERY = "{ 'query': { 'bool': { %M% #N# #F# } } }";

        public async Task<long> ContarPorConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId)
        {
            string consulta = CreaConsulta(q, plantilla, PuntoMontajeId);
            var body = ES.PostData.String(consulta);
            var response = await cliente.LowLevel.CountAsync<CountResponse>(body);

            if (response.IsValid)
            {
                return response.Count;
            }

            return 0;
            
        }

        private string CreaConsulta(Consulta q, Plantilla plantilla, string PuntoMontajeId)
        {
                
            string must = "";
            string must_not = "";
            string filter = TERM
                        .Replace("#F#", $"IF")
                        .Replace("#V#", PuntoMontajeId); 

            foreach (var f in q.Filtros)
            {
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
                            if (tBoolean != null) {
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

            if (must_not.Length > 0) { 
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
                .Replace("M", must.Length>0 ? must + "," : "" )
                .Replace("N", must_not.Length > 0 ? must_not + "," : "")
                .Replace("F", filter)
                ;

            Console.WriteLine(consulta);

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
                
                if(!decimal.TryParse(ni, out _))
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
                if(ff==null || fi ==null) return (null, false);
                
                if(DateTime.Parse(fi).Ticks > DateTime.Parse(ff).Ticks)
                {
                    string tmp = fi;
                    fi = ff;
                    ff = tmp;
                }

            } else
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
