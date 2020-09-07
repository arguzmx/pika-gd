using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class AppGrant
    {
        /// <summary>
        /// Unique appliction ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Identificador único del modulo de la aplicación
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Almacena la máscara de permisos de acceso
        /// </summary>
        public ulong GrantMask { get; set; }


        /// <summary>
        /// Tipo de objeto asicoado al permiso por ejemplo rol o usuario 
        /// en base a las contasntes de ConstantesSeguridad
        /// </summary>
        public string GrantedType { get; set; }

        /// <summary>
        /// Identificador único del objeto al cual ha sido asignado el permiso
        /// </summary>
        public string GrantedId { get; set; }


    }
}
