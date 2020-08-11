using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Exportar_Importar
{
  public  class IOCuadroClasificacion
    {
        public void ExportarCuadroCalsificacionExcel(string id) 
        { 
        }

        public int NumeroCulumna { get; set; }
        public int NumeroRenglon { get; set; }
        public string PosicionColumna { get; set; }
        public string ValorRenglon { get; set; }
        public int Region { get; set; }

    }
}
