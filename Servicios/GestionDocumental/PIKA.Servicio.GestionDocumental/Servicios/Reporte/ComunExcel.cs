

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PIKA.Servicio.GestionDocumental
{
    public class ComunExcel
    {
        public List<Estructuraexcel> ListEstructuraExcel;
        public ComunExcel() 
        {
             ListEstructuraExcel = new List<Estructuraexcel>();
        }
        public bool ValidarRuta(string ruta)
        {
            if (File.Exists(ruta))
                return true;
            else
            {
               Directory.CreateDirectory(ruta);
                return false;
            }
        }

        public string   CrearArchivo(List<Estructuraexcel> ListaFile, string CuadroClasificacionId, string ruta, string separador, string nombre)
        {

            string fileName = $@"{ruta}{separador}{CuadroClasificacionId}{separador}{nombre}.xlsx";
           
            if (ValidarRuta($"{ruta}{separador}{CuadroClasificacionId}"))
            {
                return fileName;
            }

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

                int index;
                Cell cell;
              
                foreach (Estructuraexcel m in ListaFile)
                {
                    index = InsertSharedStringItem($"{m.ValorCelda}", shareStringPart);
                    cell = InsertCellInWorksheet($"{m.PosicionColumna}", (uint)m.NumeroRenglon, worksheetPart);
                    //agrega el valor a la celda recibida
                    cell.CellValue = new CellValue(index.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

                }
                //Guarda la  workbookpart.Workbook
                workbookpart.Workbook.Save();
                // cierra el documento
                spreadsheetDocument.Close();
                
                return fileName;
           

        }
        public string CrearArchivoExcel(List<Estructuraexcel> ListaFile,string id, string ruta, string? separador, string nombre)
        {
            string fileName;

            if (!string.IsNullOrEmpty(separador))
             fileName = $@"{ruta}{separador}{id}{separador}{nombre}.xlsx";
            else
                fileName= $@"{ruta}{nombre}.xlsx";

            if (ValidarRuta($"{ruta}{separador}{id}"))
            {
                return fileName;
            }
            else
            {

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
                //byte[] array = File.ReadAllBytes(fileName);
                return fileName;
            }

        }
        
        public string CreaArchivoExistente(List<Estructuraexcel> ListaFile, string id, string ruta, string nombre)
        {
           
                // Open the document for editing.
                using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(ruta, true))
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
                    WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart, nombre);

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
                return ruta;
            

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
        private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart,string nombre)
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

            string sheetName = nombre;

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
        public void CrearCeldas(int columna, int renglon, string NombreColumna, string texto)
        {
            ListEstructuraExcel.Add(new Estructuraexcel
            {
                NumeroCulumna = columna,
                NumeroRenglon = renglon,
                PosicionColumna = NombreColumna,
                ValorCelda = texto
            });
        }

        private void crearruta(string ruta)
        {
            System.IO.Directory.CreateDirectory(ruta);
        }
        public static byte[] UnirDocumentos(List<byte[]> ListaArchivos)
        {
            return CombinarDocumentos(ListaArchivos);
        }
        public static byte[] UnirDocumentos(List<string> ListaRutas)
        {
            List<byte[]> listafile = new List<byte[]>();
            foreach (string namefile in ListaRutas)
            {
                byte[] f = File.ReadAllBytes(namefile);
                listafile.Add(f);

            }
            return CombinarDocumentos(listafile);
        }
        public static byte[] CombinarDocumentos(List<byte[]> DocumentoFusionado)
        {
            int x = 0;
            byte[] fileOld = { };
            byte[] resultado = { };
            foreach (byte[] documentByteArray in DocumentoFusionado)
            {
                if (x == 0)
                { fileOld = documentByteArray; }
                else
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(documentByteArray, 0, (int)documentByteArray.Length);
                        using (WordprocessingDocument myDoc =
                    WordprocessingDocument.Open(stream, true))
                        {
                            string altChunkId = "AltChunkId1";
                            MainDocumentPart mainPart = myDoc.MainDocumentPart;
                            AlternativeFormatImportPart chunk =
                                mainPart.AddAlternativeFormatImportPart(
                                AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                            using (MemoryStream file = new MemoryStream(fileOld))
                                chunk.FeedData(file);
                            AltChunk altChunk = new AltChunk();
                            altChunk.Id = altChunkId;
                            mainPart.Document
                                .Body
                                .InsertBefore(altChunk, mainPart.Document.Body
                                .Elements<Paragraph>().First());
                            mainPart.Document.Save();
                        }

                        fileOld = stream.ToArray();
                        resultado = stream.ToArray();

                    }
                x++;

            }
            return resultado;
        }
    }
}
