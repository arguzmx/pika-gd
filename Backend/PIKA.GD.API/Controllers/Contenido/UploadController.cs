using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.Extensiones;
using PIKA.Modelo.Contenido.ui;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using shortid;
using shortid.Configuration;

namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadController : ACLController
    {
        private ILogger<UploadController> logger;
        private IServicioVolumen servicioVol;
        private ConfiguracionServidor configuracionServidor;
        private IServicioElementoTransaccionCarga servicioTransaccionCarga;
        private IRepoContenidoElasticSearch repoContenido;
        private IServicioElemento servicioElemento;
        public UploadController(
            IRepoContenidoElasticSearch repoContenido,
            IServicioElementoTransaccionCarga servicioTransaccionCarga,
            IServicioElemento servicioElemento,
            ILogger<UploadController> logger,
            IServicioVolumen servicioVol,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.repoContenido = repoContenido;
            this.servicioTransaccionCarga = servicioTransaccionCarga;
            this.servicioVol = servicioVol;
            this.logger = logger;
            this.servicioElemento = servicioElemento;
            this.configuracionServidor = opciones.Value;
            FiltroArchivos.minimo = 1; //1 KB
            FiltroArchivos.maximo = long.MaxValue; //10 MB
            FiltroArchivos.addExt(".jpg");
            FiltroArchivos.addExt(".pdf");
            FiltroArchivos.addExt(".doc");
            FiltroArchivos.addExt(".docx");
            FiltroArchivos.addExt(".xls");
            FiltroArchivos.addExt(".xlsx");
            FiltroArchivos.addExt(".ppt");
            FiltroArchivos.addExt(".pptx");
            FiltroArchivos.addExt(".mp4");
            FiltroArchivos.addExt(".mp3");
            FiltroArchivos.addExt(".mov");
            FiltroArchivos.addExt(".avi");
            FiltroArchivos.addExt(".wmv");
            FiltroArchivos.addExt(".wav");
            FiltroArchivos.addExt(".ogg");

        }


        /// <summary>
        /// Añade las nuevas páginas y devulve la lista de páginas actualizadas
        /// </summary>
        /// <param name="TransaccionId"></param>
        /// <returns></returns>
        [RequestSizeLimit(long.MaxValue)]
        [HttpPost("completar/{TransaccionId}")]
        public async Task<ActionResult<List<Pagina>>> FinalizarLote(string TransaccionId)
        {
            await repoContenido.CreaRepositorio().ConfigureAwait(false);
            string ruta = Path.Combine(configuracionServidor.ruta_cache_fisico, TransaccionId);

            try
            {

                var elementos = await this.servicioTransaccionCarga.OtieneElementosTransaccion(TransaccionId).ConfigureAwait(false);

                if (elementos.Count > 0)
                {
                    string elementoId = elementos[0].ElementoId;
                    string VolId = elementos[0].VolumenId;
                    string version = elementos[0].VersionId;
                    int PosicionInicio = elementos[0].PosicionInicio;
                    PosicionCarga Posicion = elementos[0].Posicion;

                    long conteoBytes = 0;
                    IGestorES gestor = await servicioVol.ObtienInstanciaGestor(VolId)
                           .ConfigureAwait(false);
                    
                    if(gestor==null)
                    {
                        return BadRequest("Gestor de volumen no válido");
                    }

                    Modelo.Contenido.Version v = await this.repoContenido.ObtieneVersion(version).ConfigureAwait(false);
                    if (v == null)
                    {

                        // Sustituir esta sección por el almacenamiento en elasticsearc
                        v = new Modelo.Contenido.Version()
                        {
                            Id = version,
                            Activa = true,
                            CreadorId = this.GetUserId(),
                            ElementoId = elementoId,
                            Eliminada = false,
                            FechaCreacion = DateTime.UtcNow,
                            VolumenId = VolId,
                            ConteoPartes = 0,
                            MaxIndicePartes = 0,
                            TamanoBytes = 0, 
                            EstadoIndexado = EstadoIndexado.PorIndexar,
                        };

                        var id = await this.repoContenido.CreaVersion(v).ConfigureAwait(false);
                    }

                    int indice = 1;
                    int inicio = 1;
                    if (v.Partes == null)
                    {
                        v.Partes = new List<Parte>();
                    } else
                    {
                        v.Partes = v.Partes.OrderBy(p => p.Indice).ToList();
                    }
       
                    switch (Posicion)
                    {
                        case PosicionCarga.al_inicio:
                            // recorre todos los indices en base al número de archivos intertados
                            inicio = elementos.Count + 1;
                            indice = 1;
                            for (int i=0; i< v.Partes.Count;i++)
                            {
                                v.Partes[i].Indice = inicio + i;
                            }
                            break;


                        case PosicionCarga.en_posicion:
                            // crea un intervalo recorre los elementos a partir de la posición inicial N espacios
                            inicio = PosicionInicio + elementos.Count + 1;
                            indice = PosicionInicio;
                            for (int i = 0; i < v.Partes.Count; i++)
                            {
                                if(v.Partes[i].Indice >= PosicionInicio)
                                {
                                    v.Partes[i].Indice = v.Partes[i].Indice + PosicionInicio;
                                } 
                            }
                            break;

                        // case PosicionCarga.al_final:
                        // la posicion por default es al final 
                        default:
                            // incrementa el índice al final de los elementos
                            indice = v.Partes.Count + 1;
                            for (int i = 0; i < v.Partes.Count; i++)
                            {
                                v.Partes[i].Indice = i + 1;
                            }
                            break;
                    }

                    var options = new GenerationOptions
                    {
                        UseNumbers = true, 
                        UseSpecialCharacters = false, 
                        Length =8
                    };

                    List<Parte> partes = new List<Parte>();
                    foreach (var el in elementos)
                    {
                        Parte p = el.ConvierteParte();
                        p.Indice = indice;
                        string filePath = Path.Combine(ruta, p.Id + Path.GetExtension(p.NombreOriginal));

                        if (System.IO.File.Exists(filePath))
                        {
                            FileInfo fi = new FileInfo(filePath);
                            
                            p.LongitudBytes = fi.Length;
          
                            p.Id = ShortId.Generate(options).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
                            while  (v.Partes.Any(x=>x.Id == p.Id) ||  partes.Any(x=>x.Id == p.Id))
                            {
                                p.Id = ShortId.Generate(options).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
                            }
                            
                            partes.Add(p);

                            await gestor.EscribeBytes(p.Id, p.ElementoId, p.VersionId, filePath, fi, false).ConfigureAwait(false);
                            
                            conteoBytes += p.LongitudBytes;
                            indice++;
                        }
                    }


                    v.TamanoBytes = v.Partes.Sum(x => x.LongitudBytes);
                    v.ConteoPartes = v.Partes.Count;
                    v.MaxIndicePartes = v.Partes.Count - 1;
                    v.Partes.AddRange(partes);

                    await this.repoContenido.ActualizaVersion(v.Id, v, true).ConfigureAwait(false);
                    await this.servicioElemento.ActualizaConteoPartes(elementoId, v.Partes.Count).ConfigureAwait(false);
                    await this.servicioElemento.ActualizaTamanoBytes(elementoId, v.Partes.Sum(x=>x.LongitudBytes)).ConfigureAwait(false);

                    try
                    {
                        Directory.Delete(ruta, true);
                    }
                    catch (Exception ex) {
                    }

                    await this.servicioTransaccionCarga.EliminarTransaccion(TransaccionId, VolId, conteoBytes).ConfigureAwait(false);
                    return Ok(v.Partes.APaginas());
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return BadRequest($"{TransaccionId}");
        }


        [HttpPost()]
        public async Task<IActionResult> PostContenido([FromForm] ElementoCargaContenido model)
        {
            try
            {
                var entradaCarga = model.ConvierteETC();

                var entrada = await servicioTransaccionCarga.CrearAsync(entradaCarga).ConfigureAwait(false);

                string ruta = Path.Combine(configuracionServidor.ruta_cache_fisico, model.TransaccionId);
                if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
                string filePath = Path.Combine(ruta, entrada.Id + Path.GetExtension(model.file.FileName));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.file.CopyToAsync(stream).ConfigureAwait(false);
                }

                if (System.IO.File.Exists(filePath))
                {
                    return Ok();

                } else
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Error de escritura");
                }

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);

            }
           

            //if (tamanoValido(model.file.Length, FiltroArchivos.minimo, FiltroArchivos.maximo))
            //{
            //    if (extensionValida(model.file, FiltroArchivos.extensionesValidas))
            //    {



            //    }
            //    else
            //        ModelState.AddModelError(model.file.Name, "extensión inválida");
            //}
            //else
            //{
            //    ModelState.AddModelError(model.file.Name, "tamaño inválido");
            //}

            //return BadRequest(ModelState);
        }

        private bool extensionValida(IFormFile file, ICollection<string> extensionesV)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !extensionesV.Contains(ext)) return false;
            if (!firmaValida(file.OpenReadStream(), ext)) return false;
            return true;
        }
        private bool firmaValida(Stream file, string ext)
        {
            bool valida = false;
            var firmasValidas = FiltroArchivos.firmasArchivo;
            using (var reader = new BinaryReader(file))
            {
                var signatures = firmasValidas[ext] != null ? firmasValidas[ext] : null;
                if (signatures != null)
                {
                    var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                    valida = signatures.Any(signature =>
                             headerBytes.Take(signature.Length).SequenceEqual(signature));
                }
            }
            return valida;
        }
        private bool tamanoValido(long size, long min, long max)
        {
            if (size > 0)
                if (size > min && size <= max)
                    return true;
            return false;
        }

    }
}

