using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{

    /// <summary>
    /// Destinatario del permiso, puede ser por ejemplo un grupo o una cuenta de usurios
    /// </summary>
    public class DestinatarioPermiso
    {
        /// <summary>
        /// Identiicador único del permiso de contenido
        /// </summary>
        public string PermisoId { get; set; }

        /// <summary>
        /// Identificador único del destinatario del permiso
        /// </summary>
        public string DestinatarioId { get; set; }

        /// <summary>
        /// Indica si el permiso es aplciable a un grupo de usuarios
        /// </summary>
        public bool EsGrupo { get; set; }

        public Permiso Permiso { get; set; }
    }
}
