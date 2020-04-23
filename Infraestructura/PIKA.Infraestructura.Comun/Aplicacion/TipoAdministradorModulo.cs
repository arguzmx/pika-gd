using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class TipoAdministradorModulo : Entidad<string>
    {

        /// <summary>
        /// Unique appliction ID
        /// </summary>
        public string AplicacionId { get; set; }

        /// <summary>
        /// Identificador único del modulo de la aplicación
        /// </summary>
        public string ModuloId { get; set; }

        public ModuloAplicacion ModuloApp { get; set; }
        /// <summary>
        /// Tipos del objeto administrador por el módulo
        /// </summary>
        public List<Type> TiposAdministrados { get; set; }


    }
}
