﻿using ImageMagick;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Gestores
{


    public class GestorLocal : GestorBase, IGestorES
    {
        private GestorLocalConfig configGestor;
        private ConfiguracionServidor configServidor;
        private ILogger logger;
        private readonly IConfiguration configuration;
        public bool AlmacenaOCR { get => true; }
        public bool UtilizaIdentificadorExterno { get => false; }

        /// <summary>
        /// Constructur para validación de la conexión
        /// </summary>
        /// <param name="configGestor"></param>
        /// <param name="volumen"></param>
        /// <param name="opciones"></param>
        public GestorLocal(
            ILogger logger,
            GestorLocalConfig configGestor,
            IConfiguration configuration,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.logger = logger;
            this.configGestor = configGestor;
            this.configServidor = opciones.Value;
            this.configuration = configuration;
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


        public Task<bool> ExisteParte(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string nombreArchivo = ParteId + Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            return Task.FromResult(File.Exists(rutaFinal));
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
            if (!File.Exists(rutaFinal))
            {
                string rutaOriginal = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
                string nombreArchivoOriginal = ParteId + Extension.ToUpper();
                string rutaFinalOriginal = Path.Combine(rutaOriginal, nombreArchivoOriginal);

                CreaMiniatura(ruta, rutaFinalOriginal, 200, ParteId);
            }

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
        private void CreaMiniatura(string rutaMinuaturas, string imagenFuente, int tamanoCuadro, string ParteId)
        {
            try
            {

                if (!Directory.Exists(rutaMinuaturas))
                {
                    Directory.CreateDirectory(rutaMinuaturas);
                }

                using (Image image = Image.Load(imagenFuente))
                {
                    if (image.Width > image.Height)
                    {
                        int h = (int)((((float)image.Height) / ((float)image.Width)) * tamanoCuadro);
                        // Landscape
                        image.Mutate(x => x.Resize(tamanoCuadro, h));

                    }
                    else
                    {
                        //Portrait
                        int w = (int)((((float)image.Width) / ((float)image.Height)) * tamanoCuadro);
                        // Landscape
                        image.Mutate(x => x.Resize(w, tamanoCuadro));

                    }

                    string nombreArchivo = ParteId + ".PNG";
                    string rutaFinal = Path.Combine(rutaMinuaturas, nombreArchivo);
                    if (File.Exists(rutaFinal))
                    {
                        File.Delete(rutaFinal);
                    }
                    image.Save(rutaFinal);
                }

            }
            catch (Exception ex)
            {
                try
                {
                    using (Image image = Image.Load(Path.Combine(AssemblyDirectory, "Contenido", "Imagenes", "img-error.png")))
                    {
                        if (image.Width > image.Height)
                        {
                            int h = (int)((((float)image.Height) / ((float)image.Width)) * tamanoCuadro);
                            // Landscape
                            image.Mutate(x => x.Resize(tamanoCuadro, h));

                        }
                        else
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
                catch (Exception)
                {
                }
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
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

        public async Task<string> ObtieneZIP(Modelo.Contenido.Version version, List<string> parteIds)
        {
            var tempDir = this.configServidor.ruta_cache_fisico;
            var zipFile = Path.Combine(tempDir, $"z{Guid.NewGuid().ToString().Replace("-","")}.zip");
            int cuenta = 0;
            using (ZipFile zip = new ZipFile() { CompressionLevel = Ionic.Zlib.CompressionLevel.None})
            {
                version.Partes.OrderBy(x => x.Indice).ToList().ForEach(p =>
                {
                    string ruta = Path.Combine(this.configGestor.Ruta, version.ElementoId, version.Id);
                    string nombreArchivo = p.Id.ToString() + p.Extension.ToUpper();
                    string rutaFinal = Path.Combine(ruta, nombreArchivo);

                    if (File.Exists(rutaFinal))
                    {
                        string nombreZip = $"{p.Indice.ToString().PadLeft(8, '0')}-{p.NombreOriginal}" + p.Extension.ToUpper();
                        zip.AddEntry(nombreZip, File.ReadAllBytes(rutaFinal));
                        
                        cuenta ++;
                    }

                });

                if (cuenta > 0)
                {
                    zip.Save(zipFile);
                }
            }

            await Task.Delay(1);
            return cuenta > 0 ? zipFile : "";
        }


        public async Task<string> ObtienePDF(Modelo.Contenido.Version version, List<string> parteIds, int PorcientoEscala = 100)
        {
            
            string fileId = Guid.NewGuid().ToString().Replace("-","");
            var tempDir = Path.Combine( this.configServidor.ruta_cache_fisico, fileId);
            if(!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            int cuenta = 0;
            int parte = 1;
            string finalPDF = "";
            int tamanolote = 30;
            string pdfJoiner = configuration.GetValue<string>("TareasBackground:pdf:convertidor");
            bool debug = configuration.GetValue<bool>("TareasBackground:pdf:debug");
            var per = new Percentage((double)PorcientoEscala);

            if (PorcientoEscala>100)
            {
                PorcientoEscala = 100;
            }

            if (PorcientoEscala < 10)
            {
                PorcientoEscala = 10;
            }

            string stamanolote = configuration.GetValue<string>("TareasBackground:pdf:tamanolote");
            if (!string.IsNullOrEmpty(stamanolote))
            {
                if (int.TryParse(stamanolote, out tamanolote))
                {
                    if (tamanolote <= 20)
                    {
                        tamanolote = 20;
                    }

                } else
                {
                    tamanolote = 30;
                }
            }
            
            List<string> imgFormats = new List<string>() { ".png", ".jpg", ".jpeg", ".bmp", ".tif", ".gif" };
            List<string> imagenes = new List<string>();
            List<string> pdfs = new List<string>();

            int Indice = 1;
            foreach (var p in version.Partes.OrderBy(x => x.Indice).ToList())
            {
                if (debug) logger.LogDebug($"Procesando {Indice + 1}/{version.Partes.Count()}");
                if (imgFormats.IndexOf(p.Extension.ToLower()) >= 0)
                {
                    string ruta = Path.Combine(this.configGestor.Ruta, version.ElementoId, version.Id);
                    string nombreArchivo = p.Id.ToString() + p.Extension.ToUpper();
                    string rutaFinal = Path.Combine(ruta, nombreArchivo);

                    if (File.Exists(rutaFinal))
                    {
                        if (debug) logger.LogDebug($"Imagen {rutaFinal}");
                        FileInfo fi = new FileInfo(rutaFinal);
                        
                        string rutaCopia = Path.Combine(tempDir, $"{Indice.ToString().PadLeft(8, '0')}{fi.Extension}");
                        var im = new MagickImage(rutaFinal);
                        if (PorcientoEscala != 100)
                        {
                            im.Resize(per);
                        }
                        im.Quality = 80;
                        im.Write(rutaCopia, MagickFormat.Jpg);
                        im.Dispose();

                        imagenes.Add(rutaCopia);
                        Indice++;

                    }
                }
            }

            finalPDF = Path.Combine(this.configServidor.ruta_cache_fisico, $"z{fileId}.pdf");
            int result = EjecutaPDFTemporal(pdfJoiner, imagenes, finalPDF, debug);
            if (result != 0)
            {
                finalPDF = "";
            }


            // NO ELIMINAR ESTE CODIGO, PUEDE UTILZIARSE EN EL FUTURO
            // ------------------------------------------------------------
            // ------------------------------------------------------------

            //int conteo = 0;
            //List<string> listaTemp = new List<string>();
            //for(int  i =0 ; i < imagenes.Count; i++)
            //{
            //    listaTemp.Add(imagenes[i]);
            //    conteo++;
            //    if (conteo >= tamanolote)
            //    {

            //        var pdfFile = Path.Combine(tempDir, $"z{fileId}{parte}.pdf");
            //        if (debug) logger.LogDebug($"Creando temporal {pdfFile}@{cuenta}");

            //        int result = EjecutaPDFTemporal(pdfJoiner, listaTemp, pdfFile, debug);
            //        if (result != 0)
            //        {
            //            pdfs.Clear();
            //            conteo = 0;
            //            break;

            //        } else
            //        {
            //            pdfs.Add(pdfFile);
            //            foreach (string f in listaTemp)
            //            {
            //                try
            //                {
            //                    File.Delete(f);
            //                }
            //                catch (Exception)
            //                {
            //                }
            //            }
            //        }
            //        listaTemp.Clear();
            //        conteo = 0;
            //        parte++;
            //    } 
            //}

            //if (conteo > 0)
            //{
            //    var pdfFile = Path.Combine(tempDir, $"z{fileId}{parte}.pdf");
            //    if (debug) logger.LogDebug($"Creando temporal {pdfFile}@{cuenta}");

            //    int result = EjecutaPDFTemporal(pdfJoiner, listaTemp, pdfFile, debug);
            //    if (result != 0)
            //    {
            //        pdfs.Clear();
            //        conteo = 0;
            //    }
            //    else
            //    {
            //        pdfs.Add(pdfFile);
            //        foreach (string f in listaTemp)
            //        {
            //            try
            //            {
            //                File.Delete(f);
            //            }
            //            catch (Exception)
            //            {
            //            }
            //        }
            //    }
            //}



            //if (pdfs.Count > 0)
            //{
            //    finalPDF = Path.Combine(this.configServidor.ruta_cache_fisico, $"z{fileId}.pdf");
            //    string files = "";
            //    foreach (var pdf in pdfs)
            //    {
            //        files += $"{pdf} ";
            //    }


            //    var info = new ProcessStartInfo
            //    {
            //        FileName = "gs",
            //        Arguments = $" -dNOPAUSE -sDEVICE=pdfwrite -sOUTPUTFILE={finalPDF} -dBATCH {files}".TrimEnd(),
            //        RedirectStandardError = true,
            //        RedirectStandardOutput = true,
            //        CreateNoWindow = true,
            //        UseShellExecute = false
            //    };

            //    if (debug) logger.LogDebug($"Ejecutando: {info.FileName}{info.Arguments}");

            //    using (var ps = Process.Start(info))
            //    {
            //        ps.WaitForExit();
            //        var exitCode = ps.ExitCode;

            //        if (exitCode == 0)
            //        {
            //            if (debug) logger.LogDebug($"{finalPDF} Finalizado OK");
            //        }
            //        else
            //        {

            //            finalPDF = "";
            //            if (debug) logger.LogDebug($"Error al generar PDF {ps.ExitCode} {ps.StandardOutput.ReadToEnd()}  {ps.StandardError.ReadToEnd()}");
            //        }
            //    }

            //}
            //else
            //{
            //    finalPDF = "";
            //}


            try
            {
                Directory.Delete(tempDir, true);
            }
            catch (Exception ex)
            {
                if (debug) logger.LogDebug($"Error al eliminar temporal = {tempDir}");

            }

            await Task.Delay(1);
            if (debug) logger.LogDebug($"Respuesta X PDF = {finalPDF}");
            return finalPDF;

        }


        private int EjecutaPDFTemporal(string pdfJoiner, List<string> Archivos, string PDF, bool debug)
        {

            //gs -q -dNOSAFER -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -sOutputFile=output.pdf viewjpeg.ps -c (00000001.JPG) viewJPEG showpage (00000002.JPG) viewJPEG showpage
            string ListaArchivos = "";
            Archivos.ForEach(a =>
            {
                ListaArchivos += $"({a}) viewJPEG showpage ";
            });

            var info = new ProcessStartInfo
            {
                FileName = "gs",
                Arguments = $" -q -dNOSAFER -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -sOutputFile={PDF} viewjpeg.ps -c {ListaArchivos}".TrimEnd(),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            if (debug) logger.LogDebug($"Ejecutando: {info.FileName}{info.Arguments}");
            int exitCode = -1;

            using (var ps = Process.Start(info))
            {
                ps.WaitForExit();
                exitCode = ps.ExitCode;

                if (exitCode == 0)
                {
                    if (debug) logger.LogDebug($"{PDF} Finalizado OK");
                }
                else
                {

                    if (debug) logger.LogDebug($"Error al generar PDF {ps.ExitCode} {ps.StandardOutput.ReadToEnd()}  {ps.StandardError.ReadToEnd()}");
                }
            }

            return exitCode;
        }
        public async Task<long> EscribeThumbnailBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string rutaMiniaturas = Path.Combine(ruta, "thumbnails");
            string nombreArchivo = ParteId + ".PNG";
            string rutaFinal = Path.Combine(rutaMiniaturas, nombreArchivo);
            await File.WriteAllBytesAsync(rutaFinal, contenido);
            return contenido.Length;
        }

        public async Task<long> EscribeOCRBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string rutaMiniaturas = Path.Combine(ruta, "ocr");
            
            if (!Directory.Exists(rutaMiniaturas))
            {
                Directory.CreateDirectory(rutaMiniaturas);
            }

            string nombreArchivo = ParteId + ".TXT";
            string rutaFinal = Path.Combine(rutaMiniaturas, nombreArchivo);
            await File.WriteAllBytesAsync(rutaFinal, contenido);
            return contenido.Length;
        }

        public Task<byte[]> LeeOCRBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string rutaMiniaturas = Path.Combine(ruta, "ocr");
            string nombreArchivo = ParteId + ".TXT";
            string rutaFinal = Path.Combine(rutaMiniaturas, nombreArchivo);
            return LeeArchivo(rutaFinal);
        }

        public Task<bool> EliminaOCRBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string rutaMiniaturas = Path.Combine(ruta, "ocr");
            string nombreArchivo = ParteId + ".TXT";
            string rutaFinal = Path.Combine(rutaMiniaturas, nombreArchivo);
            if (File.Exists(rutaFinal))
            {
                File.Delete(rutaFinal);
                // return Task.FromResult(true);
            }

            return Task.FromResult(true);
        }

        public async Task<bool> Elimina(string RutaArchivo)
        {
            await Task.Delay(1);
            try
            {
                File.Delete(RutaArchivo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task EliminaBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            List<string> archivos = new List<string>();
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string nombreArchivo = ParteId + Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            
            string rutaOCR = Path.Combine(ruta, "ocr");
            string nombreArchivoOCR = ParteId + ".TXT";
            string rutaFinalOCR = Path.Combine(rutaOCR, nombreArchivoOCR);

            string rutaMini = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId, "thumbnails");
            string nombreArchivoMini = ParteId + ".PNG";
            string rutaFinalMini = Path.Combine(rutaMini, nombreArchivoMini);

            archivos.Add(rutaFinalMini);
            archivos.Add(rutaFinal);
            archivos.Add(rutaFinalOCR);
            archivos.ForEach(ruta =>
            {
                if (!string.IsNullOrEmpty(ruta) && File.Exists(ruta))
                {
                    try
                    {
                       File.Delete(ruta);
                    }
                    catch (Exception)
                    {
                    }
                };
            });


            return Task.CompletedTask;
        }

        public Task<string> RotarImagen(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension, int Angulo)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string nombreArchivo = ParteId + Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            if(File.Exists(rutaFinal))
            {
                if (File.Exists($"{rutaFinal}.modificada"))
                {
                    File.Delete($"{rutaFinal}.modificada");
                }

                bool exito = false;
                if (".JPG.JPEG".IndexOf(Extension.ToUpper()) >= 0)
                {
                    exito = JPEGLoseless.Rotar(configServidor.ruta_jpegtran, rutaFinal, $"{rutaFinal}.modificada", Angulo);
                    if (!exito)
                    {
                        // Fallback en caso de una conversion no existosa por ejemplo de una imagen corrompida
                        using (Image image = Image.Load(rutaFinal))
                        {
                            image.Mutate(x => x.Rotate(Angulo));
                            image.SaveAsJpeg($"{rutaFinal}.modificada", new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = 90 });
                            exito = true;
                        }
                    }
                } else
                {
                    using (Image image = Image.Load(rutaFinal))
                    {
                        image.Mutate(x => x.Rotate(Angulo));
                        image.SaveAsJpeg($"{rutaFinal}.modificada", new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = 90 });
                        exito = true;
                    }
                }

                if (exito)
                {
                    
                    File.Move(rutaFinal, $"{rutaFinal}.old");
                    File.Move($"{rutaFinal}.modificada", rutaFinal);
                    File.Delete($"{rutaFinal}.old");

                    EliminaOCRBytes(ElementoId, ParteId, VersionId, VolumenId, Extension);
                    RegeneraMiniatura(ElementoId, ParteId, VersionId, VolumenId, Extension);
                    return Task.FromResult(ElementoId);
                }
            }

            return Task.FromResult("");
        }


        private void RegeneraMiniatura(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId, "thumbnails");
            string nombreArchivo = ParteId + ".PNG";
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            string rutaOriginal = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string nombreArchivoOriginal = ParteId + Extension.ToUpper();
            string rutaFinalOriginal = Path.Combine(rutaOriginal, nombreArchivoOriginal);
            CreaMiniatura(ruta, rutaFinalOriginal, 200, ParteId);
        }

        public Task<string> ReflejarImagen(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension, string Direccion)
        {
            string ruta = Path.Combine(this.configGestor.Ruta, ElementoId, VersionId);
            string nombreArchivo = ParteId + Extension.ToUpper();
            string rutaFinal = Path.Combine(ruta, nombreArchivo);
            if (File.Exists(rutaFinal))
            {
                if (File.Exists($"{rutaFinal}.modificada"))
                {
                    File.Delete($"{rutaFinal}.modificada");
                }

                bool exito = false;
                if (".JPG.JPEG".IndexOf(Extension.ToUpper()) >= 0)
                {
                    exito = JPEGLoseless.Reflejar(configServidor.ruta_jpegtran, rutaFinal, $"{rutaFinal}.modificada", Direccion.ToUpper() == "H");
                    if(!exito)
                    {

                        // Fallback en caso de una conversion no existosa por ejemplo de una imagen corrompida
                        using (Image image = Image.Load(rutaFinal))
                        {
                            if (Direccion.ToUpper() == "H")
                            {
                                image.Mutate(x => x.Flip(FlipMode.Horizontal));
                            }
                            else
                            {
                                image.Mutate(x => x.Flip(FlipMode.Vertical));
                            }
                            try
                            {
                                image.SaveAsJpeg($"{rutaFinal}.modificada", new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() {  Quality =90} );
                                exito = true;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ex}");
                                throw;
                            }
                            
                        }
                    }
                }
                else
                {
                    using (Image image = Image.Load(rutaFinal))
                    {
                        if(Direccion.ToUpper() == "H")
                        {
                            image.Mutate(x => x.Flip(FlipMode.Horizontal));
                        } else
                        {
                            image.Mutate(x => x.Flip(FlipMode.Vertical));
                        }
                        image.SaveAsJpeg($"{rutaFinal}.modificada", new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder() { Quality = 90 });
                        exito = true;
                    }
                }

                if (exito)
                {
                    File.Move(rutaFinal, $"{rutaFinal}.old");
                    File.Move($"{rutaFinal}.modificada", rutaFinal);
                    File.Delete($"{rutaFinal}.old");

                    EliminaOCRBytes(ElementoId, ParteId, VersionId, VolumenId, Extension);
                    RegeneraMiniatura(ElementoId, ParteId, VersionId, VolumenId, Extension);
                    return Task.FromResult(ElementoId);
                }
            }

            return Task.FromResult("");
        }
    }
}
