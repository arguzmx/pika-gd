using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.TareasAutomaticas
{
    public class ServicioTareasEnDemandaBackgroud : BackgroundService
    {
        private const string MsgInicio = "Iniciando procesador de servicios en demanda background";
        private const string MsgParo = "Finalizando procesador de servicios en demanda background";
        private const string MsgEjecutar = "Ejecutando procesador de servicios en demanda background";
        private readonly ILogger<ServicioTareasEnDemandaBackgroud> _logger;

        public ServicioTareasEnDemandaBackgroud(IServiceProvider services,
            ILogger<ServicioTareasEnDemandaBackgroud> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(MsgInicio);

            await DoWork(stoppingToken).ConfigureAwait(false);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation(MsgEjecutar);

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<ITareaEnDemandaBackground>();

                await scopedProcessingService.DoWork(stoppingToken).ConfigureAwait(false);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            
        }

    }
}
