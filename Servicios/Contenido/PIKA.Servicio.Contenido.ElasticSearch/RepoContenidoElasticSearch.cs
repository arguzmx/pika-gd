using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Contenido.ElasticSearch.modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ES = Elasticsearch.Net;

namespace PIKA.Servicio.Contenido.ElasticSearch
{
    public partial class RepoContenidoElasticSearch : IRepoContenidoElasticSearch
    {
        private IConfiguration Configuration;
        private ElasticClient cliente;
        private ElasticClient clienteOCR;
        private const string INDICEVERSIONES = "contenido-versiones";
        private const string INDICECONTENIDO = "contenido-indexado";
        private ILogger logger;

        public RepoContenidoElasticSearch(IConfiguration Configuration,
            ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(nameof(RepoContenidoElasticSearch));
            this.Configuration = Configuration;
            this.CreaClientes(this.Configuration);

        }


        public void CreaClientes(IConfiguration configuration)
        {
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Contenido").Bind(options);

            var settingsVersiones = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(INDICEVERSIONES).DisableDirectStreaming(); 
            var settingsContenido = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(INDICECONTENIDO).DisableDirectStreaming(); 

            cliente = new ElasticClient(settingsVersiones);
            clienteOCR = new ElasticClient(settingsContenido);
        }


        public async Task<string> CreaVersion(Modelo.Contenido.Version version)
        {
            var respuesta = await this.cliente.IndexDocumentAsync(version);
            if (!respuesta.IsValid)
            {
                logger.LogError(respuesta.ServerError.ToString());
                return null;
            }
            return version.Id;
        }

        public async Task<bool> CreaRepositorio()
        {
            logger.LogInformation($"Verificando repositorio ...");
            if (!await ExisteIndice(INDICEVERSIONES))
            {
                logger.LogInformation($"Creando repositorio de contenido");
                var createIndexResponse = await cliente.Indices.CreateAsync(INDICEVERSIONES, c => c
                    .Map<Modelo.Contenido.Version>(m => m.AutoMap())
                );

                if (!createIndexResponse.ApiCall.Success)
                {
                    logger.LogError(createIndexResponse.DebugInformation);
                    logger.LogError(createIndexResponse.OriginalException.ToString());
                }
            }
            else
            {

                logger.LogInformation("Actualizando repositorio contenido");
                var body = ES.PostData.String("{\"properties\": {\"partes\": {\"properties\": {\"xid\": {\"type\":\"keyword\" }}}}}");
                var r = cliente.LowLevel.Indices.PutMapping<PutMappingResponse>(INDICEVERSIONES, body);

                body = ES.PostData.String("{\"properties\": {\"ocr_id\": {\"type\":\"keyword\" }}}");
                r = cliente.LowLevel.Indices.PutMapping<PutMappingResponse>(INDICEVERSIONES, body);

                body = ES.PostData.String("{\"properties\": {\"ocr_f\": {\"type\":\"date\" }}}");
                r = cliente.LowLevel.Indices.PutMapping<PutMappingResponse>(INDICEVERSIONES, body);

                Console.WriteLine($"{r.Acknowledged}");

                logger.LogInformation($"Repositorio de contenido configurado");
            }

            if (!await ExisteIndice(INDICECONTENIDO))
            {
                logger.LogInformation($"Creando repositorio de indexado de contenido");
                var createIndexResponse = await cliente.Indices.CreateAsync(INDICECONTENIDO, c => c
                    .Map<Modelo.Contenido.ContenidoTextoCompleto>(m => m.AutoMap())
                );

                if (!createIndexResponse.ApiCall.Success)
                {
                    logger.LogError(createIndexResponse.DebugInformation);
                    logger.LogError(createIndexResponse.OriginalException.ToString());
                }
            }
            else
            {
                logger.LogInformation($"Repositorio de indexados de contenido configurado");
            }


            return true;
        }

        public async Task<bool> ExisteIndice(string id)
        {
            var response = await cliente.LowLevel.Indices.ExistsAsync<ExistsResponse>(id);
            return (response.Exists);
        }

