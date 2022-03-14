using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Text;

namespace PikaOCR
{
    public class ResultadoOCR
    {
        public ResultadoOCR()
        {
            Exito = false;
            Rutas = new List<string>();
        }

        public bool Exito { get; set; }
        public List<string> Rutas { get; set; }
        public Parte Parte { get; set; }
        public string NombreTemporal { get; set; }
    }
}
