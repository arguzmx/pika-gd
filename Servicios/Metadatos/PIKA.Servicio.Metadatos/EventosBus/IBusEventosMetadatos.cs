using PIKA.Infrastructure.EventBus.Abstractions;
using PIKA.Infrastructure.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.EventosBus
{
    public interface IBusEventosMetadatos
    {

        Task CambioPlantilla(string plantillaId);
    }
}
