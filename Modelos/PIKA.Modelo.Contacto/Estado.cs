using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contacto
{
    public class Estado : EntidadCatalogo <string, Estado>
    {
        /// <summary>
        /// Identificador del esatdo de acuerdo al código ISO
        /// </summary>
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre del país en el idioma local
        /// </summary>
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        /// <summary>
        /// Identificador del país de acuerdo al c´dogio ISO
        /// </summary>
        public string PaisId { get; set; }

        /// <summary>
        /// Navegación
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual Pais Pais { get; set; }

        /// <summary>
        /// Navegacion
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<DireccionPostal> Direcciones { get; set; }
    }
}
