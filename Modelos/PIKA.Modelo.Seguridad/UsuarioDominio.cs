using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Seguridad.Base
{
    public class UsuarioDominio
    {

        /// <summary>
        /// Especifica si el usuario e administardor del dominio
        /// </summary>
        public bool EsAdmin { get; set; }
        
        public string UnidadOrganizacionalId { get; set; }

        public string DominioId { get; set; }

        public string ApplicationUserId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
