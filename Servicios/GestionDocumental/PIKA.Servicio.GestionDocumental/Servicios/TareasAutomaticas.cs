using Microsoft.Extensions.Configuration;
using PIKA.Infraestructura.Comun.Tareas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class TareasAutomaticas : IProveedorTareasAutomaticas
    {

        public IInstanciaTareaAutomatica InstanciaTarea(string DominioId, string Id,  string TokenSegumiento, IConfiguration configuracion, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            return null;
        }

        public List<TareaAutomatica> ObtieneTareasAutomaticas()
        {
            List<TareaAutomatica> Lista = new List<TareaAutomatica>();

            Lista.Add(new TareaAutomatica()
            {
                Id = "GestionDocumental.Estadistica",
                Nombre = "Actualización de estadísticas de gestión documental",
                Periodo = PeriodoProgramacion.Diario,
                FechaHoraEjecucion = new DateTime(2001, 1, 1, 0, 0, 30),
                HoraEjecucion = new DateTime(2001, 1, 1, 0, 0, 30),
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
