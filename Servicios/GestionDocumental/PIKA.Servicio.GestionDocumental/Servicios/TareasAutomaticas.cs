using Microsoft.Extensions.Configuration;
using PIKA.Infraestructura.Comun.Tareas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class TareasAutomaticas : IProveedorTareasAutomaticas
    {
        public Task<ResultadoTareaAutomatica> EjecutarTarea(string DominioId, string Id, IConfiguration configuracion, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
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
                Intervalo =1
            }); ;

            return Lista;
        }
    }
}
