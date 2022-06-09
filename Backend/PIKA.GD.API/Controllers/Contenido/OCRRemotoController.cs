using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using PikaOCR.modelo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    [AllowAnonymous]
    public class OCRRemotoController : ControllerBase
    {
        private readonly ILogger<OCRRemotoController> _logger;
        private readonly IConfiguration _configuracion;
        private readonly IServicioVolumen _volumenes;
        private readonly IServicioElemento _elementos;
        private readonly IRepoContenidoElasticSearch _repoElastic;
        private readonly IOptions<ConfiguracionServidor> _opciones;

        public OCRRemotoController(
            ILogger<OCRRemotoController> logger,
             IConfiguration configuracion,
             IServicioVolumen volumenes,
             IRepoContenidoElasticSearch repoElastic,
             IServicioElemento elementos,
             IOptions<ConfiguracionServidor> opciones)
        {
            _opciones = opciones;
            _logger = logger;
            _configuracion = configuracion;
            _volumenes = volumenes;
            _repoElastic = repoElastic;
            _elementos = elementos;
        }


        #region Funciones de soporte
        private ContenidoTextoCompleto ParteAContenidoTextoCompleto(string ElementoId, string ParteId, string VolumenId, string VersionId, string Texto, string PuntoMontajeId = "", string CarpetaId = "", int Pagina = 1)
        {
            return new ContenidoTextoCompleto()
            {
                Texto = Texto,
                ElementoId = ElementoId,
                Eliminado = false,
                Activo = true,
                ParteId = ParteId,
                Pagina = Pagina,
                VolumenId = VolumenId,
                CarpetaId = CarpetaId,
                PuntoMontajeId = PuntoMontajeId,
                VersionId = VersionId,
                DocumentoId = ElementoId
            };
        }

        private TrabajoOCRRemoto ATrabajo(Version v, Elemento elemento)
        {

            TrabajoOCRRemoto remoto = new TrabajoOCRRemoto()
            {
                ElementoId = v.ElementoId,
                VersionId = v.Id,
                VolumenId = v.VolumenId,
                PuntoMontajeId = elemento.PuntoMontajeId,
                CarpetaId = elemento.CarpetaId
            };

            v.Partes.ForEach(p =>
            {
                remoto.Paginas.Add(new PaginaOCRRemoto()
                {
                    ConsecutivoVolumen = p.ConsecutivoVolumen,
                    EsImagen = p.EsImagen,
                    EsPDF = p.EsPDF,
                    Extension = p.Extension,
                    Id = p.Id,
                    Indice = p.Indice
                });

            });

            return remoto;
        }
        #endregion

        /// <summary>
        /// Onbtiene el sieguiente tarbajo disponible
        /// </summary>
        /// <returns></returns>
        [HttpGet("{idprocesador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TrabajoOCRRemoto>> ObtieneTrabajo(string idprocesador)
        {
            var siguiente = await _repoElastic.SiguenteIndexar(null,true, idprocesador).ConfigureAwait(false);

            if (siguiente != null)
            {
                
                Elemento e = await _elementos.UnicoAsync(x => x.Id == siguiente.ElementoId).ConfigureAwait(false);
                if (siguiente.Partes == null)
                {
                    siguiente.Partes = new List<Parte>();
                }
                var trabajo = ATrabajo(siguiente, e);
                trabajo.ProcesadorId = idprocesador;
                await _repoElastic.EliminaOCRVersion(siguiente).ConfigureAwait(false);
                return Ok(trabajo);
            } else
            {
                return NotFound();
            }
        }


        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EliminaTrabajo(FinTrabajoRemoto fintrabajo)
        {
            bool ok = await _repoElastic.ActualizaEstadoOCR(fintrabajo.VersionId, fintrabajo.ProcesadorId, EstadoIndexado.PorIndexar ).ConfigureAwait(false);
            if (ok)
            {
                return Ok();

            } else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Obtiene el contenido asociado a la página como un arreglo de bytes
        /// </summary>
        /// <param name="volumenid"></param>
        /// <param name="elementoid"></param>
        /// <param name="versionid"></param>
        /// <param name="paginaid"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        [HttpGet("bytes/{volumenid}/{elementoid}/{versionid}/{paginaid}/{extension}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<byte[]>> ObtieneBytesContenido(string volumenid, string elementoid, string versionid, string paginaid, string extension)
        {
            Volumen v = await _volumenes.UnicoAsync(v => v.Id == volumenid).ConfigureAwait(false);
            IGestorES gestor = await _volumenes.ObtienInstanciaGestor(v.Id).ConfigureAwait(false);
            var bytes = await gestor.LeeBytes(elementoid, paginaid, versionid, volumenid, extension).ConfigureAwait(false);

            if(bytes!= null && bytes.Length > 0)
            {

                return Ok(bytes);

            } else
            {
                return NotFound();
            }
            //return new FileContentResult(bytes, "application/octet-stream")
            //{
            //    FileDownloadName = $"{paginaid}{extension}"
            //};
        }

        [HttpGet("estadoocr", Name = "ObtieneEstadoOCRRemoto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<EstadoOCR>> ObtieneEstadoOCR()
        {
            var data = await _repoElastic.OntieneEstadoOCR().ConfigureAwait(false);
            return Ok(data);
        }

        /// <summary>
        /// Actualiza el OCR para un conjunto de páginas
        /// </summary>
        /// <param name="trabajo"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizaPaginas([FromBody] TrabajoOCRRemoto trabajo)
        {
            var v = await _repoElastic.ObtieneVersion(trabajo.VersionId).ConfigureAwait(false);
            if(v.IdPRocesadorOCR != trabajo.ProcesadorId)
            {
                return Forbid();
            }

            if (v != null)
            {
                foreach (var p in trabajo.Paginas)
                {

                    foreach (var ocr in p.Resultados)
                    {
                        if(!string.IsNullOrEmpty( ocr.Texto))
                        {
                            string idPaginaExistente = await _repoElastic.ExisteTextoCompleto(new ContenidoTextoCompleto()
                            {
                                ElementoId = trabajo.ElementoId,
                                ParteId = p.Id,
                                VersionId = trabajo.VersionId,
                                Pagina = ocr.Pagina
                            }).ConfigureAwait(false);
                            if (string.IsNullOrEmpty(idPaginaExistente))
                            {
                                await _repoElastic.IndexarTextoCompleto(
                                    ParteAContenidoTextoCompleto(trabajo.ElementoId, p.Id, trabajo.VolumenId, trabajo.VersionId,
                                    ocr.Texto, trabajo.PuntoMontajeId, trabajo.CarpetaId, ocr.Pagina)
                                    ).ConfigureAwait(false);

                            }
                            else
                            {
                                await _repoElastic.ActualizarTextoCompleto(idPaginaExistente,
                                    ParteAContenidoTextoCompleto(trabajo.ElementoId, p.Id, trabajo.VolumenId, trabajo.VersionId,
                                    ocr.Texto, trabajo.PuntoMontajeId, trabajo.CarpetaId, ocr.Pagina)
                                    ).ConfigureAwait(false);
                            }
                        }
                     
                    }
                    var  parte = v.Partes.Where(x => x.Id == p.Id).FirstOrDefault();
                    if (parte != null)
                    {
                        parte.Indexada = true;
                    }
                }

                await _repoElastic.ActualizaEstadoOCR(trabajo.VersionId, v).ConfigureAwait(false);
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }


        /// <summary>
        /// Finaliza un trabajo de OCR remoto
        /// </summary>
        /// <param name="trabajo"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> FinalizaTrabajo([FromBody] FinTrabajoRemoto trabajo)
        {
            var v = await _repoElastic.ObtieneVersion(trabajo.VersionId).ConfigureAwait(false);

            if (v.IdPRocesadorOCR != trabajo.ProcesadorId)
            {
                return Forbid();
            }

            if (v != null)
            {
                v.EstadoIndexado = trabajo.Ok ? EstadoIndexado.FinalizadoOK : EstadoIndexado.FinalizadoError;
                await _repoElastic.ActualizaEstadoOCR(trabajo.VersionId, v).ConfigureAwait(false); 

            } else
            {
                return NotFound();
            }
            
            return Ok();
        }

    }
}
