using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ImageMagick;

namespace PIKA.Servicio.Contenido.Gestores
{
    public class GestorLaserfiche : GestorBase, IGestorES
    {
        private GestorLaserficheConfig configGestor;
        private ConfiguracionServidor configServidor;
        private ILogger logger;

        public bool AlmacenaOCR { get => true; }
        public bool UtilizaIdentificadorExterno { get => true; }

        /// <summary>
        /// Constructur para validación de la conexión
        /// </summary>
        /// <param name="configGestor"></param>
        /// <param name="volumen"></param>
        /// <param name="opciones"></param>
        public GestorLaserfiche(
            ILogger logger,
            GestorLaserficheConfig configGestor,
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


        private string MiniPorId(string ParteId)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            List<string> partes = Split(hex, 2).ToList();
            string ruta = Path.Combine(this.configGestor.Ruta, partes[0], partes[1], partes[2]);
            return Path.Combine(ruta, $"{hex}.PNG").ToUpper();
        }

        private string RutaPorId(string ParteId, string Extension)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            List<string> partes = Split(hex, 2).ToList();
            string prefijo = Extension.Contains("tif", StringComparison.InvariantCultureIgnoreCase) ? "" : "e";
            string ruta = Path.Combine(this.configGestor.Ruta, $"{prefijo}{partes[0]}", partes[1], partes[2]);
            return Path.Combine(ruta, $"{hex}{ (prefijo=="e"? "" : Extension)}").ToUpper();
        }

        private string RutaPorIdJpeg(string ParteId, string Extension)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            List<string> partes = Split(hex, 2).ToList();
            string ruta = Path.Combine(this.configGestor.Ruta, $"{partes[0]}", partes[1], partes[2]);
            return Path.Combine(ruta, $"{hex}{Extension}".ToUpper());
        }

        private string Ruta(string ParteId, string Extension)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            List<string> partes = Split(hex, 2).ToList();
            string prefijo = Extension.Contains("tif", StringComparison.InvariantCultureIgnoreCase) ? "" : "e";
            return Path.Combine(this.configGestor.Ruta, $"{prefijo}{partes[0]}", partes[1], partes[2]).ToUpper();
        }

        public async Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido, FileInfo informacion, bool sobreescribir)
        {
            string rutaFinal = RutaPorId(ParteId, informacion.Extension);

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

            await File.WriteAllBytesAsync(rutaFinal, contenido);

            if (EsImagen(informacion))
            {
                CreaMiniatura(rutaFinal, 200, ParteId); 
            }
            
            return contenido.Length;
        }

        public async Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, string archivoFuente, FileInfo informacion, bool sobreescribir)
        {
            string rutaFinal = RutaPorId(ParteId, informacion.Extension);

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
                CreaMiniatura(rutaFinal, 200, ParteId);
            }

            return informacion.Length;

        }

        /// <summary>
        /// Crea la miniatura a prtir de una imagen
        /// </summary>
        /// <param name="imagenFuente"></param>
        /// <param name="tamanoCuadro"></param>
        /// <param name="ParteId"></param>
        private void CreaMiniatura(string imagenFuente, int tamanoCuadro, string ParteId)
        {
            string rutaMini = MiniPorId(ParteId);

            try
            {
                using (var image = new MagickImage(imagenFuente))
                {

                    if (image.Width > image.Height)
                    {
                        int h = (int)((((float)image.Height) / ((float)image.Width)) * tamanoCuadro);
                        // Landscape
                        image.Resize(tamanoCuadro, h);

                    }
                    else
                    {
                        //Portrait
                        int w = (int)((((float)image.Width) / ((float)image.Height)) * tamanoCuadro);
                        // Landscape
                        image.Resize(w, tamanoCuadro);

                    }
                    image.Write(rutaMini);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



        private string TIFaJPEG(string parteId)
        {
            
            var jpeg = RutaPorIdJpeg(parteId, $".{configGestor.FormatoConversion}");
            if (!File.Exists(jpeg))
            {
                var tif = RutaPorId(parteId, ".TIF");
                try

                {
                    using (var image = new MagickImage(tif))
                    {
                        if (configGestor.FormatoConversion.Equals("PNG", StringComparison.InvariantCultureIgnoreCase))
                        {
                            image.Write(jpeg, MagickFormat.Png);

                        } else
                        {
                            image.Quality = 90;
                            image.Write(jpeg, MagickFormat.Jpg);
                        }
                        
                    }

                    if (configGestor.ConvertirTiff)
                    {
                        try
                        {
                            File.Delete(tif);
                        }
                        catch (Exception)
                        {


                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return jpeg;
        }

        public async Task<byte[]> LeeBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string rutaFinal = RutaPorId(ParteId, Extension);
            Console.WriteLine(rutaFinal);

            if(Extension.IndexOf("tif", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                rutaFinal = TIFaJPEG(ParteId);
            }

            return await LeeArchivo(rutaFinal);
        }

        private static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }


        private async Task<byte[]> LeeArchivo(string ruta)
        {
            if (File.Exists(ruta)) return (await File.ReadAllBytesAsync(ruta));
            return null;
        }

        public Task<byte[]> LeeThumbnailBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string rutaMini = MiniPorId(ParteId);
            string rutaOriginal = RutaPorId(ParteId, Extension);
            if (!File.Exists(rutaMini))
            {
                CreaMiniatura(rutaOriginal, 200, ParteId);
            }
            return LeeArchivo(rutaMini);
        }

        public async Task<long> EscribeThumbnailBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            string rutafinal = Path.Combine(Ruta(ParteId, ".tif"), $"{hex}.PNG");
            await File.WriteAllBytesAsync(rutafinal, contenido);
            return contenido.Length;
        }

        public async Task<long> EscribeOCRBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            string rutafinal = Path.Combine(Ruta(ParteId, ".tif"), $"{hex}.TXT");
            await File.WriteAllBytesAsync(rutafinal, contenido);
            return contenido.Length;
        }

        public Task<byte[]> LeeOCRBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string hex = int.Parse(ParteId).ToString("x").PadLeft(8, '0');
            string rutafinal = Path.Combine(Ruta(ParteId, ".tif"), $"{hex}.TXT");
            return LeeArchivoTexto(rutafinal.ToUpper());
        }


        public Task<string> ObtienePDF(Modelo.Contenido.Version version, List<string> parteIds)
        {
            throw new NotImplementedException();
        }

        public Task<string> ObtieneZIP(Modelo.Contenido.Version version, List<string> parteIds)
        {
            throw new NotImplementedException();
        }

        private async Task<byte[]> LeeArchivoTexto(string ruta)
        {
            string ocr = null;
            if (File.Exists(ruta))
            {
                var bytes = await File.ReadAllBytesAsync(ruta);
                ocr = Encoding.Unicode.GetString(bytes);

            }

            return ocr != null ? Encoding.UTF8.GetBytes(ocr) : null;
        }


    }
}
