using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class ProcesadorTareaBackground: IDisposable
    {

        public string Id { get; set; }
        public IInstanciaTareaBackground Instancia { get; set; }

        public int Index { get; set; }

        public bool EnEjecucion { get; set; }

        /// <summary>
        /// Utilizado para las tareas de ejecución automática
        /// </summary>
        public DateTime? SiguienteEjecucion { get; set; }

        public string TokenSeguimiento { get; set; }

        public void Dispose()
        {
            if(Instancia!= null)
            {
                Instancia.Dispose(); ;
            }
        }
    }
}
