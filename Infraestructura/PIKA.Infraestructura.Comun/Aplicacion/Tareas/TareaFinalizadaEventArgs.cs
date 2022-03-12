using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class TareaFinalizadaEventArgs: EventArgs
    {
        public string TokenSeguimiento { get; set; }
        public ResultadoTareaBackground Resultado { get; set; }
    }
}
