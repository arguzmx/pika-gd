﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class PermisoAplicacion: MascaraPermisos
    {
        /// <summary>
        /// Identificador unico del dominio para el conjunto de permisos
        /// </summary>
        public string DominioId { get; set; }

        /// <summary>
        /// Identificador único de la aplicación a la que pertenecen los módulos 
        /// </summary>
        public string AplicacionId { get; set; }

        /// <summary>
        /// Identificador único del módulo al que se dá em permiso
        /// </summary>
        public string ModuloId { get; set; }

        /// <summary>
        /// Tipo de entidad  asociada con el acceso
        /// </summary>
        public string TipoEntidadAcceso { get; set; }

        /// <summary>
        /// Identificador único de la entidad asociada al acceso
        /// </summary>
        public string EntidadAccesoId { get; set; }

    }
}
