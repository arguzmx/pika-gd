﻿using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ES = Elasticsearch.Net;

namespace PIKA.ServicioBusqueda.Contenido
{
    public partial class ServicioBusquedaContenido : IServicioBusquedaContenido
    {
        private IConfiguration Configuration;
        private ElasticClient cliente;
        private const string INDICEBUSQUEDA = "contenido-busqueda";
        private const string INDICECONTENIDO = "contenido-indexado";
        private const string INDICEVERSIONES = "contenido-versiones";
        private ILogger logger;
        private HashSet<Elemento> elementos;
        private IAppCache cache;
        private IRepositorioMetadatos repoMetadatos;
        private IServicioPlantilla servicioPlantilla;
        private IRepoContenidoElasticSearch repoContenido;
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
            IRepoContenidoElasticSearch repoContenido,
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
            this.repoContenido = repoContenido;
        }

        public void CreaClientes(IConfiguration configuration)
        {
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("BusquedaContenido").Bind(options);

            var settingsVersiones = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(INDICEBUSQUEDA);

            cliente = new ElasticClient(settingsVersiones);
        }

        public async Task<List<HighlightHit>> BuscarSinopsis(string Id, List<string> Ids)
        {
            BusquedaContenido bc = cache.Get<BusquedaContenido>($"b{Id}");
            CacheBusqueda cacheB = cache.Get<CacheBusqueda>(Id);

            if (cacheB != null && bc != null)
            {
                var b = bc.ObtenerBusqueda(Constantes.TEXTO);
                var fTexto = b.Consulta.Filtros.Where(f => f.Propiedad == "texto").SingleOrDefault();
                return await repoContenido.SinopsisPorIDs(fTexto.ValorString, Ids, "b", fTexto.ValorString.Length * 5);
            } 
            return new List<HighlightHit>();
        }

