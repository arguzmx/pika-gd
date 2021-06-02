using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Atributos
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class LinkMetadatosAttribute : Attribute
    {

        public const string IndiceFiltrado = "IF";
        public const string IndiceJerarquico = "IJ";

        private string _CampoMetadatos;
        public LinkMetadatosAttribute(string CampoMetadatos)
        {
            _CampoMetadatos = CampoMetadatos;
        }

        public virtual string CampoMetadatos
        {
            get { return _CampoMetadatos; }
        }

    }
}
