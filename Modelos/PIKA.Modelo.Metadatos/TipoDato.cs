using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class TipoDato : Entidad<string>
    {
        public const string tString = "string";
        public const string tDouble = "double";
        public const string tBoolean = "bool";
        public const string tInt32 = "int";
        public const string tInt64 = "long";
        public const string tDateTime = "date";
        public const string tBinaryData = "bin";

        /// <summary>
        /// Identificador único del tipo de propiedad
        /// </summary>
        public override string Id { get; set; }


        /// <summary>
        /// Nombre para despliegue del tipo de propiedad
        /// </summary>
        public string Nombre { get; set; }


    }
}
