using Microsoft.Extensions.Configuration;
using Nest;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class RepoMetadatosElasticSearch: IRepositorioMetadatos
    {
        private readonly IConfiguration Configuration;
        private ElasticClient cliente;
        public RepoMetadatosElasticSearch(IConfiguration configuration)
        {
            Configuration = configuration;
            MetadatosOptions options = new MetadatosOptions();
            Configuration.GetSection("Metadatos").Bind(options);
            var settings = new ConnectionSettings(new Uri(options.ElasticSearch.CadenaConexion()));
            cliente = new ElasticClient(settings);

        }

        public bool Actualiza(Plantilla plantilla, ValoresPlantilla valores)
        {
            throw new NotImplementedException();
        }

        public List<ValoresPlantilla> Consulta(Plantilla plantilla, Consulta query)
        {
            throw new NotImplementedException();
        }

        public bool Elimina(Plantilla plantilla, string id)
        {
            throw new NotImplementedException();
        }

        public string Inserta(Plantilla plantilla, ValoresPlantilla valores)
        {
            throw new NotImplementedException();
        }

        public ValoresPlantilla Unico(Plantilla plantilla, string id)
        {
            throw new NotImplementedException();
        }
    }
}
