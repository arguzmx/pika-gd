using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// Rol laboral en la organización
    /// </summary>
    public class Rol : Entidad<string>, IEntidadNombrada, IEntidadRelacionada
    {

       
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;


        public Rol() {

            this.UsuariosRol = new HashSet<UsuariosRol>();
            this.TipoOrigenId = this.TipoOrigenDefault;
        }

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
        /// Identificador del rol padre en una estructura de jerarquía de roles
        /// </summary>
        public string RolPadreId { get; set; }


        /// <summary>
        /// Rol padre del rol actual
        /// </summary>
        public Rol RolPadre { get; set; }

        /// <summary>
        /// Roles dependientes del rol actual
        /// </summary>
        public ICollection<Rol> SubRoles { get; set; }

        /// <summary>
        /// USuarios participantes en el rol
        /// </summary>
        public ICollection<UsuariosRol> UsuariosRol { get; set; }
    }
}
