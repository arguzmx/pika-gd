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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;


namespace PIKA.Servicio.Contenido
{
    public class TareasEnDemanda : IProveedorTareasEnDemanda
    {

        public TareasEnDemanda()
        {

        }

        public const string TAREA_EXPPORTAR_PDF = "ContenidoPIKA.ExportarPDF";
        public const string TAREA_EXPPORTAR_ZIP = "ContenidoPIKA.ExportarZIP";

        public IInstanciaTareaBackground InstanciaTareaPDF(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var scope = serviceProvider.CreateScope();
            var elasticserv = serviceProvider.GetRequiredService<IRepoContenidoElasticSearch>();
            var vol = serviceProvider.GetRequiredService<IServicioVolumen>();
            var elemento = serviceProvider.GetRequiredService<IServicioElemento>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TareaExportarPDF>>();
            var opciones = scope.ServiceProvider.GetRequiredService<IOptions<ConfiguracionServidor>>();
            return new TareaExportarPDF(DominioId, Id, TokenSegumiento, logger, configuracion, vol, elemento, elasticserv, opciones, stoppingToken);

        }

        public IInstanciaTareaBackground InstanciaTareaZIP(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var scope = serviceProvider.CreateScope();
            var elasticserv = serviceProvider.GetRequiredService<IRepoContenidoElasticSearch>();
            var vol = serviceProvider.GetRequiredService<IServicioVolumen>();
            var elemento = serviceProvider.GetRequiredService<IServicioElemento>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TareaExportarZIP>>();
            var opciones = scope.ServiceProvider.GetRequiredService<IOptions<ConfiguracionServidor>>();
            return new TareaExportarZIP(DominioId, Id, TokenSegumiento, logger, configuracion, vol, elemento, elasticserv, opciones, stoppingToken);

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
            var assembly = Assembly.GetExecutingAssembly();
            tareas.Add(new TareaEnDemanda()
            {
                Id = TAREA_EXPPORTAR_PDF,
                NombreEnsamblado = $"{assembly.FullName.Split(',')[0]}.TareasEnDemanda, {assembly.FullName}",
                TipoRespuesta = TareaEnDemandaTipoRespuesta.BLOB,
                URLRecoleccion = "",
                Nombre = "Exportar a PDF",
                HorasCaducidad =72
            });

            tareas.Add(new TareaEnDemanda()
            {
                Id = TAREA_EXPPORTAR_ZIP,
                NombreEnsamblado = $"{assembly.FullName.Split(',')[0]}.TareasEnDemanda, {assembly.FullName}",
                TipoRespuesta = TareaEnDemandaTipoRespuesta.BLOB,
                URLRecoleccion = "",
                Nombre = "Exportar a ZIP",
                HorasCaducidad = 72
            });

            return tareas;
        }

        public IInstanciaTareaBackground InstanciaTarea(string DominioId, string IdProceso, string IdTarea,  string TokenSegumiento, IConfiguration configuracion, 
            IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            IInstanciaTareaBackground tarea = null;
            switch (IdProceso)
            {
                case TAREA_EXPPORTAR_PDF:
                    tarea = InstanciaTareaPDF(DominioId, IdTarea, TokenSegumiento, configuracion, serviceProvider, stoppingToken);
                    break;

                case TAREA_EXPPORTAR_ZIP:
                    tarea = InstanciaTareaZIP(DominioId, IdTarea, TokenSegumiento, configuracion, serviceProvider, stoppingToken);
                    break;
            }

            return tarea;
        }

        public async Task EliminaCaducos(string DominioId, string IdProceso, string IdTarea, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, 
            CancellationToken stoppingToken, string InputPayload, string OutputPayload)
        {

            ILogger<TareasEnDemanda> logger = null; ;

            try
            {
                var scope = serviceProvider.CreateScope();
                logger = scope.ServiceProvider.GetRequiredService<ILogger<TareasEnDemanda>>();

                IInstanciaTareaBackground tarea = null;
                switch (IdProceso)
                {
                    case TAREA_EXPPORTAR_PDF:
                        tarea = InstanciaTareaPDF(DominioId, IdTarea, TokenSegumiento, configuracion, serviceProvider, stoppingToken);
                        if (tarea != null)
                        {
                            await tarea.CaducarTarea(InputPayload, OutputPayload);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                if(logger != null)
                {
                    logger.LogError($"Error al procesar caduca {IdTarea}:{IdProceso}\r\nInput: {JsonSerializer.Serialize(InputPayload)}\r\nOutput: {JsonSerializer.Serialize(OutputPayload)}");
                }

            }
          
        }
    }
}
