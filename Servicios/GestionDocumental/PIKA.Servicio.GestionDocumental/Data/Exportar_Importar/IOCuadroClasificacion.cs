using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Servicios;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Exportar_Importar
{
  public  class Estructuraexcel
    {
        public void ExportarCuadroCalsificacionExcel(string id) 
        { 
        }
        /// <summary>
        /// Idenificador del nùmero de columna
        /// </summary>
        public int NumeroCulumna { get; set; }
        /// <summary>
        /// Identidicador de Nùmero de Fila o renglòn
        /// </summary>
        public int NumeroRenglon { get; set; }
        /// <summary>
        /// La posiciòn coresponde a la letra de la columna que se asigna por medio del nivel obtenido en la regiòn 1
        /// </summary>
        public string PosicionColumna { get; set; }
        /// <summary>
        /// Valor de Renglon o el contenido que tendra el texto.
        /// </summary>
        public string ValorCelda { get; set; }
  

    }
    public class CrearDocumento {
        
        public bool ValidarRuta(string ruta) {
            if (File.Exists(ruta))
                return true;

            else
            {
                System.IO.Directory.CreateDirectory(ruta);
                return false;
            }
        }

        public async Task<Array> CrearArchivo(List<Estructuraexcel> ListaFile,string CuadroClasificacionId,string ruta,string separador,string nombre)
        {
            byte[] byteArray;
          
            string fileName = $@"{ruta}{separador}{CuadroClasificacionId}{separador}{nombre}.xlsx";


            if (ValidarRuta($"{ruta}{separador}{CuadroClasificacionId}"))
            {
                byte[] arrayFile = File.ReadAllBytes(fileName);
                return arrayFile;
            }
            else { 
                
            // Crea un spreadsheet del documento con la ruta establecida.
            // lo autoguarda y edita con el tipo de extensión Xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);

                // agrega una WorkbookPart al documento.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();
                // agrega una WorksheetPart dentro del  WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());
                // Agrga una Sheets dentro del libro de trabajo.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());
                // Agregar un nuevo worksheet y lo asocia al libro de trabajo.
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = nombre
                };
                sheets.Append(sheet);
                // obtiene el SharedStringTablePart. si no existe crea uno nuevo.
                SharedStringTablePart shareStringPart;
                if (spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadsheetDocument.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadsheetDocument.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                int index = 0;
                Cell cell = new Cell();
                // inserta el  text dentro del SharedStringTablePart.
                foreach (Estructuraexcel m in ListaFile)
                {
                    index = InsertSharedStringItem($"{m.ValorCelda}", shareStringPart);
                    // inserta una cell A1 dentro de la nueva worksheet.
                    cell = InsertCellInWorksheet($"{m.PosicionColumna}", (uint)m.NumeroRenglon, worksheetPart);
                    //agrega el valor a la celda recibida
                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                }
                //Guarda la  workbookpart.Workbook
                workbookpart.Workbook.Save();
                // cierra el documento
                spreadsheetDocument.Close();
            byte[] array = File.ReadAllBytes(fileName);
            return array;
            }

        }

       
        public void CreaArchivoExistente(List<Estructuraexcel> ListaFile,string NombreArchivo) {
            // Open the document for editing.
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(NombreArchivo, true))
            {
                // Get the SharedStringTablePart. If it does not exist, create a new one.
                SharedStringTablePart shareStringPart;
                if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }
                int index = 0;
                Cell cell = new Cell();
                // Insert the text into the SharedStringTablePart.
                WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart);

                foreach (Estructuraexcel m in ListaFile)
                {
                    index = InsertSharedStringItem($"{m.ValorCelda}", shareStringPart);
                    // Insert cell A1 into the new worksheet.
                    cell = InsertCellInWorksheet($"{m.PosicionColumna}", (uint)m.NumeroRenglon, worksheetPart);
                    // Set the value of cell A1.
                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                }

                // Save the new worksheet.
                worksheetPart.Worksheet.Save();
                // Close the document.
                spreadSheet.Close();
            }

        }
        /// <summary>
        // recibe un SharedStringTablePart, crea el texto SharedStringItem con el texto enviado para cada celda 
        // lo inserta en SharedStringTablePart. si ya existe regresa un indice
        /// </summary>
        /// <param name="text"></param>
        /// <param name="shareStringPart"></param>
        /// <returns></returns>
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // si la parte no contiene un SharedStringTable, crea uno.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // itera en todos los items del SharedStringTable. si el txto ya existe regresa el indice donde se encuentra.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }
            // si no existe lo crea y regresa el indice
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }
        /// <summary>
        //recibe un WorkbookPart, inserta un nuevo worksheet.
        // 
        /// </summary>
        /// <param name="workbookPart">recibe un WorkbookPart, inserta un nuevo worksheet.</param>
        /// <returns></returns>
        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart)
        {
            // agrega un nuevo worksheet en el workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Obtiene el Id de la Sheet(hoja).
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet";

            // abre un nuevo worksheet y lo asicia con el workbook(libro).
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }
        
        /// <summary>
        /// recibe el nombre de la columna con su respectivo indice , 
        /// recibe el la parte del libro donde se trabajara, inserta en la celda y regresa esta
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="worksheetPart"></param>
        /// <returns></returns>
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // valida si el libro worksheet no contiene la fila con las especificacione del index e inserta una.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {

                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            //valida si esta no contiene las espeficicaciones de la columna,  inserta una columna
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName).Count() > 0)
            {

                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                //se inserta una nueva columna con las especificaciones recibidas
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
       /// <summary>
       /// Esta función busca el indice en de la A a la Z del alfabeto y
       /// retorna el valor del abecedario que corresponde al indice ingresado
       /// </summary>
       /// <param name="indice">Indice de la detra el alfabeto</param>
       /// <returns></returns>
        public string GetAbecedario(int indice)
        {
            indice--;
            String col = Convert.ToString((char)('A' + (indice % 26)));
            while (indice >= 26)
            {
                indice = (indice / 26) - 1;
                col = Convert.ToString((char)('A' + (indice % 26))) + col;
            }
            return col;
        }

    }
    public class RegionElementoClasificacion
    {
        /// <summary>
        /// Identificador ùnico del Elemento.
        /// </summary>
        public string ElementoId { get; set; }
        /// <summary>
        /// Identificador ùnico del Cuadro Clasificaciòn
        /// </summary>
        public string CuadroClasificacionId { get; set; }
        /// <summary>
        /// Nivel del Elemento,esto representa la posicion de la columna que le correspode en el excel.
        /// </summary>
        public int Nivel { get; set; }
        /// <summary>
        /// Si el elemento esta eliminado agrega un 1 de lo contrario un 0
        /// </summary>
        public int Eliminado { get; set; }
        /// <summary>
        /// Esta propiedad representa la uniòn entre Nombre y clave del Elemento
        /// </summary>
        public string NombreClave { get; set; }

    }

    public class IOCuadroClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable
    {
        private IRepositorioAsync<CuadroClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<EntradaClasificacion> RepoEntrda;
        private IRepositorioAsync<TipoArchivo> repoTA;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private IRepositorioAsync<TipoValoracionDocumental> repoTVD;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoTV;
        private IRepositorioAsync<ElementoClasificacion> RepoElemento;
        public IOCuadroClasificacion(ILogger<ServicioCuadroClasificacion> Logger, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones) 
            : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.RepoEntrda = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoTA = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
            this.repoTD = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoTVD = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
            this.repoTV = UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
            this.RepoElemento = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
           

        }


        /// <summary>
        /// Regresa una lista de la entidad Elementos Clasificación obtenio los hijos de cada elemento
        /// </summary>
        /// <param name="PadreId">Identificador único de los elementos</param>
        /// <param name="JerquiaId">Identificador único de cuadro clasificación</param>
        /// <returns></returns>
        private async Task<List<ElementoClasificacion>> ObtenerHijosAsync(string PadreId, string JerquiaId)
        {
            var l = await this.RepoElemento.ObtenerAsync(x => x.CuadroClasifiacionId == JerquiaId &&x.Eliminada!=true
           && x.ElementoClasificacionId == PadreId, y => y.OrderBy(z => z.NombreJerarquico));
            return l.ToList();
        }
      /// <summary>
      /// Crea el archivo xml
      /// </summary>
      /// <param name="CuadroClasificacionId"></param>
      /// <returns></returns>
        public async Task<Array> ExportarCuadroCalsificacionExcel(string CuadroClasificacionId,string ruta,string separador)
        {
            byte[] byteArray;
            CrearDocumento CD = new CrearDocumento();
            List<Estructuraexcel> listExport = new List<Estructuraexcel>();

            string CuadroClasificaiconId = CuadroClasificacionId;
                await LlenadoCuadroClasificacion(CuadroClasificacionId, listExport);

           return await CD.CrearArchivo(listExport, CuadroClasificaiconId, ruta,separador, await ObtineNombreCuadroClasificacion(CuadroClasificaiconId));
         

        }
        /// <summary>
        /// Elemina la ruta del cuadro clasificacion siempre y cuando se haga un cambbio a la estructura del cuadro clasificación
        /// </summary>
        /// <param name="CuadroClasificacionId">Identificador unico </param>
        /// <param name="ruta">Ruta fisica</param>
        /// <param name="separador">separador de ruta</param>
        /// <returns></returns>
        public async Task EliminarCuadroCalsificacionExcel(string CuadroClasificacionId, string ruta, string separador) 
        {
            CuadroClasificacion cc = await this.repo.UnicoAsync(x=>x.Id.Equals(CuadroClasificacionId,StringComparison.InvariantCultureIgnoreCase));
          if(cc!=null)
            System.IO.Directory.Delete($"{ruta}{separador}{CuadroClasificacionId}",true);

        }
        /// <summary>
        /// Obtiene el nombre del cuadro clasificación
        /// </summary>
        /// <returns></returns>
        private async Task<string> ObtineNombreCuadroClasificacion(string CuadroClasificacionId) 
        {

            CuadroClasificacion cc = await this.repo.UnicoAsync(x=>x.Id.Equals(CuadroClasificacionId,StringComparison.InvariantCultureIgnoreCase));
            if (cc != null)
                return cc.Nombre;
            else
                return "Sin Nombre";
        }
        /// <summary>
        /// llenado del cuadro clasidicación
        /// </summary>
        /// <param name="CuadroClasificacionId"></param>
        /// <param name="listExport"></param>
        /// <returns></returns>
        private async Task<string> LlenadoCuadroClasificacion(string CuadroClasificacionId, List<Estructuraexcel> listExport) {
            int columna = 1;
            int Row = 2;
            string nombre="";
            CuadroClasificacion cuadroclasificacion = await this.repo.UnicoAsync(x=> x.Id.Equals(CuadroClasificacionId,StringComparison.InvariantCultureIgnoreCase));
            List<RegionElementoClasificacion> lr1 = new List<RegionElementoClasificacion>();

            if (cuadroclasificacion != null)
            {
                nombre = cuadroclasificacion.Nombre;
                LlenadoEstructuraExcel(listExport, columna, Row, $" {cuadroclasificacion.Nombre} ");
                Row = Row + 3;
                await ObtnerHijosElmentos(lr1, columna, cuadroclasificacion.Id, null);
            }

           await LlenadoRegion2y3(lr1, listExport);

            return nombre;
        }
        /// <summary>
        /// Llenado de la region 2 y 3
        /// </summary>
        /// <param name="lr1">lista de elementos </param>
        /// <param name="listExport">lista del llenado por celdas</param>
        /// <returns></returns>
        private async Task LlenadoRegion2y3(List<RegionElementoClasificacion> lr1, List<Estructuraexcel> listExport) 
        {
            int contador = 4;
            foreach (RegionElementoClasificacion r in lr1)
            {
                LlenadoEstructuraExcel(listExport, r.Nivel, contador, $" {r.NombreClave} ");
                List<EntradaClasificacion> lisentrada = await this.RepoEntrda.ObtenerAsync(x => x.ElementoClasificacionId.Equals
                (r.ElementoId, StringComparison.InvariantCultureIgnoreCase));
                if (lisentrada.Count() > 0)
                    await encabezados(listExport, lr1.Max(x => x.Nivel));
                foreach (EntradaClasificacion entradas in lisentrada)
                {
                    await InsertaValoresCuadro2(listExport, entradas, lr1.Max(x => x.Nivel), contador);
                    contador++;
                }
                contador++;
            }
        }

        /// <summary>
        /// Crea los Encabezados de la region 2 y 3 de las entidades EntradaCuadroClasificación y Tipo valoración
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private async Task encabezados(List<Estructuraexcel> exp, int c)
        {
            int row = 2;
            
            for (int i = 1; i < 7; i++)
            {
                c = c + 1;
                LlenadoEstructuraExcel(exp, c, row, ValorEncabezado(i));
            }

            List<TipoValoracionDocumental> list = await this.repoTVD.ObtenerAsync(x => x.Id != null, include: null);

            foreach (TipoValoracionDocumental tp in list)
            {
                c = c + 1;
                LlenadoEstructuraExcel(exp,c,row,tp.Nombre);
               
            }


        }
        /// <summary>
        /// /Esta funsión retorna un string que será el nombre que llevará el encabezado de la región 2
        /// </summary>
        /// <param name="indice">Identificar de la celda para cada columna de la region 2 del excel</param>
        /// <returns></returns>
        private string ValorEncabezado(int indice) 
        {
            string text = "";
            switch (indice)
            {
                case 1:
                    text = EncabezadoTabla.NombreEntrada;
                    break;
                case 2:
                    text = EncabezadoTabla.AT;
                    break;
                case 3:
                    text = EncabezadoTabla.AC;
                    break;
                case 4:
                    text = EncabezadoTabla.AH;
                    break;
                case 5:
                    text = EncabezadoTabla.Eliminado;
                    break;
                case 6:
                    text = EncabezadoTabla.Tipo_dis;
                    break;
            }
            return text;
        }
        /// <summary>
        /// Esta función esta diseñada para llenar una lista por cada una de las celdas que serán imprimidas en el archivo
        /// </summary>
        /// <param name="exp">lista de estructura del excel</param>
        /// <param name="NumeroCulumna">Numero de Columna</param>
        /// <param name="NumeroRenglon">numero de Renglón</param>
        /// <param name="PosicionColumna">Posición de la columna</param>
        /// <param name="ValorCelda">Valor de la </param>
        private void LlenadoEstructuraExcel(List<Estructuraexcel> exp, int NumeroCulumna,int NumeroRenglon, string ValorCelda) 
        {
            exp.Add(new Estructuraexcel
            {
                NumeroCulumna = NumeroCulumna,
                NumeroRenglon = NumeroRenglon,
                PosicionColumna = $"{GetAbecedario(NumeroCulumna)}",
                ValorCelda = ValorCelda

            });
        }
        /// <summary>
        /// Llena la lista de estructura del excel para el llenado de celdas
        /// </summary>
        /// <param name="exp">lista de datos</param>
        /// <param name="EntradasClasificion">entidad de entrada clasificacion</param>
        /// <param name="IndiceColumna">Indice de la columna</param>
        /// <param name="NumeroFila">Numero de Fila para la celda</param>
        /// <param name="PosicionColumna"></param>
        /// <returns></returns>
        private async Task InsertaValoresCuadro2(List<Estructuraexcel> exp, EntradaClasificacion EntradasClasificion, int IndiceColumna, int NumeroFila)
        {
            string nombreclave = $"{ EntradasClasificion.Clave}{ EntradasClasificion.Nombre}";
            for (int i = 1; i < 7; i++)
            {
                IndiceColumna = IndiceColumna + 1;
                if (i == 2)
                { nombreclave = EntradasClasificion.MesesVigenciTramite.ToString(); }
                if (i == 3)
                { nombreclave = EntradasClasificion.MesesVigenciConcentracion.ToString(); }
                if (i == 4)
                { nombreclave = EntradasClasificion.MesesVigenciHistorico.ToString(); }
                if (i == 5)
                { nombreclave = EntradasClasificion.Eliminada ? "Eliminado" : ""; }
                if (i == 6)
                {
                    TipoDisposicionDocumental tp = await this.repoTD.UnicoAsync(x => x.Id == EntradasClasificion.TipoDisposicionDocumentalId);
                    if (tp != null)
                    {
                        if (!String.IsNullOrEmpty(tp.Nombre))
                            nombreclave = tp.Nombre;
                        else
                            nombreclave = "sin Tipo Disposición";
                    }
                }
                LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, nombreclave);

            }
            List<TipoValoracionDocumental> list = await this.repoTVD.ObtenerAsync(x => x.Id != null, include: null);

            foreach (TipoValoracionDocumental tp in list)
            {
                IndiceColumna = IndiceColumna + 1;
                List<ValoracionEntradaClasificacion> vl = await this.repoTV.ObtenerAsync(x => x.TipoValoracionDocumentalId.Equals(tp.Id) && x.EntradaClasificacionId.Equals(EntradasClasificion.Id));
                if (vl.Count() > 0)
                {
                    LlenadoEstructuraExcel(exp, IndiceColumna, NumeroFila, "X");
                }

            }
        }

        /// <summary>
        /// devuelve un string con la lectra del alfabeto que se obtiene el indice recibido
        /// </summary>
        /// <param name="indice">Identificar del Indice de la posición del alfabeto</param>
        /// <returns></returns>
        private string GetAbecedario(int indice)
        {
            indice--;
            String col = Convert.ToString((char)('A' + (indice % 26)));
            while (indice >= 26)
            {
                indice = (indice / 26) - 1;
                col = Convert.ToString((char)('A' + (indice % 26))) + col;
            }
            return col;
        }
        /// <summary>
        /// Obtiene todos los niveles y los hijos de cada elemento
        /// </summary>
        /// <param name="lr1">lista de región </param>
        /// <param name="Indicecolumna">Identificador del indice de la columna</param>
        /// <param name="CuadroClasificacionId">Identificador único del cuadro Clasificación</param>
        /// <param name="ElementoClasificacionId">Identificador único del elemento clasificación</param>
        /// <returns></returns>
        private async Task ObtnerHijosElmentos(List<RegionElementoClasificacion> lr1, int Indicecolumna, string CuadroClasificacionId, string? ElementoClasificacionId)
        {
            if (String.IsNullOrEmpty(ElementoClasificacionId))
            {
                Indicecolumna = Indicecolumna - 1;
                
            }

            List<ElementoClasificacion> ListaElemntosHijos = new List<ElementoClasificacion>();
            ListaElemntosHijos = await ObtenerHijosAsync(ElementoClasificacionId, CuadroClasificacionId);

            if (ListaElemntosHijos.Count() > 0)
            { Indicecolumna++;  }

            foreach (ElementoClasificacion hijos in ListaElemntosHijos)
            {
                LlenadoRegion1(lr1,Indicecolumna, $" {hijos.Clave} {hijos.Nombre} ",hijos.CuadroClasifiacionId,hijos.Id);
                await ObtnerHijosElmentos(lr1, Indicecolumna, hijos.CuadroClasifiacionId, hijos.Id);
            }
            
            
           

        }
        /// <summary>
        /// Se llena la clase Región1 para obtener todos los niveles o columnas que se deben considerar.
        /// </summary>
        /// <param name="r1">lista de la región</param>
        /// <param name="columna">Numero de columna por hijo encontrado si este tiene hijos le agrega una columna por cada padre que encuentre</param>
        /// <param name="nombreClave">Parametro donde se concatena el nombre y la clave del elemento</param>
        /// <param name="CuadroClasificacionId">Identificador único del cuadro clasificación</param>
        /// <param name="ElementoClasificacionId">Identificador Unico del elemento clasificacion</param>
        /// <returns></returns>
        private List<RegionElementoClasificacion> LlenadoRegion1(List<RegionElementoClasificacion> r1, int columna,string nombreClave,string CuadroClasificacionId,string ElementoClasificacionId ) {
            r1.Add(new RegionElementoClasificacion
            {
                Nivel = columna,
                NombreClave = nombreClave ,
                CuadroClasificacionId = CuadroClasificacionId,
                ElementoId = ElementoClasificacionId
            });
            return r1;
        }
       
    }
}