        public async Task<Modelo.Contenido.Version> ObtieneVersion(string Id)
        {
            var result = await this.cliente.GetAsync<Modelo.Contenido.Version>(new GetRequest<Modelo.Contenido.Version>(Id));
            if (result.Found)
            {
                return result.Source;
            }
            return null;
        }

        public async Task<bool> ActualizaVersion(string Id, Modelo.Contenido.Version version, bool MarcarPorIndexar = true)
        {

            if(MarcarPorIndexar)
            {
                version.EstadoIndexado = Modelo.Contenido.EstadoIndexado.PorIndexar;
            }
            
            var resultado = await cliente.UpdateAsync<Modelo.Contenido.Version, object>(
                new DocumentPath<Modelo.Contenido.Version>(Id),
               u => u.Index(INDICEVERSIONES)
                   .DocAsUpsert(true)
                   .Doc(version)
                   .Refresh(Elasticsearch.Net.Refresh.True));

            if (resultado.Result == Result.Updated ||
                resultado.Result == Result.Noop)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> EliminaVersion(string Id)
        {
            var resultado = await cliente.DeleteAsync<Modelo.Contenido.Version>(
                new DocumentPath<Modelo.Contenido.Version>(Id));

            if (resultado.Result == Result.Deleted)
            {
                return true;
            }

            return false;
        }

        public Task<EstadisticaVolumen> ObtieneEstadisticasVolumen(string VolId)
        {
            EstadisticaVolumen estadisticaVolumen = new EstadisticaVolumen() { Id = VolId };

            

            Task<CountResponse> conteo = cliente.CountAsync<Modelo.Contenido.Version>(c => c
                                            .Index(INDICEVERSIONES)
                                         .Query(
                    q => q
                    .Bool(bq => bq
                      .Filter(f => f
                          .Term(t=> t 
                            .Field("vol_id.keyword")
                            .Value(VolId)
                          )
                      )
                      .Must(mq => mq
                       .Term(t => t
                            .Field("act")
                            .Value(true)
                          )
                      )
                    )
                 )
            );

            Task<ISearchResponse<Modelo.Contenido.Version>> cantidades = cliente.SearchAsync<Modelo.Contenido.Version>(c => c
                                            .Index(INDICEVERSIONES)
                .Aggregations(aggs => aggs
                     .Stats("totales", st => st
                       .Field("partes.l")
                     )
                )
                .Query(
                    q => q
                    .Bool(bq => bq
                      .Filter(f => f
                          .Term(t => t
                            .Field("vol_id.keyword")
                            .Value(VolId)
                          )
                      )
                      .Must(mq => mq
                       .Term(t => t
                            .Field("act")
                            .Value(true)
                          )
                      )
                    )
                 )

          );
            
            List<Task> tareas = new List<Task>() { conteo, cantidades };

            Task.WaitAll(tareas.ToArray());
            bool falla = false;

            if (conteo.Result.IsValid)
            {
                estadisticaVolumen.ConteoElementos = conteo.Result.Count;
            }  else
            {
                falla = true;
            }

            if (cantidades.Result.IsValid)
            {
                var commitStats = cantidades.Result.Aggregations.Stats("totales");
                estadisticaVolumen.ConteoPartes = (long)(commitStats.Count);
                estadisticaVolumen.TamanoBytes = (long)(commitStats.Sum); ;
            } else
            {
                falla = true;
            }


            return falla ? null : Task.FromResult(estadisticaVolumen);
        }

        public async Task<bool> EstadoVersion(string Id, bool Activa)
        {
            string script = $"ctx._source.act = {(Activa? "true": "false")}";

            var resultado = await cliente.UpdateByQueryAsync<Modelo.Contenido.Version>(u => u
                   .Query(q => q
                       .Term(f=> f
                        .Name("id.keyword")
                        .Value(Id)
                       )
                   )
                   .Script(script)
                   .Conflicts(Conflicts.Proceed)
                   .Refresh(true)
               );

            if(!resultado.IsValid)
            {
                Console.WriteLine(resultado.ServerError.ToString());
            }

            return resultado.IsValid;
        }

    }
}
