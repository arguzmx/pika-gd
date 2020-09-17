using System;

using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using Newtonsoft.Json;

namespace PIKA.Infraestrctura.Reportes
{
    public class ReporteEntidades
    {





        /// <summary>
        /// Devuelve el documento .docx medificdo en base a los parámetros recibidos
        /// </summary>
        /// <param name="rutaPlantilla">Ruta del documento de word a utilizar</param>
        /// <param name="propiedesJson">texto json para el llenado del reporte</param>
        /// <param name="rutaTemporales">ruta al directorio de temporales</param>
        /// <param name="eliminarTemporal">elimina el archivo tempral automa´ticamente</param>
        /// <returns></returns>
        public static byte[] ReportePlantilla(string rutaPlantilla, 
            string propiedesJson, string rutaTemporales, bool eliminarTemporal = true)
        {
            if (!File.Exists(rutaPlantilla)) throw new Exception("Plantilla inexistente");

            dynamic objeto = JsonConvert.DeserializeObject(propiedesJson);
            string nombreArchivo = System.Guid.NewGuid().ToString() + ".docx";
            string rutaArchivo = Path.Combine(rutaTemporales, nombreArchivo);

            string n = objeto.nombre;
            Console.WriteLine($"Documento valido {n}");
            using (WordprocessingDocument doc =
                    WordprocessingDocument.Open(rutaPlantilla, false))
            {
                Body b = doc.MainDocumentPart.Document.Body;
                b.OfType<Paragraph>().ToList().ForEach(p =>
                {
                    Console.WriteLine($"{p.InnerText}");

                });

                b.OfType<Table>().ToList().ForEach(t =>
                {
                    Console.WriteLine($"------------------");
                    t.OfType<TableProperties>().ToList().ForEach(tp =>
                    {
                        tp.OfType<TableCaption>().ToList().ForEach(t =>
                        {
                            Console.WriteLine($"{t.Val}");
                        })
                         ;
                    }
                        );
                });


                doc.SaveAs(rutaArchivo);
            }

            if (File.Exists(rutaArchivo))
            {
                byte[] bytes = File.ReadAllBytes(rutaArchivo);
                if(eliminarTemporal)
                {
                    try
                    {
                        File.Delete(rutaArchivo);
                    }
                    catch (Exception ){}
                }

                return bytes;
            }

            return null;
        }

    }
}
