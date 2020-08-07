using System;
using System.Collections.Generic;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PIKA.Servicio.GestionDocumental
{
    public class ExporFile
    {
        public ExporFile()
        {
            
        }
        public int NumeroCulumna { get; set; }
        public int NumeroRenglon { get; set; }
        public string PosicionColumna { get; set; }
        public string ValorRenglon { get; set; }
        public int  Region { get; set; }
        public int hijos { get; set; }



       
        public void CrearArchivo() {
            SpreadsheetDocument Documento = SpreadsheetDocument.Create(@"C:\Arguz\pika-gd\Servicios\GestionDocumental\PIKA.Servicio.GestionDocumental\Data\Exportar_Importar\", SpreadsheetDocumentType.Workbook);

            WorkbookPart workbookpart = Documento.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            Sheets sheets = Documento.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            Sheet sheet = new Sheet() { Id = Documento.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
            sheets.Append(sheet);
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            Row row;
            row = new Row() { RowIndex = 1 };
            sheetData.Append(row);
            Cell refCell = null;
            foreach (Cell cell in row.Elements<Cell>())
            {
                if (string.Compare(cell.CellReference.Value, "A1", true) > 0)
                {
                    refCell = cell;
                    break;
                }
            }
            Cell newCell = new Cell() { CellReference = "A1" };
            row.InsertBefore(newCell, refCell);
            newCell.CellValue = new CellValue("100");
            newCell.DataType = new EnumValue<CellValues>(CellValues.Number);
            Documento.Close();
        }

    }

    
}
