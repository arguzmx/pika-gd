using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Threading;

namespace PIKA.Infraestrctura.Reportes
{

    public class ParteToken
    {
        public ParteToken()
        {
            Tokens = new List<string>();
        }

        public int Parrafo { get; set; }
        public int Run { get; set; }

        public int Text { get; set; }
        public string Texto { get; set; }
        public string TextoNuevo { get; set; }

        public List<string> Tokens { get; set; }

    }



    public static class ReporteEntidades
    {

        public const string PREFIJO_BARCODE1D = "CB:1D:";
        public const string PREFIJO_BARCODE2D = "CB:2D:";


 


        #region comunes

        public static bool EsCodigo1D(this string token)
        {
            return token.StartsWith(PREFIJO_BARCODE1D);
        }


        public static bool EsCodigo2D(this string token)
        {
            return token.StartsWith(PREFIJO_BARCODE2D);
        }

        public static bool EsTokenTexto(this string token)
        {

            if(token.StartsWith(PREFIJO_BARCODE1D) || token.StartsWith(PREFIJO_BARCODE2D))
            {
                return false;
            }

            return true;
        }

        private static TokenCB1D ObtieneToken1D(this string token)
        {
            TokenCB1D t = new TokenCB1D();
            List<string> partes = token.TrimStart('#').TrimEnd('#').Split(':').ToList();
            try
            {
                t.Alto = float.Parse(partes[4]);
                t.Ancho = float.Parse(partes[3]);
                t.Dato = partes[5];
                t.Tipo = partes[2];
            }
            catch (Exception)
            {
                t = null;
            }
            return t;
        }

        private static TokenCB2D ObtieneToken2D(this string token)
        {
            TokenCB2D t = new TokenCB2D();
            List<string> partes = token.TrimStart('#').TrimEnd('#').Split(':').ToList();
            try
            {
                t.Tipo = partes[2];
                t.Ancho = float.Parse(partes[3]);
                t.Dato = partes[4];
            }
            catch (Exception)
            {
                t = null;
            }
            return t;
        }

        #endregion region

 


        /// <summary>
        /// Devuelve el documento .docx medificdo en base a los parámetros recibidos
        /// </summary>
        /// <param name="plantilla">arreglo de bytes con el contenido worsx</param>
        /// <param name="propiedesJson">texto json para el llenado del reporte</param>
        /// <param name="rutaTemporales">ruta al directorio de temporales</param>
        /// <param name="eliminarTemporal">elimina el archivo tempral automa´ticamente</param>
        /// <returns></returns>
        public static byte[] ReportePlantilla(byte[] plantilla,
            string propiedesJson, string rutaTemporales, bool eliminarTemporal = true)
        {

            string ArchivoTemporal = Path.Combine(rutaTemporales, $"{System.Guid.NewGuid().ToString()}.docx");
            File.WriteAllBytes(ArchivoTemporal, plantilla);
            var result = ReportePlantilla(ArchivoTemporal, propiedesJson, rutaTemporales, eliminarTemporal);

            if (eliminarTemporal)
            {
                if (File.Exists(ArchivoTemporal))
                {
                    File.Delete(ArchivoTemporal);
                }
            }

            return result;

            
        }



