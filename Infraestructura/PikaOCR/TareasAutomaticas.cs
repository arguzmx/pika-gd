using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PikaOCR
{
    public class TareasAutomaticas : IProveedorTareasAutomaticas
    {

        private const string TAREA_OCR= "PikaOCR.OCR";

        public TareasAutomaticas()
        {

        }

        public IInstanciaTareaBackground InstanciaTarea(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            switch(Id)
            {
                case TAREA_OCR:
                    return InstanciaTareaOCR(DominioId, Id, TokenSegumiento, configuracion, serviceProvider, stoppingToken);

                default:
                    return null;
            }

        }

    

        public IInstanciaTareaBackground InstanciaTareaOCR(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TareaAutomaticaOCR>>();
            var vol = scope.ServiceProvider.GetRequiredService<IServicioVolumen>();
            var eleserv = scope.ServiceProvider.GetRequiredService<IServicioElemento>();
            var elasticserv = scope.ServiceProvider.GetRequiredService<IRepoContenidoElasticSearch>();
            var opciones = scope.ServiceProvider.GetRequiredService<IOptions<ConfiguracionServidor>>();

            return new TareaAutomaticaOCR(DominioId, Id, TokenSegumiento, logger, configuracion, vol, elasticserv, eleserv, opciones, stoppingToken);
           
        }


        public List<TareaAutomatica> ObtieneTareasAutomaticas()
        {
            List<TareaAutomatica> Lista = new List<TareaAutomatica>();

            Lista.Add(new TareaAutomatica()
            {
                Id = TAREA_OCR,
                Nombre = "Proceso autómatico de extracción de texto y OCR",
                Periodo = PeriodoProgramacion.Continuo,
                FechaHoraEjecucion = new DateTime(2001, 1, 1, 0, 0, 00),
                HoraEjecucion = new DateTime(2001, 1, 1, 0, 0, 00),
                DiaMes = 0,
                DiaSemana = 0,
                Intervalo = 0,
                TareaEjecucionContinua = true,
                Estado = EstadoTarea.Habilidata,
                TareaEjecucionContinuaMinutos = 1
            });

            return Lista;
        }

    
    }
}
