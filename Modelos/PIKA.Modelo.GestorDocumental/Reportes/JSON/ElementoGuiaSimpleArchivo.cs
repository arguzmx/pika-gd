using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class ElementoGuiaSimpleArchivo
    {
        public string Serie { get; set; }
        public string Subserie { get; set; }
        public string Descripcion { get; set; }
        public string FechasLimites { get; set; }
        public int Cantidad { get; set; }
    }
}
