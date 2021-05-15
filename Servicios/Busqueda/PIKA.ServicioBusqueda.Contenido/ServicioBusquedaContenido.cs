using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Interfaces;
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
        private IAppCache cache;
        private IRepositorioMetadatos repoMetadatos;
        private IServicioPlantilla servicioPlantilla;
        private Plantilla Plantilla;

        /// <summary>
        /// Lista los identificadores del filtro 'en folder'
        /// </summary>
        private List<string> IdsEnfolder;

        /// <summary>
        /// Lista los identificadores del filtro 'propiedades'
        /// </summary>
        private List<string> IdsPropiedades;


        private UnidadDeTrabajo<DbContextBusquedaContenido> UDT;
        protected DbContextBusquedaContenido contexto;

        public ServicioBusquedaContenido(
            IServicioPlantilla servicioPlantilla,
            IRepositorioMetadatos repositorioMetadatos,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextBusquedaContenido> proveedorOpciones,
            IConfiguration Configuration,
            ILoggerFactory loggerFactory)
        {
            DbContextContenidoFactory cf = new DbContextContenidoFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.cache = cache;
            this.logger = loggerFactory.CreateLogger(nameof(ServicioBusquedaContenido));
            this.Configuration = Configuration;
            this.CreaClientes(this.Configuration);
            this.UDT = new UnidadDeTrabajo<DbContextBusquedaContenido>(contexto);
            this.repoMetadatos = repositorioMetadatos;
            this.servicioPlantilla = servicioPlantilla;
        }

        public void CreaClientes(IConfiguration configuration)
        {
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("BusquedaContenido").Bind(options);

            var settingsVersiones = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(INDICEBUSQUEDA);

            cliente = new ElasticClient(settingsVersiones);
        }



        public async Task<Paginado<ElementoBusqueda>> Buscar(BusquedaContenido busqueda)
        {

            try
            {



                CacheBusqueda cacheB = null;

                if (!busqueda.recalcular_totales)
                {
                    cacheB = cache.Get<CacheBusqueda>(busqueda.Id);

                }


                if (cacheB == null)
                {

                    await EjecutarConteos(busqueda);

                    if (busqueda.Elementos.Sum(x => x.Conteo) > 0)
                    {
                        await EjecutarUQeryIds(busqueda);

                        var validos = busqueda.Elementos.Where(x => x.Conteo > 0)
                            .OrderBy(x => x.Conteo).ToList();

                        List<string> unicos = new List<string>();
                        if (validos.Count > 1)
                        {
                            for (int i = 0; i < validos.Count - 1; i++)
                            {
                                switch (validos[i].Tag)
                                {
                                    case Constantes.METADATOS:
                                        unicos = validos[i].Ids.Intersect(
                                            validos[i].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList()
                                            ).ToList();
                                        break;

                                    default:
                                        unicos = validos[i].Ids.Intersect(validos[i + 1].Ids).ToList();
                                        break;
                                }
                            }
                        }
                        else
                        {
                            switch (validos[0].Tag)
                            {
                                case Constantes.METADATOS:
                                    unicos = validos[0].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList();
                                    break;

                                default:
                                    unicos = validos[0].Ids;
                                    break;
                            }
                        }

                        cacheB = new CacheBusqueda()
                        {
                            Id = busqueda.Id,
                            Unicos = unicos,
                            UnicosElastic = new List<string>(),
                            Plantilla = this.Plantilla,
                            sort_col = busqueda.ord_columna,
                            sort_dir = busqueda.ord_direccion
                        };

                        if (unicos.Count > 0 && validos.Any(x => x.Tag == Constantes.METADATOS))
                        {
                            var t = validos.Where(x => x.Tag == Constantes.METADATOS).First();

                            unicos.ForEach(u =>
                            {
                                var IdElastic = t.Ids.Where(x => x.StartsWith(u)).SingleOrDefault();
                                cacheB.UnicosElastic.Add(IdElastic.Split('|')[Constantes.INDICEIDELASTIC]);
                            });
                        }


                        cache.Add(busqueda.Id, cacheB);
                    }
                }



                // Comienzan los querys paginados

                Paginado<ElementoBusqueda> p = new Paginado<ElementoBusqueda>()
                {
                    ConteoFiltrado = 0,
                    ConteoTotal = 0,
                    Desde = busqueda.indice,
                    Elementos = new List<ElementoBusqueda>(),
                    Indice = busqueda.indice,
                    Paginas = 0,
                    Tamano = busqueda.tamano
                };



                if (cacheB != null)
                {
                    p.ConteoFiltrado = cacheB.Unicos.Count;
                    p.ConteoTotal = cacheB.Unicos.Count;
                    p.Paginas = (cacheB.Unicos.Count % busqueda.tamano) > 0 
                        ? (int)(cacheB.Unicos.Count / busqueda.tamano) + 1 
                        : (int)(cacheB.Unicos.Count / busqueda.tamano);

                    if (!string.IsNullOrEmpty(busqueda.PlantillaId) )
                    {
                        busqueda.PlantillaId = busqueda.ObtenerBusqueda(Constantes.METADATOS).Topico;
                    }

                    
                    // esta diferecnaición es necesario debodi a que puede haber documentos sin metadatos
                    if (busqueda.OrdenamientoMetadatos() && (cacheB.UnicosElastic.Count() > 0))
                    {
                        
                        await GeneraPaginaMetadatos(busqueda, p, cacheB);

                        List<string> documentos = p.PropiedadesExtendidas.ValoresEntidad.Select(x => x.Id).ToList();
                        List<string> faltantes = cacheB.Unicos.Except(documentos).ToList();
                        if(documentos.Count() < busqueda.tamano)
                        {
                            int cuantos = busqueda.tamano - documentos.Count();
                            int conteofaltantes = faltantes.Count();
                            for(int i= 0; i < cuantos; i++)
                            {
                                if (conteofaltantes >= i + 1)
                                {
                                    documentos.Add(faltantes[i]);
                                } else
                                {
                                    break;
                                }
                            }
                        }
                        faltantes.Clear();
                        ///COumentoc contien ahor la lista de elementos para la busqueda
                        p.Elementos = (await BuscarPorIds(busqueda, documentos, true)).ToList();
                    }
                    else
                    {

                        p.Elementos = (await BuscarPorIds(busqueda, cacheB.Unicos, false)).ToList();
                        if (!string.IsNullOrEmpty(busqueda.PlantillaId) && p.ConteoTotal > 0)
                        {
                            await GeneraMetadatos(busqueda, p, cacheB);
                        }
                    }
                    
                }

                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private async Task GeneraPaginaMetadatos(BusquedaContenido busqueda, Paginado<ElementoBusqueda> p, CacheBusqueda b)
        {
            Plantilla Plantilla = null;
            if (busqueda.PlantillaId != b.Plantilla.Id)
            {
                Plantilla = await servicioPlantilla.UnicoAsync(x => x.Id == busqueda.PlantillaId, null,
                    p => p.Include(z => z.Propiedades));
            }
            {
                Plantilla = b.Plantilla;
            }

            p.PropiedadesExtendidas = new PropiedadesExtendidas();
            Plantilla.Propiedades.ToList().OrderBy(x => x.IndiceOrdenamiento).ToList().ForEach(prop =>
            {
                p.PropiedadesExtendidas.Propiedades.Add(
                    new PropiedadExtendida()
                    {
                        Id = prop.Id,
                        Nombre = prop.Nombre,
                        PlantillaId = Plantilla.Id,
                        TipoDatoId = prop.TipoDatoId
                    }
                    );
            });

            p.PropiedadesExtendidas.ValoresEntidad = await this.repoMetadatos.ConsultaPaginaMetadatosPorListaIds(b.Plantilla, b.UnicosElastic, busqueda.AConsulta());

        }


        private async Task GeneraMetadatos(BusquedaContenido busqueda, Paginado<ElementoBusqueda> p, CacheBusqueda b)
        {
            Plantilla Plantilla = null;
            if (busqueda.PlantillaId != b.Plantilla.Id)
            {
                Plantilla = await servicioPlantilla.UnicoAsync(x => x.Id == busqueda.PlantillaId, null,
                    p => p.Include(z => z.Propiedades));
            }
            {
                Plantilla = b.Plantilla;
            }

            p.PropiedadesExtendidas = new PropiedadesExtendidas();
            Plantilla.Propiedades.ToList().OrderBy(x => x.IndiceOrdenamiento).ToList().ForEach(prop =>
             {
                 p.PropiedadesExtendidas.Propiedades.Add(
                     new PropiedadExtendida()
                     {
                         Id = prop.Id,
                         Nombre = prop.Nombre,
                         PlantillaId = Plantilla.Id,
                         TipoDatoId = prop.TipoDatoId
                     }
                     );
             });

            List<string> elastic = new List<string>();
            p.Elementos.Select(c => c.Id).ToList().ForEach(e =>
            {

                int index = b.Unicos.IndexOf(e);
                if (index >= 0)
                {
                    elastic.Add(b.UnicosElastic[index]);
                }

            });

            p.PropiedadesExtendidas.ValoresEntidad = await this.repoMetadatos.ConsultaMetadatosPorListaIds(b.Plantilla, elastic);

        }

        private async Task EjecutarUQeryIds(BusquedaContenido busqueda)
        {
            List<Task> conteos = new List<Task>();
            Task<long> ConteoEnFolder = null;
            Task<long> ConteoPropieddes = null;
            Task<List<string>> ConteoMetadatos = null;

            if (busqueda.Conteo(Constantes.METADATOS) > 0)
            {
                var filtro = busqueda.ObtenerBusqueda(Constantes.METADATOS);
                ConteoMetadatos = this.repoMetadatos.IdsrPorConsulta(filtro.Consulta, this.Plantilla, busqueda.PuntoMontajeId);
                conteos.Add(ConteoMetadatos);
            }


            if (busqueda.Conteo(Constantes.ENFOLDER) > 0)
            {
                ConteoEnFolder = ContarEnFolder(busqueda.ObtenerBusqueda(Constantes.ENFOLDER).Consulta, true);
                conteos.Add(ConteoEnFolder);
            }

            if (busqueda.Conteo(Constantes.PROPIEDEDES) > 0)
            {
                ConteoPropieddes = ContarPropiedades(busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES).Consulta, true, busqueda.PuntoMontajeId);
                conteos.Add(ConteoPropieddes);
            }

            if (conteos.Count > 0)
            {
                Task.WaitAll(conteos.ToArray());

                if (busqueda.BuscarEnFolder())
                {
                    busqueda.ActualizaIds(Constantes.ENFOLDER, IdsEnfolder);
                }

                if (busqueda.BuscarPropiedades())
                {
                    busqueda.ActualizaIds(Constantes.PROPIEDEDES, IdsPropiedades);
                }

                if (busqueda.BuscarMetadatos())
                {
                    busqueda.ActualizaIds(Constantes.METADATOS, ConteoMetadatos.Result);
                }
            }
        }


        private async Task EjecutarConteos(BusquedaContenido busqueda)
        {
            List<Task> conteos = new List<Task>();
            Task<long> ConteoEnFolder = null;
            Task<long> ConteoPropieddes = null;
            Task<long> ConteoMetadatos = null;


            if (busqueda.BuscarMetadatos())
            {
                var filtro = busqueda.ObtenerBusqueda(Constantes.METADATOS);
                this.Plantilla = await servicioPlantilla.UnicoAsync(x => x.Id == filtro.Topico, null, p => p.Include(z => z.Propiedades));
                if (this.Plantilla != null)
                {
                    foreach (var p in Plantilla.Propiedades)
                    {
                        if (p.TipoDatoId == TipoDato.tList)
                        {
                            p.ValoresLista = await servicioPlantilla.ObtenerValores(p.Id);
                        }
                    }
                }

                ConteoMetadatos = this.repoMetadatos.ContarPorConsulta(filtro.Consulta, this.Plantilla, busqueda.PuntoMontajeId);
                conteos.Add(ConteoMetadatos);
            }

            if (busqueda.BuscarEnFolder())
            {
                ConteoEnFolder = ContarEnFolder(busqueda.ObtenerBusqueda(Constantes.ENFOLDER).Consulta, false);
                conteos.Add(ConteoEnFolder);
            }

            if (busqueda.BuscarPropiedades())
            {
                ConteoPropieddes = ContarPropiedades(busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES).Consulta, false, busqueda.PuntoMontajeId);
                conteos.Add(ConteoPropieddes);
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
                    busqueda.ActualizaConteo(Constantes.PROPIEDEDES, ConteoPropieddes.Result);
                }

                if (busqueda.BuscarMetadatos())
                {
                    busqueda.ActualizaConteo(Constantes.METADATOS, ConteoMetadatos.Result);
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
