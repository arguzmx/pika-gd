using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;


namespace PIKA.Infraestrctura.Reportes
{

    public class TokensCelda {
        public TokensCelda(int col, List<string> tokens)
        {
            this.Columna = col;
            this.Tokens = tokens;
        }
        public int Columna { get; set; }
        public List<string> Tokens { get; set; }
    }


    public static class ReporteEntidades
    {

        public const string PREFIJO_BARCODE1D = "CB:1D:";
        public const string PREFIJO_BARCODE2D = "CB:2D:";

        #region gestion de tablas


        private static Table ProcesaTabla(Table t, string TablaId, dynamic objeto)
        {

            int total = ((JArray)objeto[TablaId]).Count;

            if (total > 0)
            {
                var clon = (TableRow)t.OfType<TableRow>().ElementAt(1).Clone();
                // ontiene los tokens de cada renglon
                List<TableCell> celdas = clon.Descendants<TableCell>().ToList();
                List<TokensCelda> alltokens = new List<TokensCelda>();
                for (int i = 0; i < celdas.Count; i++)
                {
                    List<Paragraph> parrafos = celdas[i].OfType<Paragraph>().ToList();
                    for (int j = 0; j < parrafos.Count; j++)
                    {
                        string texto = parrafos[j].InnerText;
                        if (!string.IsNullOrEmpty(texto))
                        {
                            List<string> l = ObtieneTokens(texto);
                            if (l.Count > 0)
                            {
                                alltokens.Add(new TokensCelda(i, l));
                            }
                        }
                    }
                }


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
                        // Reemplaza para cad token localizado en la columna
                        alltokens.Where(x => x.Columna == c).ToList().ForEach(columna =>
                        {
                            columna.Tokens.ForEach(token =>
                            {
                                List<Paragraph> parrafos = celdas[c].OfType<Paragraph>().ToList();
                                for (int p = 0; p < parrafos.Count; p++)
                                {
                                    string texto = parrafos[p].InnerText;
                                    if (!string.IsNullOrEmpty(texto))
                                    {
                                        if(token.EsTokenTexto())
                                        {
                                            var x = ReemplazaTokenTabla(parrafos[p], TablaId, token, objeto, i - 1);
                                            parrafos[p].InnerXml = x.InnerXml;
                                        }
                                    }
                                }
                            });
                        });
                    }
                }

            }
            else
            {
                t.Descendants<TableRow>().Last().Remove();
            }


