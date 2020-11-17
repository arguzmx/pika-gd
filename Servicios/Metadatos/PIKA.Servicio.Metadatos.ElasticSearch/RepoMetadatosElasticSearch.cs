using Microsoft.Extensions.Configuration;
using ES=Elasticsearch.Net;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using PIKA.Infraestructura.Comun;
using Microsoft.Extensions.Logging;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Newtonsoft.Json.Linq;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class RepoMetadatosElasticSearch: IRepositorioMetadatos
    {
        public static DateTime FechaBaseHora => new DateTime(2000, 1, 1, 0, 0, 0);

        private readonly IConfiguration Configuration;
        private ElasticClient cliente;
        private ILogger<RepoMetadatosElasticSearch> logger;
        public RepoMetadatosElasticSearch(IConfiguration configuration, ILogger<RepoMetadatosElasticSearch> logger)
        {
            this.logger = logger;
            Configuration = configuration;
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Metadatos").Bind(options);
            var settings = new ConnectionSettings(new Uri(options.CadenaConexion()));
                        
        

            cliente = new ElasticClient(settings);
        }

        public async Task<string> CrearIndice(Plantilla plantilla)
        {

            var body = ES.PostData.String(plantilla.ObtieneJSONPlantilla());
            var response = await cliente.LowLevel.Indices.CreateAsync<CreateIndexResponse>(plantilla.Id, body);
            return response.Index;
        }

        public async Task<string> Inserta(string tipoid, string id, string tipoOrigenId, 
            string origenId, bool esLista, string ListaId,
            Plantilla plantilla, RequestValoresPlantilla valores)
        {

            DocumentoPlantilla valoresplantilla = new DocumentoPlantilla()
            {
                Id = Guid.NewGuid().ToString(),
                DatoId = id,
                TipoDatoId = tipoid,
                PlantillaId = plantilla.Id,
                TipoOrigenId = tipoOrigenId,
                OrigenId = origenId,
                Valores = valores.Valores,
                IndiceFiltrado = valores.Filtro,
                EsLista = esLista,
                ListaId = ListaId
            };

            string json = valoresplantilla.ObtieneJSONValores(plantilla);
            var body = ES.PostData.String(json);
            var response = await cliente.LowLevel.CreateAsync<CreateResponse>(plantilla.Id, valoresplantilla.Id, body);


            if (response.ApiCall.Success) {
                return valoresplantilla.Id;
            } 
            
            return null;
        }

        public async Task<bool> Actualiza(string id, Plantilla plantilla, RequestValoresPlantilla request)
        {
            var doc = await Unico(plantilla, id);

            if (doc !=null )
            {
                DocumentoPlantilla valores = new DocumentoPlantilla()
                {
                    Id = id,
                    Valores = request.Valores,
                    IndiceFiltrado = request.Filtro,
                    PlantillaId = doc.PlantillaId,
                    EsLista = doc.EsLista,
                    ListaId = doc.ListaId,
                    DatoId = doc.DatoId,
                    TipoDatoId = doc.TipoDatoId,
                    TipoOrigenId = doc.TipoOrigenId,
                    OrigenId = doc.OrigenId
                };


                var body = ES.PostData.String(valores.ActualizarDocumento(plantilla));
                var response = await cliente.LowLevel.UpdateAsync<StringResponse>(plantilla.Id, valores.Id, body);
                if (response.ApiCall.Success)
                {
                    return true;
                }     
            }
            return false;
        }

        /// <summary>
        /// Indica la existencia de un Id en el índice
        /// </summary>
        /// <param name="id"></param>
        /// <param name="indice"></param>
        /// <returns></returns>
        private async Task<bool> ExisteId(string id, string indice)
        {
            
            var r = await cliente.LowLevel.SearchAsync<StringResponse>(indice, id.BuscarId());
            if (r.Success)
            {
                ElasticsearchResult esr = JsonSerializer.Deserialize<ElasticsearchResult>(r.Body);
                return esr.hits.total.value > 0 ? true : false;
            }

            return false;

        }

        public async Task<DocumentoPlantilla> Unico(Plantilla plantilla, string id)
        {
            var r = await cliente.LowLevel.SearchAsync<StringResponse>(plantilla.Id, id.BuscarId());
            if (r.Success)
            {

                ElasticsearchResult esr = JsonSerializer.Deserialize<ElasticsearchResult>(r.Body);
                if (esr.hits.total.value > 0)
                {
                    JsonElement e = (JsonElement)esr.hits.hits[0];
                    dynamic d = JObject.Parse(e.ToString());
                    return ElasticJSONExtender.Valores(d, plantilla);
                }
                
            } 
            return null;
        }

        public async Task<List<DocumentoPlantilla>> Lista(Plantilla plantilla, string listaId)
        {
            var r = await cliente.LowLevel.SearchAsync<StringResponse>(plantilla.Id, listaId.BuscarPorLista());
            if (r.Success)
            {
                List<DocumentoPlantilla> resultados = new List<DocumentoPlantilla>();
                ElasticsearchResult esr = JsonSerializer.Deserialize<ElasticsearchResult>(r.Body);
                if (esr.hits.total.value > 0)
                {
                    for(int i=0; i<= esr.hits.total.value; i++)
                    {
                        JsonElement e = (JsonElement)esr.hits.hits[0];
                        dynamic d = JObject.Parse(e.ToString());
                        resultados.Add(ElasticJSONExtender.Valores(d, plantilla));
                    }
                }
                return resultados;
            }
            return null;
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



        public Task<bool> ActualizarIndice(Plantilla plantilla)
        {
            throw new NotImplementedException();
        }

       

        public Task<bool> Elimina(Plantilla plantilla, string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarIndice(Plantilla plantilla)
        {
            throw new NotImplementedException();
        }

  

        public async Task<Paginado<DocumentoPlantilla>> Consulta(Plantilla plantilla, Consulta query)
        {
            string consulta = plantilla.CreaConsulta(query);
            var body = ES.PostData.String(consulta);
            var response = await cliente.LowLevel.SearchAsync<SearchResponse<dynamic>>(body);
            return null;
        }

    

      


        private async Task<bool>  ExisteUnico(string indice, string tipo, string id )
        {
            var x = @$"
                    ¡'size':1,'from':0,
                    'query':¡'bool':¡'must':[
                    ¡'term':¡'OrigenId':¡'value':'{id}'!!!,
                    ¡'term':¡'TipoOrigenId':'{tipo}'!!,
                    ¡'term':¡'Unico':true!!
                    ]!!!
                    ".ToElasticString();

            Console.WriteLine( x);

            var r = await cliente.LowLevel.SearchAsync<StringResponse>(indice, x);

            

            if (r.Success)
            {
                Console.WriteLine(r.Body);
                ElasticsearchResult esr = System.Text.Json.JsonSerializer.Deserialize<ElasticsearchResult>(r.Body);
                return esr.hits.total.value > 0 ? true : false;
            } 

            return false;
        }

        public async Task<bool> ExisteIndice(string id)
        {
            var response = await cliente.LowLevel.Indices.ExistsAsync<ExistsResponse>(id);
            return (response.Exists);
        }


        private async Task validaPlantilla(Plantilla plantilla)
        {
            bool existe = await this.ExisteIndice(plantilla.Id);
            if (!existe)
            {
                await CrearIndice(plantilla);
            }
        }
    }
}
