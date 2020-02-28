using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion
{
    public class Pais : DuplaCatalogo<string, string>
    {
        public Pais()
        {

            Estados = new HashSet<Estado>();

        }

        public virtual ICollection<Estado> Estados { get; set; }

        public virtual ICollection<DireccionPostal> Direcciones { get; set; }

    }
}
