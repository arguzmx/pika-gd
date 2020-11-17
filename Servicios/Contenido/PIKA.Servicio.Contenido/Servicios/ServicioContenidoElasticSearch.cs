using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioContenidoElasticSearch: IRepositorioContenidoElasticSearch
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<ServicioContenidoElasticSearch> logger;
        private ElasticClient cliente;
        public ServicioContenidoElasticSearch(IConfiguration configuration, ILogger<ServicioContenidoElasticSearch> logger)
        {
            this.logger = logger;
            Configuration = configuration;
            ConfiguracionRepoContenido options = new ConfiguracionRepoContenido();

            Configuration.GetSection("RepositorioContenido").Bind(options);
            var settings = new ConnectionSettings(new Uri(options.CadenaConexion())).DefaultIndex("Pika-Contenido");
            cliente = new ElasticClient(settings);
        }
    }
}
