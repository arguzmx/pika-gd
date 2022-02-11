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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido
{
    public class TareasAutomaticas : IProveedorTareasAutomaticas
    {
        public const string TAREA_ESTADISTICA_VOLUMEN = "ContenidoPIKA.EstadisticaVolumenes";
        public IInstanciaTareaAutomatica InstanciaTarea(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            switch (Id)
            {
                case TAREA_ESTADISTICA_VOLUMEN:
                    return InstanciaTareaEstadisticaVols(DominioId, Id, TokenSegumiento, configuracion, serviceProvider, stoppingToken);

                default:
                    return null;
            }

        }
        public IInstanciaTareaAutomatica InstanciaTareaEstadisticaVols(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var scope = serviceProvider.CreateScope();
            var elasticserv = serviceProvider.GetRequiredService<IRepoContenidoElasticSearch>();
            var vol = serviceProvider.GetRequiredService<IServicioVolumen>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TareaAutomaticaEstadisticaVols>>();
            var opciones = scope.ServiceProvider.GetRequiredService<IOptions<ConfiguracionServidor>>();
            return new TareaAutomaticaEstadisticaVols(DominioId, Id, TokenSegumiento, logger, configuracion, vol, elasticserv, opciones, stoppingToken);
        }

        public List<TareaAutomatica> ObtieneTareasAutomaticas()
        {
            List<TareaAutomatica> Lista = new List<TareaAutomatica>();

            Lista.Add(new TareaAutomatica()
            {
                Id = TAREA_ESTADISTICA_VOLUMEN,
                Nombre = "Actualización de estadísticas de volúmenes de contenido",
                Periodo = PeriodoProgramacion.Diario,
                FechaHoraEjecucion = new DateTime(2001, 1, 1, 14, 12, 0),
                HoraEjecucion = new DateTime(2001, 1, 1, 14, 12, 0),
                DiaMes = 0,
                DiaSemana = 0,
                Intervalo = 1,
                TareaEjecucionContinua = false,
                Estado = EstadoTarea.Habilidata,
                TareaEjecucionContinuaMinutos = 0
            });

            return Lista;
        }
    }
}
