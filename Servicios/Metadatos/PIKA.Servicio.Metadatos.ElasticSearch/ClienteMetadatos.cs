using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class ClienteMetadatos
    {
        public void Indexar()
        {
            string url = "http://localhost:9200/";
            string index = "datos";

            var settings = new ConnectionSettings(new Uri(url))
          .DefaultIndex(index);

            var client = new ElasticClient(settings);



        }

    }
}
