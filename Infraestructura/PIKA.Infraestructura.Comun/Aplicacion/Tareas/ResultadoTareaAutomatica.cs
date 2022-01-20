using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class ResultadoTareaAutomatica
    {
        public string Id { get; set; }
        public bool Exito { get; set; }
        public int Duracion { get; set; }
        public string Error { get; set; }

    }
}
