using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class AtributoTabla: Entidad<string>
    {
        public string PropiedadId { get; set; }
        public bool Incluir { get; set; }
        public bool Visible { get; set; }
        public bool Alternable { get; set; }
        public int IndiceOrdebnamiento { get; set; }
        public string IdTablaCliente { get; set; }
        [NotMapped]
        public Propiedad Propiedad { get; set; }
        public AtributoTablaPropiedadPlantilla AtributosTablapropiedadplantilla { get; set; }
    }
}
