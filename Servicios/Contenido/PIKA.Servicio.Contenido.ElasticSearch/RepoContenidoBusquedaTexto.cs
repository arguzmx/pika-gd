using Elasticsearch.Net;
using Nest;
using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.ElasticSearch
{
    public partial class RepoContenidoElasticSearch : IRepoContenidoElasticSearch
    {

        /// <summary>
        /// Elimina el OCR de una versión de contenido
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<bool> EliminaOCRVersion(Modelo.Contenido.Version version)
        {


            var filters = new List<Func<QueryContainerDescriptor<ContenidoTextoCompleto>, QueryContainer>>();
            filters.Add(fq => fq.Terms(t => t.Field(f => f.ElementoId).Terms(version.ElementoId)));
            filters.Add(fq => fq.Terms(t => t.Field(f => f.VersionId).Terms(version.Id)));

            var resultado = await cliente.DeleteByQueryAsync<ContenidoTextoCompleto>(x => x.Query(q => q
            .Bool(bq => bq.Filter(filters)))
            .Index(INDICECONTENIDO)
            );

            if (resultado.ApiCall.Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Eliminar el OCR de una parte del contenido
        /// </summary>
        /// <param name="IdParte"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<bool> EliminaOCR(string IdParte, Modelo.Contenido.Version version)
        {


            var filters = new List<Func<QueryContainerDescriptor<ContenidoTextoCompleto>, QueryContainer>>();
            filters.Add(fq => fq.Terms(t => t.Field(f => f.ParteId).Terms(IdParte)));
            filters.Add(fq => fq.Terms(t => t.Field(f => f.ElementoId).Terms(version.ElementoId)));
            filters.Add(fq => fq.Terms(t => t.Field(f => f.VersionId).Terms(version.Id)));

            var resultado = await cliente.DeleteByQueryAsync<ContenidoTextoCompleto>(x => x.Query(q => q
            .Bool(bq => bq.Filter(filters)))
            .Index(INDICECONTENIDO)
            );

            if (resultado.ApiCall.Success)
            {
                return true;
            }

            return false;
        }
        public async Task<EstadoOCR> OntieneEstadoOCR()
        {
            EstadoOCR estado = new EstadoOCR();
            
            
            var pendientesResponse = cliente.CountAsync<Modelo.Contenido.Version>(s =>
              s.Query(q =>
               q.Bool(b =>
                   b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                   Modelo.Contenido.EstadoIndexado.PorIndexar)))
           ));


            var completosResponse = cliente.CountAsync<Modelo.Contenido.Version>(s =>
              s.Query(q =>
               q.Bool(b =>
                   b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                   Modelo.Contenido.EstadoIndexado.FinalizadoOK)))
           ));

            var errorResponse = cliente.CountAsync<Modelo.Contenido.Version>(s =>
              s.Query(q =>
               q.Bool(b =>
                   b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                   Modelo.Contenido.EstadoIndexado.FinalizadoError)))
            ));


            List<Task> conteos = new List<Task>() { pendientesResponse, completosResponse, errorResponse };
            Task.WaitAll(conteos.ToArray());

            estado.Completo = completosResponse.Result.Count;
            estado.Pendiente = pendientesResponse.Result.Count;
            estado.Error = errorResponse.Result.Count;

            return estado;
        }

        public async Task ReiniciarOCRErroneos()
        {
          var data =  await  cliente.UpdateByQueryAsync<Modelo.Contenido.Version>(u => u
            .Query(q =>
                   q.Bool(b =>
                   b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                   Modelo.Contenido.EstadoIndexado.FinalizadoError)))
                )
            .Script("ctx._source.eidx = 0")
                .Conflicts(Conflicts.Proceed)
            .Refresh(true)
            );
        }

        public async Task<string> ExisteTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido)
        {

            var filters = new List<Func<QueryContainerDescriptor<ContenidoTextoCompleto>, QueryContainer>>();
            filters.Add(fq => fq.Terms(t => t.Field(f => f.ParteId).Terms(contenido.ParteId)));
            filters.Add(fq => fq.Terms(t => t.Field(f => f.ElementoId).Terms(contenido.ElementoId)));
            filters.Add(fq => fq.Terms(t => t.Field(f => f.VersionId).Terms(contenido.VersionId)));
            filters.Add(fq => fq.Terms(t => t.Field(f => f.Pagina).Terms(contenido.Pagina)));

            var response = await cliente.SearchAsync<ContenidoTextoCompleto>(x => x.Query(q => q
                    .Bool(bq => bq.Filter(filters)))
                    .Index(INDICECONTENIDO)
                );

            if (response.ApiCall.Success)
            {
                return response.Hits.Count > 0 ? response.Hits.ToList()[0].Id : null;
            }
            return null;
        }

        public async Task<bool> ActualizarTextoCompleto(string Id, Modelo.Contenido.ContenidoTextoCompleto contenido)
        {
            var resultado = await cliente.UpdateAsync<Modelo.Contenido.ContenidoTextoCompleto, object>(
                    new DocumentPath<Modelo.Contenido.ContenidoTextoCompleto>(Id),
                        u => u.Index(INDICECONTENIDO)
                        .DocAsUpsert(true)
                        .Doc(contenido)
                        .Refresh(Elasticsearch.Net.Refresh.True));

            if (resultado.Result == Result.Updated ||
                resultado.Result == Result.Noop)
            {
                return true;
            }

            return false;
        }

        public async Task<string> IndexarTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido)
        {
            var result = await clienteOCR.IndexDocumentAsync(contenido);
            if (result.ApiCall.Success)
            {
                return result.Id;
            }
            return null;
        }

        public async Task<List<HighlightHit>> SinopsisPorIDs(string texto, List<string> Ids, string HtmlTag, int Tamano)
        {

            List<HighlightHit> l = new List<HighlightHit>();
            var response = await cliente.SearchAsync<ContenidoTextoCompleto>(sd =>
                sd.Index("contenido-indexado")
                .Query(
                    q => q
                    .Bool(bq => bq
                      .Filter(f => f
                        .Terms(t => t
                            .Field(f => f.ElementoId)
                            .Terms(Ids)
                        )
                      )
                      .Must(mq => mq
                         .Match(m => m
                           .Field("t").Query(texto)
                           .Operator(Operator.And)
                           .Fuzziness(Fuzziness.Auto)
                         )

                      )
                    )
                 )
                .Highlight(h => h
                    .PreTags($"<{HtmlTag}>")
                    .PostTags($"</{HtmlTag}>")
                    .Encoder(HighlighterEncoder.Html)
                    .HighlightQuery(q => q
                        .Match(m => m
                                    .Field("t")
                                    .Query(texto)
                                    .Operator(Operator.And)
                                    .Fuzziness(Fuzziness.Auto)
                              )
                    ).Fields(
                         fs => fs
                            .Field("t")
                            .Type("plain")
                            .ForceSource()
                            .FragmentSize(Tamano)
                            .Fragmenter(HighlighterFragmenter.Span)
                            .NumberOfFragments(3)
                            .NoMatchSize(100)
                    )
                )
            );

            if (response.ApiCall.Success)
            {
                foreach (var highlightsInEachHit in response.Hits.Select(d => new { d.Highlight, d.Id, d.Source.ParteId, d.Source.Pagina, d.Source.ElementoId }))
                {
                    HighlightHit hh = l.Where(x => x.ElementoId == highlightsInEachHit.ElementoId).FirstOrDefault();
                    if (hh == null) hh = new HighlightHit() { ElasticId = highlightsInEachHit.Id, ElementoId = highlightsInEachHit.ElementoId };

                    foreach (var highlightField in highlightsInEachHit.Highlight)
                    {

                        foreach (var highlight in highlightField.Value)
                        {

                            if (hh.Highlights.Count <= 2 && !hh.Highlights.Any(x => x.Texto == highlight))
                            {
                                Highlight h = new Highlight()
                                {
                                    Pagina = highlightsInEachHit.Pagina,
                                    ParteId = highlightsInEachHit.ParteId,
                                    Texto = highlight
                                };
                                hh.Highlights.Add(h);
                            }
                        }
                    }

                    l.Add(hh);
                }
            }
            else
            {
                Console.WriteLine(response.ServerError.ToString());
            }

            return l;
        }


        public async Task<List<string>> IdsPorConsulta(string texto, string PuntoMontajeId, string IdJerarquico, int NivelFuzzy)
        {

            if (IdJerarquico == "") IdJerarquico = null;
            List<string> l = new List<string>();
            ISearchResponse<ContenidoTextoCompleto> response = null;
            if (NivelFuzzy == 0)
            {
                if (string.IsNullOrEmpty(IdJerarquico))
                {
                    response = await cliente.SearchAsync<ContenidoTextoCompleto>(sd =>
                               sd.Index(INDICECONTENIDO)
                               .Query(
                                   q => q
                                   .Bool(bq => bq
                                     .Filter(f => f
                                       .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                                     )
                                     .Must(mq => mq
                                        .Term(t => t
                                            .Field("t")
                                            .Value(texto)
                                        )
                                    )
                                   )
                                )
                           );

                }
                else
                {
                    response = await cliente.SearchAsync<ContenidoTextoCompleto>(sd =>
                               sd.Index(INDICECONTENIDO)
                               .Query(
                                   q => q
                                   .Bool(bq => bq
                                     .Filter(f => f
                                       .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                                     )
                                     .Filter(f => f
                                       .Term(t => t.CarpetaId, IdJerarquico)
                                     )
                                     .Must(mq => mq
                                        .Term(t => t
                                            .Field("t")
                                            .Value(texto)
                                        )
                                    )
                                   )
                                )
                           );

                }


            } else
            {

                if (string.IsNullOrEmpty(IdJerarquico))
                {
                    response = await cliente.SearchAsync<ContenidoTextoCompleto>(sd =>
                         sd.Index(INDICECONTENIDO)
                         .Query(
                             q => q
                             .Bool(bq => bq
                               .Filter(f => f
                                 .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                               )
                               .Must(mq => mq
                                  .Match(m => m
                                    .Field("t").Query(texto)
                                    .Operator(Operator.And)
                                    .Fuzziness(Fuzziness.Auto)
                                  )

                               )
                             )
                          )
                     );
                } else
                {
                    response = await cliente.SearchAsync<ContenidoTextoCompleto>(sd =>
                         sd.Index(INDICECONTENIDO)
                         .Query(
                             q => q
                             .Bool(bq => bq
                               .Filter(f => f
                                 .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                               )
                               .Filter(f => f
                                 .Term(t => t.CarpetaId, IdJerarquico)
                               )
                               .Must(mq => mq
                                  .Match(m => m
                                    .Field("t").Query(texto)
                                    .Operator(Operator.And)
                                    .Fuzziness(Fuzziness.Auto)
                                  )

                               )
                             )
                          )
                     );
                }
            }

            Console.WriteLine(System.Text.Encoding.Default.GetString(response.ApiCall.RequestBodyInBytes));

            if (response.ApiCall.Success)
            {
                response.Hits.ToList().ForEach(h =>
                {
                    if (!l.Any(x => x.StartsWith(h.Source.ElementoId)))
                    {
                        if (l.IndexOf($"{h.Source.ElementoId}|{h.Id}") < 0)
                        {
                            l.Add($"{h.Source.ElementoId}|{h.Id}");
                        }
                    }
                });
            }


            return l;
        }

        public async Task<long> ContarPorConsulta(string texto, string PuntoMontajeId, string IdJerarquico, int NivelFuzzy = 0)
        {

            if (IdJerarquico == "") IdJerarquico = null;
            CountResponse response = null;
            Fuzziness f = new Fuzziness();

            switch (NivelFuzzy)
            {
                case 0:
                    f = Fuzziness.EditDistance(0);
                    break;

                default:
                    f = Fuzziness.Auto;
                    break;

            }
            if (NivelFuzzy == 0)
            {

                if(string.IsNullOrEmpty(IdJerarquico))
                {
                    response = await cliente.CountAsync<ContenidoTextoCompleto>(sd =>
                    sd.Index(INDICECONTENIDO)
                   .Query(
                        q => q
                        .Bool(bq => bq
                            .Must(mq => mq
                                .Term(t => t
                                    .Field("t")
                                    .Value(texto)
                                )
                            )
                            .Filter(fq => fq
                                .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                            )
                         )
                    )
                    );
                }
                else
                {
                    response = await cliente.CountAsync<ContenidoTextoCompleto>(sd =>
                    sd.Index(INDICECONTENIDO)
                   .Query(
                        q => q
                        .Bool(bq => bq
                            .Must(mq => mq
                                .Term(t => t
                                    .Field("t")
                                    .Value(texto)
                                )
                            )
                            .Filter(fq => fq
                                .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                            )
                            .Filter(f => f
                                .Term(t => t.CarpetaId, IdJerarquico)
                            )
                         )

                    )
                    );
                }

            }
            else
            {
                if (string.IsNullOrEmpty(IdJerarquico))
                {
                    response = await cliente.CountAsync<ContenidoTextoCompleto>(sd =>
                        sd.Index(INDICECONTENIDO)
                       .Query(
                            q => q
                            .Bool(bq => bq
                                .Must(mq => mq
                                    .Match(m => m
                                   .Field("t").Query(texto)
                                   .Operator(Operator.And)
                                   .Fuzziness(f)
                                 )
                                )
                                .Filter(fq => fq
                                    .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                                )
                             )

                        )
                    );
                }
                else
                {
                    response = await cliente.CountAsync<ContenidoTextoCompleto>(sd =>
                        sd.Index(INDICECONTENIDO)
                       .Query(
                            q => q
                            .Bool(bq => bq
                                .Must(mq => mq
                                    .Match(m => m
                                   .Field("t").Query(texto)
                                   .Operator(Operator.And)
                                   .Fuzziness(f)
                                 )
                                )
                                .Filter(fq => fq
                                    .Term(t => t.PuntoMontajeId, PuntoMontajeId)
                                )
                                .Filter(f => f
                                    .Term(t => t.CarpetaId, IdJerarquico)
                                )
                             )

                        )
                    );
                }

            }
            Console.WriteLine(System.Text.Encoding.Default.GetString(response.ApiCall.RequestBodyInBytes));
            if (response.ApiCall.Success)
            {
                return response.Count;
            }


            return 0;
        }


        public async Task<long> IndexadoPendiente(string volumenId)
        {
            var searchResponse = await cliente.CountAsync<Modelo.Contenido.Version>(s =>
               s.Query(q =>
                q.Bool(b =>
                    b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                    Modelo.Contenido.EstadoIndexado.PorIndexar)))
            ));

            if (searchResponse.ApiCall.Success)
            {
                return searchResponse.Count;
            }

            return -1;
        }


        public async Task<bool> ActualizaEstadoOCR(string Id, Modelo.Contenido.Version version)
        {

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
        public async Task<Modelo.Contenido.Version> SiguenteIndexar(string volumenId, bool EstablacerEnproceso, string IdProcesador)
        {
            var searchResponse = await cliente.SearchAsync<Modelo.Contenido.Version>(s =>
            s.Size(1)
            .Query(q =>
              q.Bool(b =>
                  b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                  Modelo.Contenido.EstadoIndexado.PorIndexar)))
          ));

            if (searchResponse.IsValid)
            {
                var doc = searchResponse.Documents.FirstOrDefault();
                if (doc!=null && EstablacerEnproceso)
                {

                    doc.EstadoIndexado = EstadoIndexado.EnProceso;
                    doc.IdPRocesadorOCR = IdProcesador;
                    doc.FechaOCR = DateTime.UtcNow;
                    await ActualizaVersion(doc.Id, doc, false);
                }
                return doc;
            }
            return null;
        }


        public async Task<bool> ActualizaEstadoOCR(string Id, string ProcesadorId, EstadoIndexado Estado)
        {
            var version = await ObtieneVersion(Id);
            if (version != null)
            {
                if(version.IdPRocesadorOCR == ProcesadorId)
                {
                    version.EstadoIndexado = Estado;
                    await ActualizaVersion(Id, version, false);
                    return true;
                }
            }
            return false;
        }

    }
}
