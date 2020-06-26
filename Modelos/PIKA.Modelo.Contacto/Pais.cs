using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contacto
{
    public class Pais : EntidadCatalogo<string, Pais>
    {
        public Pais()
        {

            Estados = new HashSet<Estado>();

        }


        /// <summary>
        /// Esatdos asociados al pais
        /// </summary>
        public virtual ICollection<Estado> Estados { get; set; }

        /// <summary>
        /// Navegacion
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<DireccionPostal> Direcciones { get; set; }

    }
}
