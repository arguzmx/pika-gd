using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Infraestructura.Comun;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                .DefaultIndex(INDICEVERSIONES);
            var settingsContenido = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(INDICECONTENIDO);

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
            logger.LogInformation($"Verificando repositorio");
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
                logger.LogInformation($"Repositorio de nidexados de contenido configurado");
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

        public async Task<bool> ActualizaVersion(string Id, Modelo.Contenido.Version version)
        {

            Console.WriteLine(Id);

            var resultado = await cliente.UpdateAsync<Modelo.Contenido.Version, object>(
                new DocumentPath<Modelo.Contenido.Version>(Id),
               u => u.Index(INDICEVERSIONES)
                   .DocAsUpsert(true)
                   .Doc(version)
                   .Refresh(Elasticsearch.Net.Refresh.True));

            Console.WriteLine(resultado.Result);

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

    }
}
