using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.TareasAutomaticas
{
    public class ServicioTareasAutomaticaBackgroud: BackgroundService
    {
        private const string MsgInicio = "Iniciando procesador de servicios background";
        private const string MsgParo = "Finalizando procesador de servicios background";
        private const string MsgEjecutar = "Ejecutando procesador de servicios background";
        private readonly ILogger<ServicioTareasAutomaticaBackgroud> _logger;

        public ServicioTareasAutomaticaBackgroud(IServiceProvider services,
            ILogger<ServicioTareasAutomaticaBackgroud> logger)
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
                        .GetRequiredService<ITareaAutomaticaBackgroud>();

                await scopedProcessingService.DoWork(stoppingToken).ConfigureAwait(false);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            
        }

    }
}