        private static void ProcesadorParrafos(List<Paragraph> parrafos, dynamic data, string rutaTemporales, WordprocessingDocument wpd, bool eliminar,
            bool useIndex = false, int index = 0, string TablaId = "")
        {

            
            List<ParteToken> tokens = new List<ParteToken>();
            for(int p=0; p < parrafos.Count; p++)
            {
                if(!string.IsNullOrEmpty(parrafos[p].InnerText))
                {
                    string innert = parrafos[p].InnerText;
                    if (innert.IndexOf('#') >= 0)
                    {
                        var runs = parrafos[p].OfType<Run>().ToList();
                        for (int r = 0; r < runs.Count; r++)
                        {
                            var ts = runs[r].OfType<Text>().ToList();
                            for (int t = 0; t < ts.Count; t++)
                            {
                                tokens.Add(new ParteToken() { Parrafo = p, Run = r, 
                                    Text = t, Texto = ts[t].Text, TextoNuevo = ts[t].Text
                                });
                            }
                        }
                    }
                }
            }


            var grupos =  tokens.GroupBy(x => x.Parrafo).Select(x => new { p = x.Key, c = x.Count() });

            foreach(var g in grupos)
            {

                var renglon = tokens.Where(x => x.Parrafo == g.p).ToList();
                
                for (int i = 0; i < renglon.Count(); i++)
                {

                    if (!renglon[i].Texto.ConteoParChar('#'))
                    {
                        string temporal = renglon[i].TextoNuevo;
                        for (int j = i + 1; j < renglon.Count(); j++)
                        {
                            temporal = temporal + renglon[j].TextoNuevo;
                            if (temporal.ConteoParChar('#'))
                            {
                                for (int k = i; k <= j; k++)
                                {
                                    renglon[k].TextoNuevo = (k == j) ? temporal : "";
                                }
                                break;
                            }

                        }
                    }
 
                }
            }

            foreach (var item in tokens)
            {
               var runs  = parrafos[item.Parrafo].OfType<Run>().ToList();
               var textos = runs[item.Run].OfType<Text>().ToList();
               textos[item.Text].Text = item.TextoNuevo;

                if (item.TextoNuevo.IndexOf('#') >= 0)
                {
                    bool inicio = false;
                    string token = "";
                    foreach (char c in item.TextoNuevo)
                    {
                        
                        if(c=='#')
                        {
                            if (inicio)
                            {
                                inicio = false;
                                item.Tokens.Add(token);
                                token = "";
                            } else
                            {
                                inicio = true;
                            }
                        } else
                        {
                            if (inicio)
                            {
                                token += c; 
                            }
                        }
                    };
                }

                foreach (var t in item.Tokens)
                {

                    if (t.EsTokenTexto())
                    {
                        if (useIndex)
                        {
                            textos[item.Text].Text = ObtieneValorIndizado(TablaId, t, data, index);
                        } else
                        {
                            textos[item.Text].Text = ObtieneValorDato(t, data);
                        }
                        
                    }

                    if (t.EsCodigo1D())
                    {
                        textos[item.Text].Text = "";

                        TokenCB1D config = ObtieneToken1D(t);
                        if (config != null)
                        {
                            config.Texto = ObtieneValorDato(config.Dato, data);
                            if (!string.IsNullOrEmpty(config.Texto))
                            {
                                string ruta = CodigosOpticos.GeneraBarcodeLineal(config, rutaTemporales);
                                if (File.Exists(ruta))
                                {
                                    ImagenesWord.InsertaImagen(wpd, runs[item.Run], ruta,
                                     CodigosOpticos.Cm2Pixeles(config.Ancho),
                                     CodigosOpticos.Cm2Pixeles(config.Alto), eliminar);
                                }
                            }
                            
                        }
                        
                    }

                    if (t.EsCodigo2D())
                    {
                        textos[item.Text].Text = "";
                        TokenCB2D config = ObtieneToken2D(t);
                        if (config != null)
                        {
                            config.Texto = ObtieneValorDato(config.Dato, data);
                            if (!string.IsNullOrEmpty(config.Texto))
                            {
                                string ruta = CodigosOpticos.GeneraBarcodeQR(config, rutaTemporales);
                                if (File.Exists(ruta))
                                {
                                    ImagenesWord.InsertaImagen(wpd, runs[item.Run], ruta,
                                       CodigosOpticos.Cm2Pixeles(config.Ancho),
                                       CodigosOpticos.Cm2Pixeles(config.Ancho), eliminar);
                                }
                            }
                        }
                    }


                }

            }

        }

        private static string ObtieneValorDato(string cadena, dynamic data)
        {
            List<string> partes = cadena.Split('.').ToList();

            if(partes.Count == 1)
            {

                return data[partes[0]] ?? "";

            } else
            {
                if(data[partes[0]] != null)
                {
                    string t = cadena.Replace($"{partes[0]}.", "");
                    return ObtieneValorDato(t, data[partes[0]]);

                } else
                {
                    return "";
                }
                
                
            }
        }

