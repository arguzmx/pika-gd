using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class ProcesadorTareaAutomatica
    {

        public string Id { get; set; }
        public IInstanciaTareaAutomatica Instancia { get; set; }

        public int Index { get; set; }

        public bool EnEjecucion { get; set; }

        public DateTime? SiguienteEjecucion { get; set; }

        public string TokenSeguimiento { get; set; }
    }
}
