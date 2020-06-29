using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// Rol laboral en la organización
    /// </summary>
    public class Rol : Entidad<string>, IEntidadNombrada, IEntidadRelacionada
    {

       [XmlIgnore]
       [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;


        public Rol() {
            this.UsuariosRol = new HashSet<UsuariosRol>();
        }


        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre del rol
        /// </summary>

        public string Nombre {get; set;}

        /// <summary>
        /// Descripción del rol
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Los roles se relacionan con un dominio o una unidad organizacional
        /// que representa el contexto de uso para el rol
        /// Los roles de de dominios son globales a la organización
        /// Los roles de unidad oerganizacional sólo son aplicables a la OU
        /// </summary>
        public string TipoOrigenId {get; set;}

        /// <summary>
        /// Identificador único del dominio que contiene los roles
        /// </summary>
        public string OrigenId {get; set;}

        
        /// <summary>
        /// USuarios participantes en el rol
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<UsuariosRol> UsuariosRol { get; set; }
    }
}
