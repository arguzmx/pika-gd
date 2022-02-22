using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using Microsoft.Extensions.Logging;
using System.Linq;
using PIKA.Modelo.Metadatos.Instancias;
using PIKA.Infraestructura.Comun.Excepciones;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public partial class RepoMetadatosElasticSearch : IRepositorioMetadatos, IRepositorioInicializableAutoConfigurable
    {

        public async Task<bool> EliminaVinculoLista(string listaId, string plantillaId)
        {
            bool resultado = false;
            var searchResults = await this.clienteVinculos.SearchAsync<VinculosObjetoPlantilla>(s => s
                        .Query(q => q
                            .Nested(c => c
                            .Name("listaporid")
                            .InnerHits(i => i.Explain())
                            .Path(p => p.Listas)
                            .Query(nq => nq
                                .Match(t => t
                                    .Field(f => f.Listas.First().ListaId)
                                    .Query(listaId)
                                )
                            )
                            .IgnoreUnmapped()
                        )
                        )
                    );

            if (searchResults.ApiCall.Success)
            {
                if (searchResults.Total > 0)
                {
                    var v = searchResults.Documents.ToList()[0];
                    var l = v.Listas.Where(x => x.ListaId == listaId).FirstOrDefault();
                    if (l != null)
                    {
                        v.Listas.Remove(l);
                        if ((v.Documentos.Count + v.Listas.Count) > 0)
                        {
                            resultado = await ActualizaVinculos(v);
                        }
                        else
                        {
                            resultado = await EliminaVinculos(v);
                        }
                    }
                }
                else
                {
                    throw new EXNoEncontrado(listaId);
                }
            }

            return resultado;
        }

        public async Task<bool> EliminaVinculoDocumento(string tipo, string id, string documentoId)
        {
            bool resultado = false;
            var searchResults = await this.clienteVinculos.SearchAsync<VinculosObjetoPlantilla>(s => s
                        .Query(q => q
                             .Bool(b => b
                                .Must(mu => mu
                                   .Match(m => m
                                      .Field(f => f.Id)
                                      .Query(id)
                                   ),
                                    mu => mu
                                    .Match(m => m
                                       .Field(f => f.Tipo)
                                       .Query(tipo)
                                    )
                                )
                            )
                        )
                        );


            if (searchResults.ApiCall.Success)
            {
                if (searchResults.Total > 0)
                {
                    var v = searchResults.Documents.ToList()[0];
                    var l = v.Documentos.Where(x => x.DocumentoId == documentoId).FirstOrDefault();
                    if (l != null)
                    {
                        v.Documentos.Remove(l);
                        if ((v.Documentos.Count + v.Listas.Count) > 0)
                        {
                            resultado = await ActualizaVinculos(v);
                        }
                        else
                        {
                            resultado = await EliminaVinculos(v);
                        }
                    }
                }
                else
                {
                    throw new EXNoEncontrado(documentoId);
                }
            }

            return resultado;
        }


        public async Task<bool> CrearVinculoLista(string tipo, string id, string listaId,
            string plantillaid, string nombreRelacion)
        {
            bool resultado = false;
            VinculosObjetoPlantilla v = await ObtieneVinculos(tipo, id);
            if (v == null)
            {
                v = new VinculosObjetoPlantilla()
                {
                    _Id = Guid.NewGuid().ToString(),
                    Documentos = new List<VinculoDocumentoPlantilla>(),
                    Id = id,
                    Tipo = tipo,
                    Listas = new List<VinculoListaPlantilla>() {
                        new VinculoListaPlantilla(){ ListaId = listaId, Nombre = nombreRelacion, PlantillaId = plantillaid}
                    }
                };

                var indexResponse = await clienteVinculos.IndexDocumentAsync(v);
                resultado = (indexResponse.Result == Result.Created);

            }
            else
            {
                if (!v.Listas.Any(x => x.ListaId == listaId))
                {
                    v.Listas.Add(
                        new VinculoListaPlantilla()
                        {
                            ListaId = listaId,
                            Nombre = nombreRelacion,
                            PlantillaId = plantillaid
                        });

                    resultado = await ActualizaVinculos(v);
                }
                else
                {
                    resultado = true;
                }

            }
            return resultado;
        }

        public async Task<bool> CrearVinculoDocumento(string tipo, string id, string documentoid,
            string plantillaid, string nombreRelacion)
        {
            bool resultado = false;
            VinculosObjetoPlantilla v = await ObtieneVinculos(tipo, id);
            if (v == null)
            {
                v = new VinculosObjetoPlantilla()
                {
                    _Id = Guid.NewGuid().ToString(),
                    Documentos = new List<VinculoDocumentoPlantilla>() {
                    new VinculoDocumentoPlantilla( ) { DocumentoId = documentoid, PlantillaId = plantillaid, Nombre = nombreRelacion}
                    },
                    Id = id,
                    Tipo = tipo,
                    Listas = new List<VinculoListaPlantilla>()
                };
                var indexResponse = await clienteVinculos.IndexDocumentAsync(v);
                resultado = (indexResponse.Result == Result.Created);

            }
            else
            {
                v.Documentos.Add(new VinculoDocumentoPlantilla()
                {
                    DocumentoId = documentoid,
                    PlantillaId = plantillaid,
                    Nombre = nombreRelacion
                });
                resultado = await ActualizaVinculos(v);
            }
            return resultado;
        }


        public async Task<bool> ActualizaVinculos(VinculosObjetoPlantilla v)
        {
            try
            {
                var response = await clienteVinculos.UpdateAsync<VinculosObjetoPlantilla>(v.Id, u => u.Doc(v));
                return response.Result == Result.Updated;
            }
            catch (Exception ex)
            {

                throw;
            }
            

        }

        public async Task<bool> EliminaVinculos(VinculosObjetoPlantilla v)
        {
            var response = await clienteVinculos.DeleteAsync<VinculosObjetoPlantilla>(v._Id);
            return response.Result == Result.Updated;
        }

        public async Task<VinculosObjetoPlantilla> ObtieneVinculos(string tipo, string id)
        {
            var r = await clienteVinculos.SearchAsync<VinculosObjetoPlantilla>(s => s
                    .Query(q => q
                        .Bool(b => b
                            .Must(
                                bs => bs.Term(p => p.Tipo, tipo),
                                bs => bs.Term(p => p.Id, id)
                            )
                        )
                    )
                );

            if (r.Hits.Count > 0)
            {
                return r.Documents.ToList()[0];
            }

            return null;
        }

        private async Task VerificaRepositoiorVinculos()
        {
            if (!await ExisteIndice(NOMBREINDICEVINCULOS))
            {
                logger.LogInformation($"Creando repositorio de vinculos");
                var createIndexResponse = await clienteVinculos.Indices.CreateAsync(NOMBREINDICEVINCULOS, c => c
                    .Map<VinculosObjetoPlantilla>(m => m.AutoMap())
                );

                if (!createIndexResponse.ApiCall.Success)
                {
                    logger.LogError(createIndexResponse.DebugInformation);
                    logger.LogError(createIndexResponse.OriginalException.ToString());
                }
            }
            else
            {
                logger.LogInformation($"Repositorio de vinculos configurado");
            }
        }
    }
}
