using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using PIKA.Servicio.Contenido.Servicios.TareasAutomaticas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PIKA.Servicio.Contenido
{
    public class TareasEnDemanda : IProveedorTareasEnDemanda
    {

        public const string TAREA_EXPPORTAR_PDF = "ContenidoPIKA.ExportarPDF";
        public IInstanciaTareaEnDemanda InstanciaTarea(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public IInstanciaTareaEnDemanda InstanciaTareaEstadisticaVols(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var scope = serviceProvider.CreateScope();
            var elasticserv = serviceProvider.GetRequiredService<IRepoContenidoElasticSearch>();
            var vol = serviceProvider.GetRequiredService<IServicioVolumen>();
            var elemento = serviceProvider.GetRequiredService<IServicioElemento>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TareaAutomaticaEstadisticaVols>>();
            var opciones = scope.ServiceProvider.GetRequiredService<IOptions<ConfiguracionServidor>>();
            return null;
           // return new TareaAutomaticaEstadisticaVols(DominioId, Id, TokenSegumiento, logger, configuracion, vol, elasticserv, opciones, stoppingToken);
        }

        public TareaEnDemanda ObtieneTarea(string Id)
        {
            var ts = ObtieneTareasEnDemanda();
            var t = ts.Where(x => x.Id == Id).SingleOrDefault();
            return t;
        }

        public List<TareaEnDemanda> ObtieneTareasEnDemanda()
        {
            List<TareaEnDemanda> tareas = new List<TareaEnDemanda>();

            tareas.Add(new TareaEnDemanda()
            {
                Id = TAREA_EXPPORTAR_PDF,
                NombreEnsamblado = Assembly.GetExecutingAssembly().FullName,
                TipoRespuesta = TareaEnDemandaTipoRespuesta.BLOB,
                URLRecoleccion = "",
                Nombre = "Exportar a PDF",
                HorasCaducidad =72
            });

            return tareas;
        }
    }
}