        private static string ObtieneValorIndizado(string TablaId, string propiedad, dynamic data, int index)
        {
            try
            {
                dynamic renglon = data[TablaId][index];
                return (string)renglon[propiedad];
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static Table ProcesaTablaV2(Table t, string TablaId, dynamic objeto, string rutaTemporales, dynamic data, WordprocessingDocument wpd, bool eliminarTemporal)
        {

            int total = ((JArray)objeto[TablaId]).Count;

            if (total > 0)
            {

                // Elimina el identificador de la tabla en la celda cero
                var r0 = t.OfType<TableRow>().ElementAt(1);
                List<TableCell> celdas = r0.Descendants<TableCell>().ToList();
                for (int i = 0; i < celdas.Count; i++)
                {
                    List<Paragraph> parrafos = celdas[i].OfType<Paragraph>().ToList();
                    if (i == 0)
                    {
                        int pindex = parrafos.Count - 1;
                        int rindex = 0;
                        int tindex = 0;
                        string texto0 = celdas[i].InnerText.Replace($"*{TablaId}", "");

                        parrafos.ForEach(pp =>
                        {
                            List<Run> runs = pp.OfType<Run>().ToList();
                            rindex = runs.Count - 1;
                            runs.ForEach(u => {
                                List<Text> textos = u.OfType<Text>().ToList();
                                textos.ForEach(t =>
                                {
                                    tindex = textos.Count - 1;
                                    t.Text = "";
                                });
                            });

                        });

                        var runs = parrafos[pindex].OfType<Run>().ToList();
                        var text = runs[rindex].OfType<Text>().ToList();
                        text[tindex].Text = texto0;

                    }
                }


                var clon = (TableRow)t.OfType<TableRow>().ElementAt(1).Clone();


                // Inserta los renglones restantes
                for (int i = 0; i < (total - 1); i++)
                {
                    var insertar = (TableRow)clon.Clone();
                    t.Descendants<TableRow>().Last().InsertAfterSelf(insertar);
                }

                List<TableRow> renglones = t.OfType<TableRow>().ToList();
                for (int i = 1; i < renglones.Count; i++)
                {
                    // Itera sobre la cerdas del renglon
                    celdas = renglones[i].Descendants<TableCell>().ToList();
                    for (int c = 0; c < celdas.Count; c++)
                    {
                        List<Paragraph> parrafos = celdas[c].OfType<Paragraph>().ToList();
                        ProcesadorParrafos(parrafos, data, rutaTemporales, wpd, eliminarTemporal,  true, c, TablaId);
                    }
                }

            }
            else
            {
                t.Descendants<TableRow>().Last().Remove();
            }


            return t;
        }


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

            File.Copy(rutaPlantilla, rutaArchivo);

            using ( WordprocessingDocument doc =
                    WordprocessingDocument.Open(rutaArchivo, true, new OpenSettings() {AutoSave = false})
                  )
            {

                Body b = doc.MainDocumentPart.Document.Body;


                List<Paragraph> parrafos = b.OfType<Paragraph>().ToList();
                //ProcesaParrafos(parrafos, objeto, rutaTemporales, doc);
                ProcesadorParrafos(parrafos, objeto, rutaTemporales, doc, eliminarTemporal);

                
                b.OfType<Table>().ToList().ForEach(t =>
                {
                    
                    bool tablaEntidades = false;
                    // VErifica si l atabla tiene 2 renglones
                    if (t.OfType<TableRow>().Count() == 2)
                    {
                        var clon = (TableRow)t.OfType<TableRow>().ElementAt(1).Clone();
                        List<TableCell> celdas = clon.Descendants<TableCell>().ToList();
                        // y si la primera celda tiene com oinicio el caracters especial *
                        
                        if (celdas[0].InnerText.StartsWith("*"))
                        {
                            string clave = celdas[0].InnerText.TrimStart('*').Split('#')[0];
                            if (objeto[clave] != null)
                            {
                                if (objeto[clave] is JArray)
                                {
                                    tablaEntidades = true;
                                    t = ProcesaTablaV2(t, clave, objeto, rutaTemporales, objeto, doc, eliminarTemporal);
                                }

                            }
                            
                        }
                    }

                    // Si no es una tabla de entidades
                    if (!tablaEntidades)
                    {
                        t.OfType<TableRow>().ToList().ForEach(r =>
                       {
                           r.OfType<TableCell>().ToList().ForEach(c =>
                           {
                               List<Paragraph> parrafos = c.OfType<Paragraph>().ToList();
                               ProcesadorParrafos(parrafos, objeto, rutaTemporales, doc, eliminarTemporal);
                           });
                       });
                    }
                });
                
                doc.Save();
            }

            if (File.Exists(rutaArchivo))
            {
                byte[] bytes = File.ReadAllBytes(rutaArchivo);
                if (eliminarTemporal)
                {
                    try
                    {
                        File.Delete(rutaArchivo);
                    }
                    catch (Exception) { }
                }

                return bytes;
            }

            return null;
        }

    }
}
