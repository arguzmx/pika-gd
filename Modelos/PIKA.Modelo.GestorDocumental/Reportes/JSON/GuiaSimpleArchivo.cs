using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class GuiaSimpleArchivo
    {
        public GuiaSimpleArchivo()
        {
            Elementos = new List<ElementoGuiaSimpleArchivo>();
        }
        public Archivo Archivo { get; set; }
        public List<ElementoGuiaSimpleArchivo> Elementos { get; set; } 
    }
}
