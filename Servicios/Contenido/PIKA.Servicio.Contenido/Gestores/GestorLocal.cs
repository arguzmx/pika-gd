using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Gestores
{


    public class GestorLocal : GestorBase, IGestorES
    {
        private GestorLocalConfig configGestor;
        private ConfiguracionServidor configServidor;
        private ILogger logger;


        /// <summary>
        /// Constructur para validación de la conexión
        /// </summary>
        /// <param name="configGestor"></param>
        /// <param name="volumen"></param>
        /// <param name="opciones"></param>
        public GestorLocal(
            ILogger logger,
            GestorLocalConfig configGestor,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.logger = logger;
            this.configGestor = configGestor;
            this.configServidor = opciones.Value;
        }

        public bool ConexionValida()
        {
            try
            {
                string[] l = Directory.GetFiles(this.configGestor.Ruta);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<byte[]> LeeBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string nombreArchivo = ParteId + Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            return LeeArchivo(rutaFinal);
        }



        public Task<byte[]> LeeThumbnailBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId, "thumbnails");
            string nombreArchivo = ParteId + ".PNG";
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            return LeeArchivo(rutaFinal);
        }


        private async Task<byte[]> LeeArchivo(string ruta)
        {
            if (File.Exists(ruta)) return (await File.ReadAllBytesAsync(ruta));
            return null;
        }

        /// <summary>
        /// Crea la miniatura a prtir de una imagen
        /// </summary>
        /// <param name="rutaMinuaturas"></param>
        /// <param name="imagenFuente"></param>
        /// <param name="tamanoCuadro"></param>
        /// <param name="ParteId"></param>
        private void CreaMiniatura(string rutaMinuaturas, 
         string imagenFuente, int tamanoCuadro, string ParteId)
        {
            using (Image image = Image.Load(imagenFuente))
            {
                if(image.Width > image.Height)
                {
                    int h = (int)((((float)image.Height) / ((float)image.Width)) * tamanoCuadro);
                    // Landscape
                    image.Mutate(x => x.Resize(tamanoCuadro, h));

                } else
                {
                    //Portrait
                    int w = (int)((((float)image.Width) / ((float)image.Height)) * tamanoCuadro);
                    // Landscape
                    image.Mutate(x => x.Resize(w, tamanoCuadro));

                }

                string nombreArchivo = ParteId + ".PNG";
                string rutaFinal = Path.Combine(rutaMinuaturas, nombreArchivo);
                image.Save(rutaFinal); 
            }
        }

        /// <summary>
        /// Escribe los bytes desde un arreglo de bytes
        /// </summary>
        /// <param name="ParteId"></param>
        /// <param name="ElementoId"></param>
        /// <param name="VersionId"></param>
        /// <param name="contenido"></param>
        /// <param name="informacion"></param>
        /// <param name="sobreescribir"></param>
        /// <returns></returns>
        public async Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido, FileInfo informacion, bool sobreescribir)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string rutaMiniaturas = Path.Combine(ruta, "thumbnails");

            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            if (!Directory.Exists(rutaMiniaturas)) Directory.CreateDirectory(rutaMiniaturas);

            string nombreArchivo = ParteId + informacion.Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);

            if (File.Exists(rutaFinal))
            {
                if (sobreescribir)
                {
                    File.Delete(rutaFinal);
                } else
                {
                    throw new Exception("Archivo existente");
                }
            }

            await File.WriteAllBytesAsync(rutaFinal, contenido);

            if (EsImagen(informacion))
            {
                CreaMiniatura(rutaMiniaturas, rutaFinal, 200, ParteId);
            }
            return informacion.Length;
           
        }


        /// <summary>
        /// Escribe los bytes desde la copia de un archivo existente
        /// </summary>
        /// <param name="ParteId"></param>
        /// <param name="ElementoId"></param>
        /// <param name="VersionId"></param>
        /// <param name="archivoFuente"></param>
        /// <param name="informacion"></param>
        /// <param name="sobreescribir"></param>
        /// <returns></returns>
        public async Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, string archivoFuente, FileInfo informacion, bool sobreescribir)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string rutaMiniaturas = Path.Combine(ruta, "thumbnails");

            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            if (!Directory.Exists(rutaMiniaturas)) Directory.CreateDirectory(rutaMiniaturas);

            string nombreArchivo = ParteId + informacion.Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);

            if (File.Exists(rutaFinal))
            {
                if (sobreescribir)
                {
                    File.Delete(rutaFinal);
                }
                else
                {
                    throw new Exception("Archivo existente");
                }
            }

            await Task.Delay(1);
            File.Copy(archivoFuente, rutaFinal);

            if (EsImagen(informacion))
            {
                CreaMiniatura(rutaMiniaturas, rutaFinal, 200, ParteId);
            }
            return 1;
        }
    }
}
