using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class TipoAlmacenMetadatos: EntidadCatalogo<string, TipoAlmacenMetadatos>
    {

        public const string tElasticSearch = "esearch";
        public const string tSQL = "sql";


        public override List<TipoAlmacenMetadatos> Seed()
        {
            List<TipoAlmacenMetadatos> l = new List<TipoAlmacenMetadatos>();

            l.Add(new TipoAlmacenMetadatos() { 
             Id = tElasticSearch, Nombre ="Elasticsearch"
            });

            l.Add(new TipoAlmacenMetadatos()
            {
                Id = tSQL,
                Nombre = "Base de datos SQL"
            });

            return l;
        }
   public string AsociacionPlantillaid { get; set; }
    public ICollection< AsociacionPlantilla> AsociacionesPlantilla { get; set; }
    }
}
