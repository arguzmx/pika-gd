using Microsoft.Extensions.Logging;
using PIKA.Infrastructure.EventBus.Abstractions;
using PIKA.Infrastructure.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.EventosBus
{
    public class BusEventosMetadatos: IBusEventosMetadatos, IEventBusService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<BusEventosMetadatos> _logger;
        public BusEventosMetadatos(IEventBus eventBus, ILogger<BusEventosMetadatos> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task CambioPlantilla(string plantillaId)
        {
            await Task.Delay(1);
            IntegrationEvent evt = new EventoPlantillaCambio(plantillaId) { PlantillaId = plantillaId };
            try
            {
                _eventBus.Publish(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", evt.Id, evt);
            }
            
        }
    }

}
