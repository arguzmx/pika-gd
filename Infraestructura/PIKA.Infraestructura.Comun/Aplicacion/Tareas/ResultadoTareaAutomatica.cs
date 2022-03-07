using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class ResultadoTareaAutomatica
    {
        public ResultadoTareaAutomatica(string Id)
        {
            Exito = false;
            SegundosDuracion = 0;
            Error = null;
            this.Id = Id;
        }

        public string Id { get; set; }
        public bool Exito { get; set; }
        public int SegundosDuracion { get; set; }
        public string Error { get; set; }

        public string PayloadOutput { get; set; }

    }

}
