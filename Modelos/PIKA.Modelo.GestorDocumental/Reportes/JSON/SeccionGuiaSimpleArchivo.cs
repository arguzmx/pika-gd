using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class SeccionGuiaSimpleArchivo
    {

        public SeccionGuiaSimpleArchivo()
        {
            Elementos = new List<ElementoGuiaSimpleArchivo>();
        }

        public string Nombre { get; set; }

        public List<ElementoGuiaSimpleArchivo> Elementos { get; set; }
    }
}
