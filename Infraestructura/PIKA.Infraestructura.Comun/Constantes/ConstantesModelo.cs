using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class ConstantesModelo
    {
        /// <summary>
        /// Identificador de origen para elementos globales
        /// </summary>
        public const string IDORIGEN_NULO = "";

        /// <summary>
        /// Identificador de origen para elementos globales
        /// </summary>
        public const string IDORIGEN_GLOBAL = "global";


        /// <summary>
        /// Identificador de origen para elementos que se agrupan por dominio
        /// </summary>
        public const string IDORIGEN_DOMINIO = "dominio";

        /// <summary>
        /// Identificador de origen para elementos de la unidad organizacional
        /// </summary>
        public const string IDORIGEN_UNIDAD_ORGANIZACIONAL = "unidad-org";

        /// <summary>
        /// Identificador de origen para los usuarios del sistema
        /// </summary>
        public const string IDORIGEN_USUARIO = "usuario";

        /// <summary>
        /// Identificador de origen para un punto de montaje de contenido
        /// </summary>
        public const string IDORIGEN_PUNTO_MONTAJE = "pmontaje";

        public const string GLOBAL_DOMINIOID = "Global.IdDominio";
        public const string GLOBAL_UOID = "Global.IdUnidadOrganizacional";
        public const string CONTEXTO_ESRAIZ = "Contexto.EsRaiz";
        public const string PREFIJO_CONEXTO = "Contexto.";
    }
}
