using Microsoft.Extensions.Configuration;
using ES = Elasticsearch.Net;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using PIKA.Infraestructura.Comun;
using Microsoft.Extensions.Logging;
using Elasticsearch.Net;
using System.Text.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using PIKA.Modelo.Metadatos.Instancias;
using PIKA.Infraestructura.Comun.Excepciones;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public partial class RepoMetadatosElasticSearch : IRepositorioMetadatos,
        IRepositorioInicializableAutoConfigurable
    {
        public static DateTime FechaBaseHora => new DateTime(2000, 1, 1, 0, 0, 0);
        public const string NOMBREINDICEVINCULOS = "vinculosmetadatos";

        private IConfiguration Configuration;
        private ElasticClient cliente;
        private ElasticClient clienteVinculos;
        private ILogger logger;

        /// <summary>
        /// Esta constructor es para ser llamda vía IRepositorioInicializableAutoConfigurable
        /// </summary>
        public RepoMetadatosElasticSearch()
        {
        }

        public RepoMetadatosElasticSearch(
            IConfiguration configuration, 
            ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(nameof(RepoMetadatosElasticSearch));
            CreaClientes(configuration);
        }

        /// <summary>
        /// Crea los clientes de conexión a elasticsearch
        /// </summary>
        /// <param name="configuration"></param>
        public void CreaClientes(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Metadatos").Bind(options);
            var settings = new ConnectionSettings(new Uri(options.CadenaConexion()));
            var settingsVinculos = new ConnectionSettings(new Uri(options.CadenaConexion()))
                .DefaultIndex(NOMBREINDICEVINCULOS);

            clienteVinculos = new ElasticClient(settingsVinculos);
            cliente = new ElasticClient(settings);
        }

        #region métodos comunes
        public async Task<string> CrearIndice(Plantilla plantilla)
        {
            if (!await ExisteIndice(plantilla.Id))
            {
                var body = ES.PostData.String(plantilla.ObtieneJSONPlantilla());
                var response = await cliente.LowLevel.Indices.CreateAsync<CreateIndexResponse>(plantilla.Id, body);
            }
            return plantilla.Id;
        }

        public async Task<bool> ExisteIndice(string id)
        {
            var response = await cliente.LowLevel.Indices.ExistsAsync<ExistsResponse>(id);
            return (response.Exists);
        }

        public async Task Inicializar(IConfiguration configuration, string contentPath, bool generarDatosdemo, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(nameof(RepoMetadatosElasticSearch));
            CreaClientes(configuration);
            await VerificaRepositoiorVinculos();

        }
        #endregion


        public async Task<DocumentoPlantilla> Inserta(string tipoOrigenId, string origenId, bool esLista, string ListaId,
            Plantilla plantilla, RequestValoresPlantilla valores, string nombreRelacion = "")
        {

            if (esLista)
            {
                VinculosObjetoPlantilla v = await ObtieneVinculos(valores.Tipo, valores.Id);
                if (v == null || !v.Listas.Any(x=>x.ListaId == ListaId)) throw new EXNoEncontrado(ListaId);
            }

            
            DocumentoPlantilla valoresplantilla = new DocumentoPlantilla()
            {
                Id = Guid.NewGuid().ToString(),
                DatoId = valores.Id,
                TipoDatoId = valores.Tipo,
                PlantillaId = plantilla.Id,
                TipoOrigenId = tipoOrigenId,
                OrigenId = origenId,
                Valores = valores.Valores,
                IndiceFiltrado = valores.Filtro,
                EsLista = esLista,
                ListaId = ListaId
            };

           
            var body = ES.PostData.String(valoresplantilla.ObtieneJSONValores(plantilla));

            Console.WriteLine($"Passed 2");
            var response = await cliente.LowLevel.CreateAsync<CreateResponse>(plantilla.Id, valoresplantilla.Id, body);

            Console.WriteLine($"Passed 3");
            if (response.ApiCall.Success)
            {
                if (!esLista)
                {
                    await CrearVinculoDocumento(valoresplantilla.TipoDatoId, valoresplantilla.DatoId, valoresplantilla.Id,
                        valoresplantilla.PlantillaId, nombreRelacion);
                }

                return valoresplantilla;
            }

            return null;
        }

        public async Task<bool> Actualiza(string id, Plantilla plantilla, RequestValoresPlantilla request)
        {
            var doc = await Unico(plantilla, id);

            if (doc != null)
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


                throw new Exception("No fue posible actualizar el documento " + id);

            } else
            {
                throw new EXNoEncontrado(id);
            }
            
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


        public async Task<string> CreaLista(string plantillaid, RequestCrearLista request)
        {
            string listaid = Guid.NewGuid().ToString();
            if( await CrearVinculoLista(request.Tipo, request.Id, listaid, plantillaid, request.Nombre))
            {
                return listaid;
            } else
            {
                throw new Exception("Error al crear list");
            }
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
                    for (int i = 0; i <= esr.hits.total.value; i++)
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



        public async Task<bool> EliminaDocumento(string docid, string plantillaId)
        {
            var r = await cliente.LowLevel.SearchAsync<StringResponse>(plantillaId, docid.BuscarId());
            if (r.Success)
            {
                
                ElasticsearchResult esr = JsonSerializer.Deserialize<ElasticsearchResult>(r.Body);
                if (esr.hits.total.value > 0)
                {
                    JsonElement e = (JsonElement)esr.hits.hits[0];
                    dynamic d = JObject.Parse(e.ToString());
                    DocumentoPlantilla doc = ElasticJSONExtender.Valores(d, null);

                    var del = await cliente.LowLevel.DeleteByQueryAsync<DeleteByQueryResponse>(plantillaId, docid.BuscarId());
                    if (del.ApiCall.Success)
                    {
                        if (del.Total == 0)
                        {
                            throw new EXNoEncontrado(docid);

                        }
                        else
                        {
                            return await EliminaVinculoDocumento(doc.TipoDatoId, doc.DatoId, docid);
                        }
                    }

                }

            }

            throw new Exception($"No fue posible eliminar el documento: {r.OriginalException.Message}");
        }


        public async Task<long> EliminaListaDocumentos(string listaId, string plantillaId)
        {
            var r = await cliente.LowLevel.DeleteByQueryAsync<DeleteByQueryResponse>(plantillaId, listaId.BuscarPorLista());
            if (r.ApiCall.Success)
            {
                await EliminaVinculoLista(listaId, plantillaId);
                return r.Total;
            }

            throw new Exception($"No fue posible eliminar la lista: {r.OriginalException.Message}");
        }

        public async Task<bool> ActualizaDesdePlantilla(Plantilla plantilla)
        {
                if (!await ExisteIndice(plantilla.Id))
                {
                    await CrearIndice(plantilla);
                }

                var r = await cliente.LowLevel.Indices.GetMappingAsync<StringResponse>(plantilla.Id);

                if (r.Success)
                {
                    List<PropiedadPlantilla> adicionar = new List<PropiedadPlantilla>();
                    foreach (var prop in plantilla.Propiedades)
                    {
                        if (r.Body.IndexOf(prop.Id) < 0)
                        {
                            adicionar.Add(prop);
                        }
                    }

                    if (adicionar.Count > 0)
                    {
                        var rq = ES.PostData.String(adicionar.ObtieneJSONActualizarPlantilla());
                        var pmr = await cliente.LowLevel.Indices.PutMappingAsync<PutMappingResponse>(plantilla.Id, rq);
                        if (pmr.ApiCall.Success)
                        {
                            return true;
                        }
                    }

                }
                return false;
            
        }

        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



        public async Task<bool> ActualizarIndice(Plantilla plantilla)
        {
            var r = new GetMappingRequest( Indices.Index(plantilla.Id));
            var result = await cliente.Indices.GetMappingAsync(r);

            if (result.IsValid)
            {
                var indices = result.Indices[plantilla.Id].Mappings.Properties.ToList();
                List<int> ids = new List<int>();
                plantilla.Propiedades.ToList().ForEach(p =>
                {
                    if(!indices.Where(x=>x.Key.Name == $"P{p.IdNumericoPlantilla}").Any())
                    {
                        ids.Add(p.IdNumericoPlantilla);
                    }
                }
                    );

                if (ids.Count > 0)
                {

                    var body = ES.PostData.String(plantilla.ObtieneJSONActualizacionPlantilla(ids));
                    var response = await cliente.LowLevel.Indices.PutMappingAsync<PutMappingResponse>(plantilla.Id, body);
                    if (!response.ApiCall.Success)
                    {
                        return false;

                    }                     
                }

            } else
            {
                Console.WriteLine(result.ApiCall.OriginalException.ToString());
                return false;
            }
            

            return true;
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






    }
}
