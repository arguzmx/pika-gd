using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class MetadatosOptions
    {
        public const string ELASTICSEARCH = "elasticsearch";

        public string Tipo { get; set; }
        public ElasticSearch ElasticSearch { get; set; }
    }

    public class ElasticSearch {
        public string Protocolo { get; set; }
        public int Puerto { get; set; }
        public string Url { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }

        public string CadenaConexion() {
            return $"{Protocolo}{ (!(string.IsNullOrEmpty(Usuario) && string.IsNullOrEmpty(Contrasena)) ? Usuario + ":" + Contrasena + "@" : "") }{Url}:{Puerto}";
        }
    }


}
