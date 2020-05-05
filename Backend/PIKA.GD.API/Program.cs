using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RepositorioEntidades;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PIKA.GD.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .CreateLogger();


            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();
                var CurrentHost = host.Services.GetService<IWebHostEnvironment>();
                var config = host.Services.GetRequiredService<IConfiguration>();
                
                InicializarAplication(config, CurrentHost);

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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
             .ConfigureAppConfiguration(config =>
             {
                 config
                     // Used for local settings like connection strings.
                     .AddJsonFile("appsettings.local.json", optional: true);
             }).UseSerilog();



        private static void InicializarAplication(IConfiguration configuracion, IWebHostEnvironment env) {
            List<Type> contextos = LocalizadorEnsamblados.ObtieneContextosInicializables();
            string connectionString = configuracion.GetConnectionString("pika-gd");
            foreach (Type dbContextType in contextos)
            {
                Console.WriteLine($">>>>>{dbContextType.Name}@{connectionString}");
                var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(dbContextType);
                var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType);
                optionsBuilder.UseMySql(connectionString);
                var dbContext = (DbContext)Activator.CreateInstance(dbContextType, optionsBuilder.Options);
                try
                {

                    
                    ((IRepositorioInicializable)dbContext).AplicarMigraciones();
                    ((IRepositorioInicializable)dbContext).Inicializar(env.ContentRootPath);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

    }
}
