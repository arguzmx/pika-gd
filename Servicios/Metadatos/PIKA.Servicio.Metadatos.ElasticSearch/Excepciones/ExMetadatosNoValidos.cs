using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch.Excepciones
{
    public class ExMetadatosNoValidos: Exception
    {
        public ExMetadatosNoValidos() { }

        public ExMetadatosNoValidos(string msg ): base(msg) { }
    }
}
