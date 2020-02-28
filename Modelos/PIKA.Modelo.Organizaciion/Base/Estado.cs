using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion
{
    public class Estado : DuplaCatalogo<string, string>
    {

        public string PaisId { get; set; }
        public virtual Pais Pais { get; set; }

        public virtual ICollection<DireccionPostal> Direcciones { get; set; }
    }
}
