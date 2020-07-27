using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class GestorSMBConfig
    {
        public string VolumenId { get; set; }
        public string Ruta { get; set; }
        public string Dominio { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public Volumen Volumen { get; set; }

    }
}
