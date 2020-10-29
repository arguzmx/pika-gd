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

        public async Task<string> CrearIndice(Plantilla plantilla)
        {
            var body = ES.PostData.String(plantilla.ObtieneJSONPlantilla());
            var response = await cliente.LowLevel.Indices.CreateAsync<CreateIndexResponse>(plantilla.Id, body);
            return response.Index;
        }

        public async Task<string> Inserta(Plantilla plantilla, ValoresPlantilla valores)
        {
            string json = valores.ObtieneJSONValores(plantilla);
            var body = ES.PostData.String(json);

            
            var response = await cliente.LowLevel.CreateAsync<CreateResponse>(plantilla.Id, valores.Id, body);


            if (response.Result == Result.Created) return valores.Id;
            return null;

        }


        private async Task  ExiteUnico(string indice, string tipo, string id )
        {
            var r = await cliente.LowLevel.SearchAsync<StringResponse>(indice, PostData.Serializable(new
            {
                from = 0,
                size = 1,
                query = new
                {
                    @bool = new 
                    {
                        must = new object[]
                        {
                            new {
                                term = new
                                {
                                    TipoOrigenId = new
                                    {
                                        query = tipo
                                    }
                                }
                            },
                            new {
                                term = new
                                {
                                    OrigenId = new
                                    {
                                        query = id
                                    }
                                }
                            }
                        }
                        
                    }
                    
                }
            }));

            if (r.Success)
            {
                
            }

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
