using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class GestorAzureConfig
    {
        public string VolumenId { get; set; }
        public string Endpoint { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public Volumen Volumen { get; set; }

    }
}
