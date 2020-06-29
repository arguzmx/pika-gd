using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contacto
{
    [Entidad(EliminarLogico: false)]
    public class Estado : EntidadCatalogo <string, Estado>
    {

        /// <summary>
        /// Identificador del país de acuerdo al c´dogio ISO
        /// </summary>
        public string PaisId { get; set; }


        public override string Id { get; set; }


        public override string Nombre { get; set; }



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
