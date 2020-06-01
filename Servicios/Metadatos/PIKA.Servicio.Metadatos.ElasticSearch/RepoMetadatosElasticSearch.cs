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

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class RepoMetadatosElasticSearch: IRepositorioMetadatos
    {
        public static DateTime FechaBaseHora => new DateTime(2000, 1, 1, 0, 0, 0);

        private readonly IConfiguration Configuration;
        private ES.ElasticLowLevelClient cliente;
        private ILogger<RepoMetadatosElasticSearch> logger;
        public RepoMetadatosElasticSearch(IConfiguration configuration, ILogger<RepoMetadatosElasticSearch> logger)
        {
            this.logger = logger;
            Configuration = configuration;
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Metadatos").Bind(options);
            var settings = new ES.ConnectionConfiguration(new Uri(options.CadenaConexion()));
            cliente = new ES.ElasticLowLevelClient(settings);
        }

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
            var response = await cliente.SearchAsync<SearchResponse<dynamic>>(body);


            return null;
        }

        public async Task<string> CrearIndice(Plantilla plantilla)
        {
            var body = ES.PostData.String(plantilla.ObtieneJSONPlantilla());
            var response = await cliente.Indices.CreateAsync<CreateIndexResponse>(plantilla.Id, body);
            return response.Index;
        }

        public async Task<string> Inserta(Plantilla plantilla, ValoresPlantilla valores)
        {
            string json = valores.ObtieneJSONValores(plantilla);
            var body = ES.PostData.String(json);
            logger.LogInformation(json);

            var response = await cliente.CreateAsync<CreateResponse>(plantilla.Id, valores.Id, body);

            logger.LogInformation("+ {0} {1}", response.Result, response.Id);

            if (response.Result == Result.Created) return valores.Id;
            return null;

        }


        public async Task<bool> ExisteIndice(string id)
        {
            var response = await cliente.Indices.ExistsAsync<ExistsResponse>(id);
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