            return t;
        }


        private static Paragraph ReemplazaTokenTabla(Paragraph p, string TablaId, string Token, dynamic Objeto, int Indice)
        {
            string valor = ObtieneValorTokenTabla(TablaId, Token, Objeto, Indice);
            string comparador = "#" + Token + "#";

            if (p.InnerText.Contains(comparador))
            {

                int ultimorunindex = 0;
                string texto = "";

                List<Run> r = p.OfType<Run>().ToList();
                for (int i = 0; i < r.Count; i++)
                {
                    List<Text> t = r[i].OfType<Text>().ToList();
                    for (int j = 0; j < t.Count; j++)
                    {
                        texto = texto + t[j].Text;
                        if (texto.Contains(comparador))
                        {

                            for (int k = i; k >= ultimorunindex; k--)
                            {
                                string demo = "";
                                for (int l = k; l <= i; l++)
                                {
                                    demo = demo + r[l].InnerText;
                                    if (demo.Contains(comparador))
                                    {
                                        if (demo == comparador)
                                        {
                                            bool primero = true;
                                            for (int h = k; h <= i; h++)
                                            {
                                                List<Text> fixers = r[h].OfType<Text>().ToList();
                                                for (int f = 0; f < fixers.Count; f++)
                                                {
                                                    if (primero)
                                                    {
                                                        fixers[f].Text = valor;
                                                        primero = false;
                                                    }
                                                    else
                                                    {
                                                        fixers[f].Text = "";
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Caso no controlado ReemplazaTokenTabla");
                                        }

                                        texto = "";
                                        ultimorunindex = i;
                                        break;
                                    }

                                }
                            }




                        }
                    }
                }


            }

            return p;
        }

        private static string ObtieneValorTokenTabla(string TablaId, string Token, dynamic Objeto, int Indice)
        {
            dynamic renglon = Objeto[TablaId][Indice];
            return (string)renglon[Token];
        }

        #endregion


        #region gestion parrafos

 
        private static Paragraph ReemplazaToken(Paragraph p, string Token, dynamic Objeto)
        {
            string valor = ObtieneValorToken(Token, Objeto);
            string comparador = "#" + Token + "#";

            if (p.InnerText.Contains(comparador))
            {

                int ultimorunindex = 0;
                string texto = "";

                List<Run> r = p.OfType<Run>().ToList();
                for (int i = 0; i < r.Count; i++)
                {
                    List<Text> t = r[i].OfType<Text>().ToList();
                    for (int j = 0; j < t.Count; j++)
                    {
                        texto = texto + t[j].Text;
                        if (texto.Contains(comparador))
                        {

                            for (int k = i; k >= ultimorunindex; k--)
                            {
                                string demo = "";
                                for (int l = k; l <= i; l++)
                                {
                                    demo = demo + r[l].InnerText;
                                    if (demo.Contains(comparador))
                                    {
                                        if (demo == comparador)
                                        {
                                            bool primero = true;
                                            for (int h = k; h <= i; h++)
                                            {
                                                List<Text> fixers = r[h].OfType<Text>().ToList();
                                                for (int f = 0; f < fixers.Count; f++)
                                                {
                                                    if (primero)
                                                    {
                                                        fixers[f].Text = valor;
                                                        primero = false;
                                                    }
                                                    else
                                                    {
                                                        fixers[f].Text = "";
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Caso no controlado ReemplazaToken");
                                        }

                                        texto = "";
                                        ultimorunindex = i;
                                        break;
                                    }

                                }
                            }




                        }
                    }
                }


            }
            return p;
        }

        private static string ObtieneValorToken(string Token, dynamic Objeto)
        {
            string[] lista = Token.Split('.');
            int index = 1;
            dynamic d = null;
            string valor = "";
            try
            {
                foreach (string s in lista)
                {
                    if (index == 1)
                    {
                        d = Objeto[s];
                    }
                    else
                    {
                        d = d[s];
                    }
                    if (index == lista.Length)
                    {
                        valor = (string)d;
                    }
                    index++;
                }
                return valor;
            }
            catch (Exception)
            {

                return "";
            }
            
        }


        #endregion


        #region comunes

        private static List<string> ObtieneTokens(string texto)
        {
            List<string> lista = new List<string>();
            int posicion = 0;
            int inicio;
            int fin;
            bool terminado = false;

            do
            {
                inicio = texto.IndexOf("#", posicion);
                if (inicio >= 0)
                {
                    if (texto.Length > inicio + 1)
                    {
                        fin = texto.IndexOf("#", inicio + 1);
                        if (fin >= 0)
                        {
                            string item = texto.Substring(inicio + 1, (fin - (inicio + 1)));
                            lista.Add(item);
                            posicion = fin + 1;
                        }
                        else
                        {
                            terminado = true;
                        }

                    }
                    else
                    {
                        terminado = true;
                    }
                }
                else
                {
                    terminado = true;
                }

            } while (!terminado);



            return lista;
        }


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


        #region Codigos opticos


        private static void procesaCodigoLineal(string token, dynamic objeto, string rutaTemporales,
            WordprocessingDocument doc, Paragraph p)
        {
            TokenCB1D config = ObtieneToken1D(token);
            if (config != null)
            {

                config.Dato = ObtieneValorToken(config.Dato, objeto);

                string ruta = CodigosOpticos.GeneraBarcodeLineal(config, rutaTemporales);
                if (File.Exists(ruta))
                {
                    bool primero = true;
                    p.OfType<Run>().ToList().ForEach(p =>
                    {

                        p.OfType<Text>().ToList().ForEach(t =>
                        {
                            t.Text = "";
                        });

                        if (primero)
                        {
                            ImagenesWord.InsertaImagen(doc, p, ruta,
                                CodigosOpticos.Cm2Pixeles(config.Ancho),
                                CodigosOpticos.Cm2Pixeles(config.Alto));
                            primero = false;
                        }
                    });

                }
            }
        }

        private static void procesaCodigoQR(string token, dynamic objeto, string rutaTemporales,
            WordprocessingDocument doc, Paragraph p)
        {
            TokenCB2D config = ObtieneToken2D(token);
            if (config != null)
            {

                config.Dato = ObtieneValorToken(config.Dato, objeto);

                string ruta = CodigosOpticos.GeneraBarcodeQR(config, rutaTemporales);
                if (File.Exists(ruta))
                {
                    bool primero = true;
                    p.OfType<Run>().ToList().ForEach(p =>
                    {

                        p.OfType<Text>().ToList().ForEach(t =>
                        {
                            t.Text = "";
                        });

                        if (primero)
                        {
                            ImagenesWord.InsertaImagen(doc, p, ruta,
                                CodigosOpticos.Cm2Pixeles(config.Ancho),
                                CodigosOpticos.Cm2Pixeles(config.Ancho));
                            primero = false;
                        }
                    });

                }
            }
        }


        #endregion

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
                for (int i = 0; i < parrafos.Count; i++)
                {
                    string texto = parrafos[i].InnerText;
                    if (!string.IsNullOrEmpty(texto))
                    {
                        List<string> l = ObtieneTokens(texto);
                        if (l.Count > 0)
                        {
                            foreach (string s in l)
                            {
                                
                                if(s.EsTokenTexto())
                                {
                                    var x = ReemplazaToken(parrafos[i], s, objeto);
                                    parrafos[i].InnerXml = x.InnerXml;
                                }

                                if (s.EsCodigo1D())
                                {
                                    procesaCodigoLineal(s, objeto, rutaTemporales, doc, parrafos[i]);
                                }

                                if (s.EsCodigo2D())
                                {
                                    procesaCodigoQR(s, objeto, rutaTemporales, doc, parrafos[i]);
                                }
                            }
                        }
                    }
                }



                b.OfType<Table>().ToList().ForEach(t =>
                {
                    t.OfType<TableProperties>().ToList().ForEach(tp =>
                    {
                        tp.OfType<TableCaption>().ToList().ForEach(tc =>
                        {
                            if (objeto[tc.Val] != null)
                            {
                                if (objeto[tc.Val] is JArray)
                                {
                                    t = ProcesaTabla(t, tc.Val, objeto);
                                }

                            }
                        });
                    }
                        );
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
