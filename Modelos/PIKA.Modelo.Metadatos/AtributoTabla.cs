using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoTabla
    {
        public string PropiedadId { get; set; }
        public bool Incluir { get; set; }
        public bool Visible { get; set; }
        public bool Alternable { get; set; }
        public int IndiceOrdebnamiento { get; set; }
        public string IdTablaCliente { get; set; }
        public Propiedad Propiedad { get; set; }
    }
}
