using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.ServicioBusqueda.Contenido
{
    public partial class ServicioBusquedaContenido : IServicioBusquedaContenido
    {
        private IConfiguration Configuration;
        private ElasticClient cliente;
        private const string INDICEBUSQUEDA = "contenido-busqueda";
        private ILogger logger;
        private HashSet<Elemento> elementos;
        /// <summary>
        /// Lista los identificadores del filtro 'en folder'
        /// </summary>
        private List<string> IdsEnfolder;

        /// <summary>
        /// Lista los identificadores del filtro 'propiedades'
        /// </summary>
        private List<string> IdsPropiedades;

        /// <summary>
        /// Lista los identificadores del filtro 'metadatos'
        /// </summary>
        private List<string> IdsMetadatos;


        private UnidadDeTrabajo<DbContextBusquedaContenido> UDT;
        protected DbContextBusquedaContenido contexto;

        public ServicioBusquedaContenido(
            IProveedorOpcionesContexto<DbContextBusquedaContenido> proveedorOpciones,
            IConfiguration Configuration,
            ILoggerFactory loggerFactory)
        {
            DbContextContenidoFactory cf = new DbContextContenidoFactory(proveedorOpciones);
            this.contexto = cf.Crear();

            this.logger = loggerFactory.CreateLogger(nameof(ServicioBusquedaContenido));
            this.Configuration = Configuration;
            this.CreaClientes(this.Configuration);
            this.UDT = new UnidadDeTrabajo<DbContextBusquedaContenido>(contexto);
        }

        public void CreaClientes(IConfiguration configuration)
        {
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("BusquedaContenido").Bind(options);

            var settingsVersiones = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(INDICEBUSQUEDA);

            cliente = new ElasticClient(settingsVersiones);
        }



        public async Task<Paginado<Elemento>> Buscar(BusquedaContenido busqueda)
        {
            await Task.Delay(1);
            
            EjecutarConteos(busqueda);

            if(busqueda.Elementos.Sum(x => x.Conteo) > 0)
            {
                EjecutarUQeryIds(busqueda);

                var validos = busqueda.Elementos.Where(x => x.Conteo > 0).OrderBy(x => x.Conteo).ToList();
                
                List<string> unicos = new List<string>();
                if(validos.Count > 1)
                {
                    for (int i = 0; i < validos.Count -1; i++)
                    {
                        unicos = validos[i].Ids.Intersect(validos[i + 1].Ids).ToList();
                    }
                } else
                {
                    unicos = validos[0].Ids;
                }

                unicos.LogS();
                await EjecutarUQeryElementos(busqueda, unicos);

                elementos.LogS();
            }

            

            return null;
        }

        private async Task EjecutarUQeryElementos(BusquedaContenido busqueda, List<string> Ids)
        {
            elementos = await BuscarPorIds(busqueda, Ids);
        }


        private void EjecutarUQeryIds(BusquedaContenido busqueda)
        {
            List<Task> conteos = new List<Task>();
            Task<long> ConteoEnFolder = null;
            Task<long> ConteoPropieddes = null;
            Task<long> ConteoMetadatos = null;

            if (busqueda.Conteo(Constantes.ENFOLDER) >0  )
            {
                ConteoEnFolder = ContarEnFolder(busqueda.ObtenerBusqueda(Constantes.ENFOLDER).Consulta, true);
                conteos.Add(ConteoEnFolder);
            }

            if (busqueda.Conteo(Constantes.PROPIEDEDES) > 0)
            {
                ConteoPropieddes = ContarPropiedades(busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES).Consulta, true);
                conteos.Add(ConteoPropieddes);
            }

            if (busqueda.Conteo(Constantes.METADATOS) > 0)
            {
                //ConteoEnFolder = ContarEnMetadatos(busqueda.ObtenerBusqueda(Constantes.METADATOS).Consulta);
                //conteos.Add(ConteoMetadatos);
            }


            if (conteos.Count > 0)
            {
                Task.WaitAll(conteos.ToArray());

                if (busqueda.BuscarEnFolder())
                {
                    busqueda.ActualizaIds(Constantes.ENFOLDER,  IdsEnfolder );
                }

                if (busqueda.BuscarPropiedades())
                {
                    ConteoPropieddes.Result.LogS();
                    busqueda.ActualizaIds(Constantes.PROPIEDEDES, IdsPropiedades);
                }

                if (busqueda.BuscarMetadatos())
                {
                    // busqueda.ActualizaConteo(Constantes.METADATOS, ConteoMetadatos.Result);
                }
            }
        }


        private void EjecutarConteos(BusquedaContenido busqueda)
        {
            List<Task> conteos = new List<Task>();
            Task<long> ConteoEnFolder = null;
            Task<long> ConteoPropieddes = null;
            Task<long> ConteoMetadatos = null;

            if (busqueda.BuscarEnFolder())
            {
                ConteoEnFolder = ContarEnFolder(busqueda.ObtenerBusqueda(Constantes.ENFOLDER).Consulta, false);
                conteos.Add(ConteoEnFolder);
            }

            if (busqueda.BuscarPropiedades())
            {
                ConteoPropieddes = ContarPropiedades(busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES).Consulta, false);
                conteos.Add(ConteoPropieddes);
            }

            if (busqueda.BuscarMetadatos())
            {
                //ConteoEnFolder = ContarEnMetadatos(busqueda.ObtenerBusqueda(Constantes.METADATOS).Consulta);
                //conteos.Add(ConteoMetadatos);
            }


            if (conteos.Count > 0)
            {
                Task.WaitAll(conteos.ToArray());

                if (busqueda.BuscarEnFolder())
                {
                    busqueda.ActualizaConteo(Constantes.ENFOLDER, ConteoEnFolder.Result);
                }

                if (busqueda.BuscarPropiedades())
                {
                    ConteoPropieddes.Result.LogS();
                    busqueda.ActualizaConteo(Constantes.PROPIEDEDES, ConteoPropieddes.Result);
                }

                if (busqueda.BuscarMetadatos())
                {
                    // busqueda.ActualizaConteo(Constantes.METADATOS, ConteoMetadatos.Result);
                }
            }

        }


        #region Gestion de tareas


        public async Task<string> CreaBusqueda(BusquedaContenido busqueda)
        {
            busqueda.Id = Guid.NewGuid().ToString();
            var respuesta = await this.cliente.IndexDocumentAsync(busqueda);
            if (!respuesta.IsValid)
            {
                logger.LogError(respuesta.ServerError.ToString());
                return null;
            }
            return busqueda.Id;
        }

        public async Task<bool> CreaRepositorio()
        {
            logger.LogInformation($"Verificando repositorio");
            if (!await ExisteIndice(INDICEBUSQUEDA))
            {
                logger.LogInformation($"Creando repositorio de bpusqueda de contenido");
                var createIndexResponse = await cliente.Indices.CreateAsync(INDICEBUSQUEDA, c => c
                    .Map<BusquedaContenido>(m => m.AutoMap())
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
            return true;
        }

        public async Task<bool> ExisteIndice(string id)
        {
            var response = await cliente.LowLevel.Indices.ExistsAsync<ExistsResponse>(id);
            return (response.Exists);
        }

        public async Task<BusquedaContenido> ObtieneBusuqeda(string Id)
        {
            var result = await this.cliente.GetAsync<BusquedaContenido>(new GetRequest<BusquedaContenido>(Id));
            if (result.Found)
            {
                return result.Source;
            }
            return null;
        }

        public async Task<bool> ActualizaBusqueda(string Id, BusquedaContenido version)
        {

            var resultado = await cliente.UpdateAsync<BusquedaContenido, object>(
                new DocumentPath<BusquedaContenido>(Id),
               u => u.Index(INDICEBUSQUEDA)
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

        public async Task<bool> EliminaBusqueda(string Id)
        {
            var resultado = await cliente.DeleteAsync<BusquedaContenido>(
                new DocumentPath<BusquedaContenido>(Id));

            if (resultado.Result == Result.Deleted)
            {
                return true;
            }

            return false;
        }

        public void Inicializar(string contentPath, bool generarDatosdemo)
        {
            Task.Run(() => CreaRepositorio()).Wait();
        }

        #endregion



    }
}
