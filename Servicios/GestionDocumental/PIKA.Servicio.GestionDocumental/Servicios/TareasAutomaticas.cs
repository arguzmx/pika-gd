using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.TareasAutomaticas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class TareasAutomaticas : IProveedorTareasAutomaticas
    {
        public const string ESTDISTICA_ARCHIVO = "GestionDocumental.Estadistica";
        public IInstanciaTareaBackground InstanciaTarea(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            switch (Id)
            {
                case ESTDISTICA_ARCHIVO:
                    return InstanciaTareaEstadisticaArchivo(DominioId, Id, TokenSegumiento, configuracion, serviceProvider, stoppingToken);

                default:
                    return null;
            }

        }

        public IInstanciaTareaBackground InstanciaTareaEstadisticaArchivo(string DominioId, string Id, string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            var scope = serviceProvider.CreateScope();
            var archivos = serviceProvider.GetRequiredService<IServicioArchivo>();
            var estadisticas = serviceProvider.GetRequiredService<IServicioEstadisticaClasificacionAcervo>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TareaAutomaticaEstadisticaGuiaSimple>>();
            var opciones = scope.ServiceProvider.GetRequiredService<IOptions<ConfiguracionServidor>>();
            return new TareaAutomaticaEstadisticaGuiaSimple(DominioId, Id, TokenSegumiento, logger, configuracion, archivos, estadisticas, opciones, stoppingToken);
        }

        public List<TareaAutomatica> ObtieneTareasAutomaticas()
        {
            List<TareaAutomatica> Lista = new List<TareaAutomatica>();

            Lista.Add(new TareaAutomatica()
            {
                Id = ESTDISTICA_ARCHIVO,
                Nombre = "Actualización de estadísticas de gestión documental",
                Periodo = PeriodoProgramacion.Diario,
                FechaHoraEjecucion = new DateTime(2001, 1, 1, 2, 0, 0),
                HoraEjecucion = new DateTime(2001, 1, 1, 2, 0, 0),
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
