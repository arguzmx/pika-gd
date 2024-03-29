﻿using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PikaOCR
{
    public class ExtractorTexto
    {
        private readonly ConfiguracionServidor configuracion;
        private readonly IGestorES gestor;
        private readonly ILogger logger;
        public ExtractorTexto(
            ILogger logger,
            ConfiguracionServidor configuracion,
            IGestorES gestor)
        {
            this.logger = logger;
            this.configuracion = configuracion;
            this.gestor = gestor;
        }


        public string NombreArchivoTemporal(Parte p)
        {
            return Path.Combine(configuracion.ruta_temporal, $"{Guid.NewGuid()}{p.Extension}");
        }

        /// <summary>
        /// Devuelve una ruta al archivo de texto que contiene el OCR de la imagen
        /// </summary>
        /// <param name="p"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task<ResultadoOCR> TextoImagen(Parte p, string NombreTemporal, string lang = "spa")
        {
            ResultadoOCR resultado = new ResultadoOCR() { Parte = p, NombreTemporal = NombreTemporal };
            bool doneOk = false;
            string ruta = null;
            string filename = NombreTemporal;

            try
            {
                if (gestor.AlmacenaOCR)
                {
                    try
                    {
                        var textoOCR = await gestor.LeeOCRBytes(p.ElementoId, gestor.UtilizaIdentificadorExterno ? p.IdentificadorExterno : p.Id, p.VersionId, p.VolumenId, p.Extension);
                        if (textoOCR != null)
                        {
                            ruta = filename + ".txt";
                            File.WriteAllBytes(ruta, textoOCR);
                            doneOk = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error al leer OCR");
                        logger.LogError($"{ex}");
                    }
                }

                if (!doneOk)
                {
                    var bytes = await gestor.LeeBytes(p.ElementoId, gestor.UtilizaIdentificadorExterno ? p.IdentificadorExterno : p.Id, p.VersionId, p.VolumenId, p.Extension);
                    File.WriteAllBytes(filename, bytes);

                    string langconfig = "";
                    if (File.Exists(Path.Combine(configuracion.ruta_tesseract, "tessdata", $"{lang.ToLower()}.traineddata")))
                    {
                        langconfig = $" -l {lang}";
                    }



                    using var outputWaitHandle = new AutoResetEvent(false);
                    using var errorWaitHandle = new AutoResetEvent(false);
                    try
                    {

                        var info = new ProcessStartInfo
                        {
                            FileName = configuracion.ruta_tesseract,
                            Arguments = $"{filename} {filename}{langconfig}".TrimEnd(),
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        info.EnvironmentVariables.Add("OMP_THREAD_LIMIT", "1");


                        using (var ps = Process.Start(info))
                        {
                            ps.WaitForExit();

                            var exitCode = ps.ExitCode;

                            if (exitCode == 0)
                            {
                                ruta = filename + ".txt";
                                doneOk = true;

                                if (gestor.AlmacenaOCR)
                                {
                                    try
                                    {
                                        await gestor.EscribeOCRBytes(gestor.UtilizaIdentificadorExterno ? p.IdentificadorExterno : p.Id, p.ElementoId, p.VersionId, File.ReadAllBytes(ruta));
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError($"Error al escribir OCR");
                                        logger.LogError($"{ex}");
                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine($"{ps.ExitCode} {ps.StandardOutput.ReadToEnd()}  {ps.StandardError.ReadToEnd()}");
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }

                resultado.Exito = doneOk;
                resultado.Rutas.Add(ruta);
                return resultado;
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex}");
                logger.LogError($"Error al obtener texto para {p.Id}@{p.VersionId}-{p.VolumenId}");
                resultado.Exito = false;
                resultado.Rutas = new List<string>();
                return resultado;
            }

        }


        public async Task<ResultadoOCR> TextoPDF(Parte p, string NombreTemporal, string lang = "spa")
        {
            ResultadoOCR resultado = new ResultadoOCR() { Parte = p, NombreTemporal = NombreTemporal };
            List<string> rutas = new List<string>();
            string filename = NombreTemporal;
            try
            {
                var bytes = await gestor.LeeBytes(p.ElementoId, gestor.UtilizaIdentificadorExterno ? p.IdentificadorExterno : p.Id, p.VersionId, p.VolumenId, p.Extension);
                File.WriteAllBytes(filename, bytes);

                using (PdfDocument document = PdfDocument.Open(filename))
                {
                    int pageCount = document.NumberOfPages;

                    // Page number starts from 1, not 0.
                    for (int i = 1; i <= pageCount; i++)
                    {
                        Page page = document.GetPage(i);
                        string ocrName = $"{filename}{i}.txt";
                        string text = page.Text;
                        File.WriteAllText(ocrName, text);
                        rutas.Add(ocrName);
                    }
                }
                resultado.Exito = true;
                resultado.Rutas = rutas;
                return resultado;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                logger.LogError($"Error al obtener texto para {p.Id}@{p.VersionId}-{p.VolumenId}");
                resultado.Exito = false;
                resultado.Rutas = new List<string>();
                return resultado;
            }

        }

        public void ElimninaArchivosOCR(string file)
        {

            FileInfo fi = new FileInfo(file);
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception)
                {
                }
            }

            List<string> files = Directory.GetFiles(fi.DirectoryName, $"{fi.Name}*.txt").ToList();
            files.ForEach(f =>
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception)
                {
                }
            });

        }
    }
}
