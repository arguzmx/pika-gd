using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    /// <summary>
    /// Permite definir los detalles para una aplicación 
    /// </summary>
    public class Aplicacion
    {
        public Aplicacion()
        {
            Traducciones = new HashSet<TraduccionAplicacionModulo>();
            Modulos = new HashSet<ModuloAplicacion>();
        }

        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public string UICulture { get; set; }

        public string Version { get; set; }

        public int ReleaseIndex { get; set; }

        public ICollection<TraduccionAplicacionModulo> Traducciones { get; set; }
        public ICollection<ModuloAplicacion> Modulos { get; set; }
    }
}
