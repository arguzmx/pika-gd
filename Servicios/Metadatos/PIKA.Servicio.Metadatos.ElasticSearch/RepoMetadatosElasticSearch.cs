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
            //var settings = new ES.ConnectionConfiguration(new Uri(options.CadenaConexion()));
            var settings = new ConnectionSettings(new Uri(options.CadenaConexion()));
                        
        

            cliente = new ElasticClient(settings);
        }

        public async Task<string> CrearIndice(Plantilla plantilla)
        {
            var body = ES.PostData.String(plantilla.ObtieneJSONPlantilla());
            var response = await cliente.LowLevel.Indices.CreateAsync<CreateIndexResponse>(plantilla.Id, body);
            return response.Index;
        }

        public async Task<string> Inserta(Plantilla plantilla, ValoresPlantilla valores)
        {
            valores.Id = Guid.NewGuid().ToString();
            string json = valores.ObtieneJSONValores(plantilla);
            var body = ES.PostData.String(json);
            var response = await cliente.LowLevel.CreateAsync<CreateResponse>(plantilla.Id, valores.Id, body);
            if (response.Result == Result.Created) return valores.Id;
            return null;
        }


        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public Task<bool> Actualiza(Plantilla plantilla, ValoresPlantilla valores)
        {
            throw new NotImplementedException();
        }

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

        public Task<ValoresPlantilla> Unico(Plantilla plantilla, string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Paginado<ValoresPlantilla>> Consulta(Plantilla plantilla, Consulta query)
        {
            string consulta = plantilla.CreaConsulta(query);
            var body = ES.PostData.String(consulta);
            var response = await cliente.LowLevel.SearchAsync<SearchResponse<dynamic>>(body);


            return null;
        }

    

       


        private async Task<bool>  ExisteUnico(string indice, string tipo, string id )
        {
            logger.LogDebug("Uncnip");
            var x = @$"
                    ¡'size':1,'from':0,
                    'query':¡'bool':¡'must':[
                    ¡'term':¡'OrigenId':¡'value':'{id}'!!!,
                    ¡'term':¡'TipoOrigenId':'{tipo}'!!,
                    ¡'term':¡'Unico':true!!
                    ]!!!
                    ".ToJSONString();

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
