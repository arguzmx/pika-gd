using Microsoft.Extensions.Configuration;
using ES=Elasticsearch.Net;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class RepoMetadatosElasticSearch: IRepositorioMetadatos
    {
        private readonly IConfiguration Configuration;
        private ES.ElasticLowLevelClient cliente;
        public RepoMetadatosElasticSearch(IConfiguration configuration)
        {
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

        public Task<Paginado<ValoresPlantilla>> Consulta(Plantilla plantilla, Consulta query)
        {
            throw new NotImplementedException();
        }


        public async Task<string> CrearIndice(Plantilla plantilla)
        {
            var body = ES.PostData.String(plantilla.ObtieneJSONPlantilla());
            var response = await cliente.Indices.CreateAsync<CreateIndexResponse>(plantilla.Id, body);
            return response.Index;
        }

        public Task<bool> Elimina(Plantilla plantilla, string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarIndice(Plantilla plantilla)
        {
            throw new NotImplementedException();
        }

        public Task<string> Inserta(Plantilla plantilla, ValoresPlantilla valores)
        {
            throw new NotImplementedException();
        }

        public Task<ValoresPlantilla> Unico(Plantilla plantilla, string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExisteIndice(string id)
        {
            var response = await cliente.Indices.ExistsAsync<ExistsResponse>(id);
            Console.WriteLine($"{response.ApiCall.Uri} {response.ApiCall.HttpMethod} {response.Exists}");
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
