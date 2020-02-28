using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// El dominio es el contenedor todos los recursos asociados a una organización
    /// </summary>
    public class Dominio : Entidad<string>, IEntidadNombrada
    {

        public Dominio() {
            UnidadesOrganizacionales = new HashSet<UnidadOrganizacional>();
        }

        public string Nombre { get; set; }

        /// <summary>
        /// Identificador de relación de origem, en este caso se utiliza
        /// para vincular la unidad con su dueño en el modo MultiTenant 
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identficador único del dueño, TENANT
        /// El Id de ralción es el identificador de un dominio
        /// </summary>
        public string OrigenId { get; set; }


        public ICollection<UnidadOrganizacional> UnidadesOrganizacionales { get; set; }
    }
}
