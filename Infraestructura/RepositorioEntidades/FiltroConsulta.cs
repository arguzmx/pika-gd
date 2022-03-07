using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{

    /// <summary>
    /// Establece un filtro para uan consulta, los valors por defecto son Negacion = false, NivelFuzzy = -1 y Operador = OP_EQ
    /// </summary>
    public class FiltroConsulta
    {
        public FiltroConsulta()
        {
            Negacion = false;
            NivelFuzzy = -1;
            Operador = OP_EQ;
        }

        /// <summary>
        /// Este valor es solo un atajo debe preferirse utilizar
        /// EQ combinado con la propiedad negacion
        /// </summary>
        public const string OP_NEQ = "neq";

        public const string SEPARADOR_VALORES = ",";
        public const string OP_EQ = "eq";
        public const string OP_GT = "gt";
        public const string OP_GTE = "gte";
        public const string OP_LT = "lt";
        public const string OP_LTE = "lte";
        public const string OP_EXIST = "exists";
        public const string OP_CONTAINS = "contains";
        public const string OP_STARTS = "starts";
        public const string OP_ENDS = "ends";
        public const string OP_ISNULL = "isnull";
        public const string OP_ISNOTNULL = "notnull";
        public const string OP_BETWEN = "between";
        public const string OP_FULLTEXT = "fulltext";
        public const string OP_INLIST = "inlist";

        /// <summary>
        /// Propiedad para aplicar el filtro
        /// </summary>
        public string Propiedad { get; set; }

        /// <summary>
        /// Operador aplicable a la operación
        /// </summary>
        public string Operador { get; set; }

        /// <summary>
        /// Indica si el filtro es la negación del operador seleccionado
        /// </summary>
        public bool Negacion { get; set; }

        /// <summary>
        /// Nivel para realizar busquedas fuzzy en campos de texto
        /// -1 indica no utilizado, 0=Defalt, los valroes pueden ir de 1 a 5
        /// https://www.elastic.co/guide/en/elasticsearch/reference/current/common-options.html#fuzziness
        /// </summary>
        public int NivelFuzzy { get; set; }

        /// <summary>
        /// En el caso de operadores binarios los valroes para la operación 
        /// deben estar separados por un caracter separador de valores que por default es ,
        /// </summary>
        public string Valor { get; set; }

        /// <summary>
        /// Lista concatenada de valores 
        /// </summary>
        public string ValorString { get; set; }
    }
}
