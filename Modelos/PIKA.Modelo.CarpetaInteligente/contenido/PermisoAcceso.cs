using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    /// <summary>
    /// Permisos de acceso a las entidades
    /// </summary>
    public class PermisoAcceso: Persona
    {
        /// <summary>
        /// Permite leer el contenido  y metadatos de la entidad
        /// </summary>
        public bool Lectura { get; set; }

        /// <summary>
        /// Permite modificar el contenido  y metadatos de la entidad
        /// </summary>
        public bool Escritura { get; set; }

        /// <summary>
        /// Permite crear entidaddes, su el contenido  y metadatos
        /// </summary>
        public bool Crear { get; set; }

        /// <summary>
        /// Permite eliminar el contenido  y metadatos de la entidad
        /// </summary>
        public bool Eliminar { get; set; }

    }
}
