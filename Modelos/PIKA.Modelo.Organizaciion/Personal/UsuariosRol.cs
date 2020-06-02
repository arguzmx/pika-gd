using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Organizacion
{
    /// <summary>
    /// Vincula a los usuarios con los diferentes roles
    /// </summary>
    public class UsuariosRol
    {
        /// <summary>
        /// Identificador único del rol
        /// </summary>
        public string RolId { get; set; }

        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public string ApplicationUserId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Rol Rol { get; set; }

    }
}
