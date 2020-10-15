using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class ElementoGuiaSimpleArchivo
    {
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string FechaMinimaApertura { get; set; }
        public string FechaMaximaCierre { get; set; }
        public int Cantidad { get; set; }
    }
}
