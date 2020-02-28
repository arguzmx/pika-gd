using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class FiltroConsulta
    {
        public const string OP_EQ = "eq";
        public const string OP_NEQ = "neq";
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


        public string Propiedad { get; set; }
        public string Operador { get; set; }
        public string Valor { get; set; }
    }
}
