﻿using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PikaOCR
{
    public class  ExtractorTexto
    {
        private readonly ConfiguracionServidor configuracion;
        private readonly IGestorES gestor;
        private readonly ILogger logger;
        public ExtractorTexto (
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
        public async Task<Tuple<bool,string>> TextoImagen(Parte p, string NombreTemporal, string lang = "spa")
        {
            bool doneOk = false;
            string ruta = null;
            string filename = NombreTemporal;
            try
            {
                var bytes = await gestor.LeeBytes(p.ElementoId, p.Id, p.VersionId, p.VolumenId, p.Extension);
                File.WriteAllBytes( filename, bytes);
                var info = new ProcessStartInfo
                {
                    FileName = configuracion.ruta_tesseract,
                    Arguments = $"{filename} {filename} -l {lang}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                
                using (var ps = Process.Start(info))
                {
                    ps.WaitForExit();

                    var exitCode = ps.ExitCode;

                    if (exitCode == 0)
                    {
                        ruta = filename + ".txt";
                        doneOk = true;
                    }
                }

                return new Tuple<bool, string>(doneOk, ruta);
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex}");
                logger.LogError($"Error al obtener texto para {p.Id}@{p.VersionId}-{p.VolumenId}");
                return new Tuple<bool, string>(false, "");
            }
            
        }


        public async Task<Tuple<bool, List<string>>> TextoPDF(Parte p, string NombreTemporal, string lang = "spa")
        {
            List<string> rutas = new List<string>();
            bool doneOk = false;
            string filename = NombreTemporal;
            try
            {
                var bytes = await gestor.LeeBytes(p.ElementoId, p.Id, p.VersionId, p.VolumenId, p.Extension);
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
                        File.WriteAllText(ocrName , text);
                        rutas.Add(ocrName);
                    }
                }
                doneOk = true;

                return new Tuple<bool, List<string>>(doneOk, rutas);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                logger.LogError($"Error al obtener texto para {p.Id}@{p.VersionId}-{p.VolumenId}");
                return new Tuple<bool, List<string>>(false, new List<string>());
            }

        }

        public void ElimninaArchivosOCR(string file)
        {
            Console.WriteLine(file);
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
            files.ForEach(f => {
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