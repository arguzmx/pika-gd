using FluentValidation;
using LazyCache;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Tareas;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Metadatos.Interfaces;
using PIKA.ServicioBusqueda.Contenido;
using PikaOCR;
using RepositorioEntidades;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .CreateLogger();

            var nodb = args.Contains("/nodb");
            var demodb = args.Contains("/demodb");
  
            try
            {
                Log.Information("Iniciando PIKA-GD-API");
                var host = BuildWebHost(args);
                var environment = host.Services.GetService<IWebHostEnvironment>();
                var config = host.Services.GetRequiredService<IConfiguration>();

                if (!nodb)
                {
                    InicializarAplication(config, environment, demodb);
                    await InicializarAplicationAutoConfigurable(environment, host, demodb).ConfigureAwait(false);
                    await InicializaTareasAutomaticas(config, environment, host, demodb).ConfigureAwait(false);
                    InicializaEslasticSearch(config, host);
                }
                else
                {
                    Log.Information("Saltando la configuración de de datos de la aplicación");
                }


                host.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }



        }

        private static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .CaptureStartupErrors(false)
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.local.json", optional: true);
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<OCRHostedService>();
            })
            .UseStartup<Startup>()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseSerilog()
            .Build();


        private static void InicializaEslasticSearch(IConfiguration configuracion, IWebHost host)
        {
            var IlogFactory = host.Services.GetRequiredService<ILoggerFactory>();
            var IpOpciones = host.Services.GetRequiredService<IProveedorOpcionesContexto<DbContextBusquedaContenido>>();
            var ICache = host.Services.GetRequiredService<IAppCache>();
            var IRepositorioMetadatos = host.Services.GetRequiredService<IRepositorioMetadatos>();
            var IServicioPlantila = host.Services.GetRequiredService<IServicioPlantilla>();
            var IRepoContenidoElasticSearch = host.Services.GetRequiredService<IRepoContenidoElasticSearch>();
            ServicioBusquedaContenido s = new ServicioBusquedaContenido(IServicioPlantila, IRepositorioMetadatos, IRepoContenidoElasticSearch, ICache, IpOpciones, configuracion, IlogFactory);
            s.Inicializar("", false); ;
        }

        private static void InicializarAplication(IConfiguration configuracion, IWebHostEnvironment env, bool demodb)
        {
            List<Type> contextos = LocalizadorEnsamblados.ObtieneContextosInicializables();
            string connectionString = configuracion.GetConnectionString("pika-gd");
            foreach (Type dbContextType in contextos)
            {
                var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
                var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType);
                optionsBuilder.UseMySql(connectionString);
                var dbContext = (DbContext)Activator.CreateInstance(dbContextType, optionsBuilder.Options);
                try
                {
                    ((IRepositorioInicializable)dbContext).AplicarMigraciones();
                    ((IRepositorioInicializable)dbContext).Inicializar(env.ContentRootPath, demodb);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

        private static async Task InicializarAplicationAutoConfigurable( IWebHostEnvironment env,  IWebHost host, bool demodb)
        {
            List<Type> repositorios = LocalizadorEnsamblados.ObtieneContextosAutoConfigurables();
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            foreach (Type repo in repositorios)
            {
                var instancia = (IRepositorioInicializableAutoConfigurable)Activator.CreateInstance(repo);
                try
                {
                    await instancia.Inicializar(config, env.ApplicationName, demodb, loggerFactory).ConfigureAwait(false);
                                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }


        private static async Task InicializaTareasAutomaticas(IConfiguration configuracion, IWebHostEnvironment env, IWebHost host, bool demodb)
        {
            List<Type> tiposTareasAutomaticas = LocalizadorEnsamblados.ObtieneTiposTareasAutomaticas();
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
            ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
            string connectionString = configuracion.GetConnectionString("pika-gd");
            var ServicioTareas = host.Services.GetRequiredService<IServicioTareaAutomatica>();
            string dominioid = "principal";
            Log.Information("Procesando tareas automáticas");


            foreach (Type tipo in tiposTareasAutomaticas)
            {
                
                var instancia = (IProveedorTareasAutomaticas)Activator.CreateInstance(tipo);
                try
                {
                    var lista = instancia.ObtieneTareasAutomaticas();
                    foreach(var item in lista)
                    {
                        Log.Information($"Tarea {item.Nombre}:{tipo.Name}@{dominioid}");
                        if (!await ServicioTareas.Existe(t=>t.Id == item.Id && t.OrigenId == dominioid).ConfigureAwait(false))
                        {
                            await ServicioTareas.CrearAsync(new Modelo.Aplicacion.Tareas.TareaAutomatica() { 
                             CodigoError = null, Id = item.Id, Duracion = null, Ensamblado = tipo.FullName, Exito = null,
                             FechaHoraEjecucion = item.FechaHoraEjecucion, Intervalo = item.Intervalo, Nombre = item.Nombre, OrigenId = dominioid,
                             Periodo = item.Periodo, ProximaEjecucion =null, TipoOrigenId = item.TipoOrigenDefault, UltimaEjecucion = null})
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

    }
}