        public async Task<Paginado<string>> BuscarIds(BusquedaContenido busqueda)
        {
            try
            {

                if (cache.Get<BusquedaContenido>($"b{busqueda.Id}") == null)
                {
                    cache.Add($"b{busqueda.Id}", busqueda);
                }

                CacheBusqueda cacheB = null;

                // busca cache existente de los datos
                if (!busqueda.recalcular_totales) cacheB = cache.Get<CacheBusqueda>(busqueda.Id);

                if (cacheB == null)
                {
                    cacheB = new CacheBusqueda()
                    {
                        Id = busqueda.Id,
                        sort_col = busqueda.ord_columna,
                        sort_dir = busqueda.ord_direccion
                    };

                    // Caclcula las cantidades existentes por cada tipo de busqueda
                    await EjecutarConteos(busqueda);

                    // Si hay coincidencias 
                    if (busqueda.Elementos.Sum(x => x.Conteo) > 0)
                    {
                        // Realiza la búsqueda por tipo
                        await EjecutarUQeryIds(busqueda);

                        // Orderna la búsqueda en bae al menor numero de resultados
                        // para restringir el conteo total y las intersecciones
                        var validos = busqueda.Elementos.OrderBy(x => x.Conteo).ToList();

                        List<string> unicos = new List<string>();
                        if (validos.Count > 1)
                        {
                            // Hay más de un tipo de consulta involucrado 

                            switch (validos[0].Tag)
                            {

                                case Constantes.TEXTO:
                                    if (busqueda.BuscarTexto())
                                    {
                                        // para el caso de los datos que vienen de Elasticsearch se separa el id del elemento del del repositorio
                                        unicos = validos[0].Ids == null ? new List<string>() : validos[0].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList();
                                    }
                                    break;

                                case Constantes.METADATOS:
                                    if (busqueda.BuscarMetadatos())
                                    {
                                        // para el caso de los datos que vienen de Elasticsearch se separa el id del elemento del del repositorio
                                        unicos = validos[0].Ids == null ? new List<string>() : validos[0].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList();
                                    }
                                    break;

                                case Constantes.ENFOLDER:
                                    if (busqueda.BuscarEnFolder())
                                    {
                                        unicos = new List<string>();
                                    }
                                    break;

                                case Constantes.PROPIEDEDES:
                                    if (busqueda.BuscarPropiedades())
                                    {
                                        unicos = validos[0].Ids == null ? new List<string>() : validos[0].Ids;
                                    }
                                    break;
                            }

                            unicos.Count.ToS();

                            // Hay mas de un tipo de elementos participando en la respuesta
                            // La interseccion comienza desde el 2o elemento  por lo que el índice es 1
                            for (int i = 1; i < validos.Count; i++)
                            {
                                switch (validos[i].Tag)
                                {
                                    case Constantes.TEXTO:
                                        if (busqueda.BuscarTexto())
                                        {
                                            // Se separa el valor del elemento de la clave de Elasticsearch
                                            unicos = unicos.Intersect(
                                                validos[i].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList()
                                                ).ToList();
                                        }
                                        break;

                                    case Constantes.METADATOS:
                                        if (busqueda.BuscarMetadatos())
                                        {
                                            // Se separa el valor del elemento de la clave de Elasticsearch
                                            unicos = unicos.Intersect(
                                            validos[i].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList()
                                            ).ToList();
                                        }
                                        break;

                                    case Constantes.PROPIEDEDES:
                                        if (busqueda.BuscarPropiedades())
                                        {
                                            unicos = unicos.Intersect(validos[i].Ids).ToList();
                                        }
                                        break;

                                    case Constantes.ENFOLDER:
                                        if (busqueda.BuscarPropiedades())
                                        {
                                            //unicos = unicos.Intersect(validos[i].Ids).ToList();
                                        }
                                        break;
                                }
                            }

                        }
                        else
                        {
                            // La respuesta proviene de un solo tipo de elementos
                            switch (validos[0].Tag)
                            {
                                case Constantes.TEXTO:
                                case Constantes.METADATOS:
                                    // Se separa el id del elemento ddel id del repositorio
                                    unicos = validos[0].Ids == null ? new List<string>() : validos[0].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList();
                                    break;

                                default:
                                    unicos = validos[0].Ids == null ? new List<string>() : validos[0].Ids;
                                    break;
                            }
                        }


                        cacheB = new CacheBusqueda()
                        {
                            Id = busqueda.Id,
                            Unicos = unicos,
                            UnicosElastic = new List<string>(),
                            sort_col = busqueda.ord_columna,
                            sort_dir = busqueda.ord_direccion
                        };


                        //// Se añade la lista de Ids únicos de Elasticsearch para metadatos 
                        //if (unicos.Count > 0 && validos.Any(x => x.Tag == Constantes.METADATOS))
                        //{
                        //    var t = validos.Where(x => x.Tag == Constantes.METADATOS).First();

                        //    unicos.ForEach(u =>
                        //    {
                        //        var IdElastic = t.Ids.Where(x => x.StartsWith(u)).FirstOrDefault();
                        //        if (cacheB.UnicosElastic.IndexOf(IdElastic.Split('|')[Constantes.INDICEIDELASTIC]) < 0)
                        //        {
                        //            cacheB.UnicosElastic.Add(IdElastic.Split('|')[Constantes.INDICEIDELASTIC]);
                        //        }
                        //    });
                        //}

                        cache.Add(busqueda.Id, cacheB);
                    }
                }

                Paginado<string> p = new Paginado<string>()
                {
                    Elementos = new List<string>() { busqueda.Id },
                    ConteoTotal = cacheB.Unicos.Count(),
                    ConteoFiltrado = cacheB.Unicos.Count()
                };

                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<Paginado<ElementoBusqueda>> Buscar(BusquedaContenido busqueda)
        {

            try
            {
                if (cache.Get<BusquedaContenido>($"b{busqueda.Id}") == null)
                {
                    cache.Add($"b{busqueda.Id}", busqueda);
                }


                CacheBusqueda cacheB = null;

                // busca cache existente de los datos
                if (!busqueda.recalcular_totales) cacheB = cache.Get<CacheBusqueda>(busqueda.Id);

                if (cacheB == null)
                {
                    cacheB = new CacheBusqueda()
                    {
                        Id = busqueda.Id,
                        sort_col = busqueda.ord_columna,
                        sort_dir = busqueda.ord_direccion
                    };

                    await EjecutarConteos(busqueda);

                    if (busqueda.Elementos.Sum(x => x.Conteo) > 0)
                    {

                        // Obtiene la lsta de identificadores únicos para cada tipo de busqueda
                        await EjecutarUQeryIds(busqueda);

                        var validos = busqueda.Elementos.Where(x => x.Conteo > 0)
                            .OrderBy(x => x.Conteo).ToList();

                        List<string> unicos = new List<string>();
                        if (validos.Count > 1)
                        {
                            switch (validos[0].Tag)
                            {
                                case Constantes.TEXTO:
                                case Constantes.METADATOS:
                                    unicos = validos[0].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList();
                                    break;

                                case Constantes.ENFOLDER:
                                    unicos = new List<string>();
                                    break;

                                case Constantes.PROPIEDEDES:
                                    unicos = validos[0].Ids;
                                    break;
                            }


                            // Hay mas de un tipo de elementos participando en la respuesta
                            for (int i = 1; i < validos.Count; i++)
                            {
                                switch (validos[i].Tag)
                                {
                                    case Constantes.TEXTO:
                                    case Constantes.METADATOS:
                                        unicos = unicos.Intersect(
                                            validos[i].Ids.Select(x => x.Split('|')[Constantes.INDICEDOCUMENTO]).ToList()
                                            ).ToList();
                                        break;

                                    default:
                                        unicos = unicos.Intersect(validos[i].Ids).ToList();
                                        break;
                                }

                            }
                        }
                        else
                        {

                            // La respuesta proviene de un solo tipo de elementos
                            switch (validos[0].Tag)
                            {
                                case Constantes.TEXTO:
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
                            sort_col = busqueda.ord_columna,
                            sort_dir = busqueda.ord_direccion
                        };

                        if (unicos.Count > 0 && validos.Any(x => x.Tag == Constantes.METADATOS))
                        {
                            var t = validos.Where(x => x.Tag == Constantes.METADATOS).First();

                            unicos.Take(busqueda.tamano).ToList().ForEach(u =>
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

                    if (!string.IsNullOrEmpty(busqueda.PlantillaId))
                    {
                        busqueda.PlantillaId = busqueda.ObtenerBusqueda(Constantes.METADATOS).Topico;
                    }


                    if (busqueda.OrdenamientoMetadatos() && (cacheB.UnicosElastic.Count() > 0))
                    {

                        await GeneraPaginaMetadatos(busqueda, p, cacheB);

                        List<string> documentos = p.PropiedadesExtendidas.ValoresEntidad.Select(x => x.Id).ToList();
                        List<string> faltantes = cacheB.Unicos.Except(documentos).ToList();
                        if (documentos.Count() < busqueda.tamano)
                        {
                            int cuantos = busqueda.tamano - documentos.Count();
                            int conteofaltantes = faltantes.Count();
                            for (int i = 0; i < cuantos; i++)
                            {
                                if (conteofaltantes >= i + 1)
                                {
                                    documentos.Add(faltantes[i]);
                                }
                                else
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
            Plantilla Plantilla = await servicioPlantilla.UnicoAsync(x => x.Id == busqueda.PlantillaId, null,
                    p => p.Include(z => z.Propiedades));


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

            p.PropiedadesExtendidas.ValoresEntidad = await this.repoMetadatos.ConsultaPaginaMetadatosPorListaIds(Plantilla, b.UnicosElastic, busqueda.AConsulta());

        }

        private async Task EjecutarUQeryIds(BusquedaContenido busqueda)
        {
            List<Task> busquedaIds = new List<Task>();
            Task<long> ConteoEnFolder = null;
            Task<long> ConteoPropieddes = null;
            Task<List<string>> idsTexto = null;
            Task<List<string>> ConteoMetadatos = null;



            if (busqueda.Conteo(Constantes.TEXTO) > 0)
            {
                var b = busqueda.ObtenerBusqueda(Constantes.TEXTO);
                var fTexto = b.Consulta.Filtros.Where(f => f.Propiedad == "texto").SingleOrDefault();
                var fFuzzy = b.Consulta.Filtros.Where(f => f.Propiedad == "fuzzy").SingleOrDefault();
                if (fTexto != null && !string.IsNullOrEmpty(fTexto.ValorString))
                {
                    int fuzzy = 0;
                    if (fFuzzy != null)
                    {
                        int.TryParse(fFuzzy.ValorString, out fuzzy);
                    }
                    idsTexto = this.repoContenido.IdsPorConsulta(fTexto.ValorString, busqueda.PuntoMontajeId, busqueda.JerarquiaId, fuzzy);
                    busquedaIds.Add(idsTexto);
                }
            }

            if (busqueda.Conteo(Constantes.METADATOS) > 0)
            {
                var filtro = busqueda.ObtenerBusqueda(Constantes.METADATOS);
                ConteoMetadatos = this.repoMetadatos.IdsrPorConsulta(filtro.Consulta, this.Plantilla, busqueda.PuntoMontajeId, busqueda.JerarquiaId);
                busquedaIds.Add(ConteoMetadatos);
            }


            if (busqueda.Conteo(Constantes.ENFOLDER) > 0)
            {
                ConteoEnFolder = ContarEnFolder(busqueda.ObtenerBusqueda(Constantes.ENFOLDER).Consulta, true);
                busquedaIds.Add(ConteoEnFolder);
            }

            if (busqueda.Conteo(Constantes.PROPIEDEDES) > 0)
            {
                ConteoPropieddes = ContarPropiedades(busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES).Consulta, true, busqueda.PuntoMontajeId);
                busquedaIds.Add(ConteoPropieddes);
            }

            if (busquedaIds.Count > 0)
            {
                Task.WaitAll(busquedaIds.ToArray());

                if (busqueda.BuscarEnFolder() && (busqueda.Conteo(Constantes.ENFOLDER) > 0))
                {
                    busqueda.ActualizaIds(Constantes.ENFOLDER, IdsEnfolder);
                }

                if (busqueda.BuscarPropiedades() && (busqueda.Conteo(Constantes.PROPIEDEDES) > 0))
                {
                    busqueda.ActualizaIds(Constantes.PROPIEDEDES, IdsPropiedades);
                }

                if (busqueda.BuscarMetadatos() && (busqueda.Conteo(Constantes.METADATOS) > 0))
                {
                    busqueda.ActualizaIds(Constantes.METADATOS, ConteoMetadatos.Result);
                }

                if (busqueda.BuscarTexto() && (busqueda.Conteo(Constantes.TEXTO) > 0))
                {
                    // En el caso de la busqueda de texto pueden contrarse multimples hits en un documento 
                    // si tiene varias páginas por lo que el conteo final proviene de la lista de Ids distintos
                    busqueda.ActualizaIds(Constantes.TEXTO, idsTexto.Result);
                    busqueda.ActualizaConteo(Constantes.TEXTO, idsTexto.Result.Count);
                }
            }
        }


        private async Task EjecutarConteos(BusquedaContenido busqueda)
        {
            List<Task> conteos = new List<Task>();
            Task<long> ConteoEnFolder = null;
            Task<long> ConteoPropieddes = null;
            Task<long> ConteoMetadatos = null;
            Task<long> ConteoTexto = null;

            if (busqueda.BuscarTexto())
            {
                var b = busqueda.ObtenerBusqueda(Constantes.TEXTO);
                var fTexto = b.Consulta.Filtros.Where(f => f.Propiedad == "texto").SingleOrDefault();
                var fFuzzy = b.Consulta.Filtros.Where(f => f.Propiedad == "fuzzy").SingleOrDefault();
                if (fTexto != null && !string.IsNullOrEmpty(fTexto.ValorString))
                {
                    int fuzzy = 0;
                    if (fFuzzy != null)
                    {
                        int.TryParse(fFuzzy.ValorString, out fuzzy);
                    }
                    ConteoTexto = this.repoContenido.ContarPorConsulta(fTexto.ValorString, busqueda.PuntoMontajeId, busqueda.JerarquiaId, fuzzy);
                    conteos.Add(ConteoTexto);
                }
            }

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

                ConteoMetadatos = this.repoMetadatos.ContarPorConsulta(filtro.Consulta, this.Plantilla, busqueda.PuntoMontajeId, busqueda.JerarquiaId);
                conteos.Add(ConteoMetadatos);
            }


            if (busqueda.BuscarPropiedades())
            {
                ConteoPropieddes = ContarPropiedades(busqueda.ObtenerBusqueda(Constantes.PROPIEDEDES).Consulta, false, busqueda.PuntoMontajeId);
                conteos.Add(ConteoPropieddes);
            }


            if (busqueda.BuscarEnFolder())
            {
                ConteoEnFolder = ContarEnFolder(busqueda.ObtenerBusqueda(Constantes.ENFOLDER).Consulta, false);
                conteos.Add(ConteoEnFolder);
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

                if (busqueda.BuscarTexto())
                {
                    busqueda.ActualizaConteo(Constantes.TEXTO, ConteoTexto.Result);
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
                logger.LogInformation("Actualizando repositorio contenido");
                var body = ES.PostData.String("{\"properties\": {\"partes\": {\"properties\": {\"xid\": {\"type\":\"keyword\" }}}}}");
                var r = cliente.LowLevel.Indices.PutMapping<PutMappingResponse>(INDICEVERSIONES, body);

                body = ES.PostData.String("{\"properties\": {\"ocr_id\": {\"type\":\"keyword\" }}}");
                r = cliente.LowLevel.Indices.PutMapping<PutMappingResponse>(INDICEVERSIONES, body);

                body = ES.PostData.String("{\"properties\": {\"ocr_f\": {\"type\":\"date\" }}}");
                r = cliente.LowLevel.Indices.PutMapping<PutMappingResponse>(INDICEVERSIONES, body);

                                
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
