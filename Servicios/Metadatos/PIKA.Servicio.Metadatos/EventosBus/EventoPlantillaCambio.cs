using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infrastructure.EventBus.Abstractions;
using PIKA.Infrastructure.EventBus.Events;
using PIKA.Modelo.Metadatos;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.EventosBus
{
    public class EventoPlantillaCambioHandler : IIntegrationEventHandler<EventoPlantillaCambio>
    {
        private readonly ILogger<EventoPlantillaCambioHandler> _logger;
        private readonly IAPICache<Plantilla> _cache;

        public EventoPlantillaCambioHandler(
            ILogger<EventoPlantillaCambioHandler> logger,
            IAPICache<Plantilla> cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
        }

        public async Task Handle(EventoPlantillaCambio  @event)
        {
            
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id} para la plantilla {@event.PlantillaId} "))
            {
                _logger.LogInformation("Eliminando cache para la plantilla {IdPlantilla}", @event.PlantillaId);
                await _cache.EliminaSiContiene(@event.PlantillaId);
                
            }
        }
        
    }


    public class EventoPlantillaCambio : IntegrationEvent
    {

        public string PlantillaId { get; set; }

        public EventoPlantillaCambio(string PlantillaId)
        {
            this.PlantillaId = PlantillaId;
        }
    }
}
