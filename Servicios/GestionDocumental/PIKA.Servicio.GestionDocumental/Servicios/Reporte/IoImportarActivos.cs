using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bogus.Extensions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios.Reporte
{
    public class IoImportarActivos : ContextoServicioGestionDocumental,
          IServicioInyectable
    {
        private IRepositorioAsync<Activo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<EntradaClasificacion> RepoEntrada;
        private IRepositorioAsync<TipoArchivo> repoTA;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private IRepositorioAsync<TipoValoracionDocumental> repoTVD;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoTV;
        private IRepositorioAsync<ElementoClasificacion> RepoElemento;
        private List<Estructuraexcel> listaExcel=new List<Estructuraexcel>();
        private List<EntradaClasificacion> ListaEntradas = new List<EntradaClasificacion>();
        private string Error;
        private readonly ConfiguracionServidor configuracion;
        private ServicioActivo servicioActivos;

        public IoImportarActivos(
            ServicioActivo servicioActivos,
            ILogger<ServicioLog> Logger, 
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            IOptions<ConfiguracionServidor> Confi)
          : base(proveedorOpciones, Logger)
        {

            this.servicioActivos = servicioActivos;

           
                this.configuracion = Confi.Value;
                this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
                this.repo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
                this.RepoEntrada = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
        }

        private async Task LeerArchivo(string RutaArchivo,int col, int fila,int indice,string ArchivoId, string TipoId, string origenId,string formatofecha,string Rutacompleta)
        {
            
            string Columnas = "A,B,C,D,E,F,G,H,I,J,K,L,M";
            Activo a = new Activo();
            a.ArchivoId = ArchivoId;
            a.TipoOrigenId = TipoId;
            a.OrigenId = origenId;
            a.ArchivoOrigenId = ArchivoId;
            string valor = "";
          if(indice!=0)
            {
                foreach (string item in Columnas.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray())
                {
                    valor = ObtenerValorCelda(Rutacompleta, $"{Columnas.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray()[col]}{fila}");
                    if (indice != 0)
                    {
                        if (col == 0 && string.IsNullOrEmpty(valor))
                        {

                            indice = 0;
                        }
                        else
                        {
                            if (fila > 1)
                                await ValorColumnas(item, valor, fila, RutaArchivo, a, formatofecha);
                            else
                                LlenadoEstructuraExcel(listaExcel, col, fila, valor, item);
                            if (col == 12)
                            {
                                if (!string.IsNullOrEmpty(Error))
                                {
                                    LlenadoEstructuraExcel(listaExcel, 1, fila, Error.TrimEnd(','), "N");
                                    Error = "";
                                }
                                fila++;
                                await LeerArchivo(RutaArchivo, 0, fila, indice, ArchivoId, TipoId, origenId, formatofecha, Rutacompleta);
                            }
                        }
                    }
                    
                    col++;
                   
                }
                if (indice != 0)
                    await CrearAsyncActivo(a);
                
            }

          
        }

        private async Task<string> ValorColumnas(string columna, string valorColumna, int fila,string nombredocumento,Activo a,string formatofecha) 
        {
            int longitud = 0;
            if (!String.IsNullOrEmpty(valorColumna))
                 longitud = valorColumna.Length;
            switch (columna)
            {
                case "A":
                    if (!string.IsNullOrEmpty(valorColumna))
                    {
                        if (await ExisteEntrada(x => x.Clave.Equals(valorColumna, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            a.EntradaClasificacionId = ObtenerIdEntrada(ListaEntradas, valorColumna.Trim());
                        }
                        else
                            Error=$"{Error} Clave clasificación Incorrecta, ";
                    }

                    break;
                case "B":
                    if(string.IsNullOrEmpty(valorColumna))
                        Error = $"{Error} Nombre invalido,";
                    
                    if (longitud > LongitudDatos.Nombre)
                    {
                        a.Nombre = valorColumna.Substring(0, LongitudDatos.Nombre);
                        Error = $"{Error} Nombre ha sido cortado,";
                    }
                    else
                        a.Nombre = valorColumna;
                        break;
                case "C":
                    if (await ExisteIdUnico(valorColumna))
                        Error = $"{Error} El IdUnico repetido,";
                    if (longitud > LongitudDatos.IDunico)
                        {
                            a.IDunico = valorColumna.Substring(0, LongitudDatos.IDunico);
                            Error = $"{Error} El IdUnico ha sido cortado,";
                        }
                        else
                            a.IDunico = valorColumna;
                
                    break;
                case "D":
                    if(FechaValida(ObtenerFecha(valorColumna, formatofecha, fila,1)))
                        a.FechaApertura= Convert.ToDateTime(ObtenerFecha(valorColumna, formatofecha,fila,1)); 
                        valorColumna = a.FechaApertura.ToString();
                    break;
                case "E":
                    if (!string.IsNullOrEmpty(valorColumna))
                    {
                        if (FechaValida(ObtenerFecha(valorColumna, formatofecha, fila, 2)))
                            a.FechaCierre = Convert.ToDateTime(ObtenerFecha(valorColumna, formatofecha, fila, 2));
                        valorColumna = a.FechaCierre.ToString();
                    }
                    else
                        a.FechaCierre = null;
                    break;
                case "F":
                 
                    if (longitud > 512)
                    {
                        a.CodigoOptico = valorColumna.Substring(0, 512);
                    Error = $"{Error} El Código de barras ha sido cortado,";

                    }
                    else
                        a.CodigoOptico = valorColumna;
                    break;
                case "G":
                    
                    if (longitud > 512)
                    {
                        a.CodigoElectronico = valorColumna.Substring(0, 512);
                    Error = $"{Error} El Codigo  Eléctronico ha sido cortado,";

                    }
                    else
                        a.CodigoElectronico = valorColumna;
                    break;
                case "H":
                    if (!String.IsNullOrEmpty(valorColumna))
                        a.EsElectronico = true;
                    else
                        a.EsElectronico = false;

                    break;
                case "I":
                    if (!String.IsNullOrEmpty(valorColumna))
                        a.EnPrestamo = true;
                    else
                        a.EnPrestamo = false;

                    break;
                case "J":
                    if (!String.IsNullOrEmpty(valorColumna))
                        a.Reservado = true;
                    else
                        a.Reservado = false;

                    break;
                case "K":
                    if (!String.IsNullOrEmpty(valorColumna))
                        a.Confidencial = true;
                    else
                        a.Confidencial = false;

                    break;
                case "L":
                    if (!String.IsNullOrEmpty(valorColumna))
                        a.Ampliado = true;
                    else
                        a.Ampliado = false;

                    break;
                case "M":
                    
                    if (longitud > 2048) 
                    {
                        a.Asunto = valorColumna;
                        Error = $"{Error} El Asunto ha sido cortado,";
                    }
                    else
                        a.Asunto = valorColumna;

                    break;

                default:
                    break;
            }
            LlenadoEstructuraExcel(listaExcel, 0, fila, valorColumna, columna);

            return valorColumna;
        }
        private async Task<Activo> CrearAsyncActivo(Activo a) 
        {
            if (a.FechaApertura.Year > 1)
                if (!string.IsNullOrEmpty(a.EntradaClasificacionId)&& !string.IsNullOrEmpty(a.Nombre))
                {
                    await servicioActivos.ValidadorImportador(a);
                }
                else
                {
                    a = new Activo();
                }
            return a;
        }
        public async Task<bool> ExisteEEntrada(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            List<EntradaClasificacion> l = await this.RepoEntrada.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        private async Task<bool> ExisteIdUnico(string id) 
        {
            Activo a = await this.repo.UnicoAsync(x=>x.IDunico.Equals(id,StringComparison.InvariantCulture));
            if (a != null)
                return true;
            else
                return false;
        }
        public static bool FechaValida(string fecha)
        {
            DateTime fromDateValue;

            var formats = new[] { "M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt",
                     "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss",
                     "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt",
                     "M/d/yyyy h:mm", "M/d/yyyy h:mm",
                     "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm","dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "MM/dd/yyyy"};

            if (DateTime.TryParseExact(fecha, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string ObtenerFecha(string valor,string formatofecha,int fila,int indice) 
        {
            String date="";
            try
            {
                double s = Convert.ToDouble(valor);
                DateTime date2 = DateTime.FromOADate(s);
               date = date2.ToString("yyyy/MM/dd");
            }
            catch (Exception)
            {
                if (indice == 1)
                    Error = $"{Error} Fecha Apertura es invalida,";
                else
                    Error = $"{Error} Fecha Cierre es invalida,";

            }
            return date;
        }
        public static string ObtenerValorCelda(string fileName,
            string addressName)
        {
            string file = $"{fileName}";
            string value = null;
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(file, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;

                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();
               
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }

                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));
                WorkbookStylesPart styles = document.WorkbookPart.WorkbookStylesPart;
                Stylesheet stylesheet = styles.Stylesheet;
                CellFormats cellformats = stylesheet.CellFormats;
                Fonts fonts = stylesheet.Fonts;

                UInt32 fontIndex = fonts.Count;
                UInt32 formatIndex = cellformats.Count;
                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference == addressName).FirstOrDefault();
                if (theCell != null)
                {
                    value = theCell.InnerText;

                   
                    if (theCell.DataType != null)
                    {
                        switch (theCell.DataType.Value)
                        {
                            case CellValues.SharedString:

                                var stringTable =
                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();
                             
                                if (stringTable != null)
                                {
                                    value =
                                        stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                                }
                                break;

                            case CellValues.Boolean:
                                switch (value)
                                {
                                    case "0":
                                        value = "FALSE";
                                        break;
                                    default:
                                        value = "TRUE";
                                        break;
                                }
                                break;
                           
                        }
                    }
                }
              

            }
            return value; 

        }
        public async Task<byte[]> ImportandoDatos(byte[] file , string IdArchivo, string TipoId, string origenId,string formatofecha)
        {
            string ruta = configuracion.ruta_cache_fisico;
            string Rutacompleta = $"{ruta}{System.Guid.NewGuid().ToString()}.xlsx";
            crearruta(ruta);
            File.WriteAllBytes(Rutacompleta, file);
            ComunExcel c = new ComunExcel();

            ListaEntradas = await this.RepoEntrada.ObtenerAsync(x => x.Eliminada != true);
            LlenadoEstructuraExcel(listaExcel, 0, 1, "Estado", "N");
            await LeerArchivo(ruta, 0, 1, 1, IdArchivo, TipoId, origenId, formatofecha, Rutacompleta);
            string archivo = c.CrearArchivoExcel(listaExcel, IdArchivo, ruta, null, $"{System.Guid.NewGuid().ToString()}");
            byte[] b = File.ReadAllBytes(archivo);
            EliminarCache(ruta);
            return b;
        }
        private void crearruta(string ruta)
        {
            System.IO.Directory.CreateDirectory(ruta);
        }
        private void  EliminarCache(string ruta)
        {
          System.IO.Directory.Delete($"{ruta}", true);

        }
        private async Task<bool> ExisteEntrada(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            List<EntradaClasificacion> l = await this.RepoEntrada.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        private string ObtenerIdEntrada(List<EntradaClasificacion>listEC, string clave) 
        {

            string EntradaID = listEC.Where(x=>x.Clave.ToUpper().Equals(clave.ToUpper(),StringComparison.InvariantCulture)).Select(x=>x.Id).FirstOrDefault().ToString();
            foreach (EntradaClasificacion e in listEC)
            {
                if (e.Clave.ToUpper().Equals(clave.ToUpper(), StringComparison.InvariantCulture))
                    EntradaID = e.Id;
            }

            return EntradaID;


        }
       
        private void LlenadoEstructuraExcel(List<Estructuraexcel> exp, int NumeroCulumna, int NumeroRenglon, string ValorCelda,string Columna)
        {
            exp.Add(new Estructuraexcel
            {
                NumeroCulumna = NumeroCulumna,
                NumeroRenglon = NumeroRenglon,
                PosicionColumna = $"{Columna}",
                ValorCelda = ValorCelda

            });
          
        }
       

    }
}

