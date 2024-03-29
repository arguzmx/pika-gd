﻿using Autofac;
using FluentValidation;
using FluentValidation.AspNetCore;
using LazyCache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infrastructure.EventBus;
using PIKA.Infrastructure.EventBus.Abstractions;
using PIKA.Infrastructure.EventBusRabbitMQ;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Contenido.Servicios;
using PIKA.Servicio.Metadatos.ElasticSearch;
using PIKA.ServicioBusqueda.Contenido;
using RabbitMQ.Client;
using RepositorioEntidades;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PIKA.GD.API
{
    public static class StartupExtensions
    {

        /// <summary>
        /// Registra los modulos decorados para validación fluent
        /// </summary>
        /// <param name="services"></param>
        public static void RegistraValidables(this IServiceCollection services)
        {
            List<string> ensambladosValidables = LocalizadorEnsamblados.LocalizaConTipo(LocalizadorEnsamblados.ObtieneRutaBin(), typeof(IValidator));
            List<Assembly> ensambladosValidacion = new List<Assembly>();


            Log.Logger.Information("Tipos validables registrados automáticamente");


            services.AddMvc(options =>
            {
                //options.Conventions.Add(new RouteTokenTransformerConvention(
                //                             new SlugifyParameterTransformer()));

                options.ModelBinderProviders.Insert(0, new GenericDataPageModelBinderProvider());

            }).AddFluentValidation(opt =>
            {
                opt.LocalizationEnabled = false;
                opt.RunDefaultMvcValidationAfterFluentValidationExecutes = false;

                foreach (string item in ensambladosValidables)
                {
                    Log.Logger.Information("{Tipo}", item);
                    opt.RegisterValidatorsFromAssembly(Assembly.LoadFrom(item));
                }
                //opt.RegisterValidatorsFromAssemblies(ensambladosValidacion);
            });


        }


        /// <summary>
        /// REgistra los módulos de la aplicación utilizados para la revisón de ACL
        /// </summary>
        /// <param name="services"></param>
        public static void RegistraModulosAdministrados(this IServiceCollection services)
        {
            List<TipoAdministradorModulo> ModulosAdministrados = LocalizadorEnsamblados.ObtieneTiposAdministrados();
            Log.Logger.Information("Módulos ACL registrados automáticamente");
            foreach (var t in ModulosAdministrados)
            {
                foreach (var x in t.TiposAdministrados)
                {
                    Log.Logger.Information("{moduloid} para el tipo {tipo}", t.ModuloId, x.Name);
                }

            }

            ServicioAplicacion.SetModulosAdministrador(ModulosAdministrados);
        }



        /// <summary>
        /// Añade a DI los servicios inyectables
        /// </summary>
        /// <param name="services"></param>
        public static void RegistraServiciosIntectables(this IServiceCollection services)
        {
            Log.Logger.Information("Servicios inyectables registrados automáticamente");
            List<ServicioInyectable> inyectables = LocalizadorEnsamblados.ObtieneServiciosInyectables();

            foreach (var item in inyectables)
            {
                string servicio = item.NombreServicio.Split('.')[item.NombreServicio.Split('.').Length - 1];
                string implementacion = item.NombreImplementacion.Split('.')[item.NombreImplementacion.Split('.').Length - 1];
                Log.Logger.Information("{servicio} para el servicio {tipo}", implementacion, servicio);

                var ensamblado = Assembly.LoadFrom(item.RutaEnsamblado);
                services.AddTransient(
                    ensamblado.GetType(item.NombreServicio),
                    ensamblado.GetType(item.NombreImplementacion));
            }

        }


        public static void RegistraServicioDeContenido(this IServiceCollection services, IConfiguration Configuration)
        {
            ConfiguracionRepoMetadatos repometadtosconf = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Contenido").Bind(repometadtosconf);
            if (repometadtosconf.Tipo == ConfiguracionRepoMetadatos.ELASTICSEARCH)
            {
                Log.Logger.Information("Estableciendo el repositorio de contenido a {x}", "Elasticsearch");
                services.AddTransient<IRepoContenidoElasticSearch>(provider =>
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    return new RepoContenidoElasticSearch(Configuration, loggerFactory);
                });
            }
        }


        public static void RegistraServicioBusquedaContenido(this IServiceCollection services, IConfiguration Configuration)
        {
            ConfiguracionRepoMetadatos repometadtosconf = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("BusquedaContenido").Bind(repometadtosconf);
            if (repometadtosconf.Tipo == ConfiguracionRepoMetadatos.ELASTICSEARCH)
            {
                Log.Logger.Information("Estableciendo el repositorio de búsqueda de contenido a {x}", "Elasticsearch");
                services.AddTransient<IServicioBusquedaContenido>(provider =>
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    var IpOpciones = provider.GetRequiredService<IProveedorOpcionesContexto<DbContextBusquedaContenido>>();
                    var ICache = provider.GetRequiredService<IAppCache>();
                    var IRepositorioMetadatos = provider.GetRequiredService<IRepositorioMetadatos>();
                    var IServicioPlantilla = provider.GetRequiredService<Servicio.Metadatos.Interfaces.IServicioPlantilla>();
                    var IRepoContenidoElasticSearch = provider.GetRequiredService<IRepoContenidoElasticSearch>();
                    return new ServicioBusquedaContenido(IServicioPlantilla, IRepositorioMetadatos, IRepoContenidoElasticSearch, ICache, IpOpciones, Configuration, loggerFactory);
                });
            }
        }

        public static void RegistraServicioDeMetadatos(this IServiceCollection services, IConfiguration Configuration)
        {
            ConfiguracionRepoMetadatos repometadtosconf = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Metadatos").Bind(repometadtosconf);
            if (repometadtosconf.Tipo == ConfiguracionRepoMetadatos.ELASTICSEARCH)
            {

                Log.Logger.Information("Estableciendo el repositorio de metadatos a {x}", "Elasticsearch");
                services.AddTransient<IRepositorioMetadatos>(provider =>
                {
                    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                    var appCache = provider.GetRequiredService<IAppCache>();
                    return new RepoMetadatosElasticSearch(appCache, Configuration, loggerFactory);
                });

            }
        }

        /// <summary>
        /// Resgitra los servicios que implementan IEventBusService para la publicación de eventos
        /// </summary>
        /// <param name="services"></param>
        public static void RegistraServiciosParaEventos(this IServiceCollection services)
        {
            Log.Logger.Information("Servicios para bus de eventos registrados automáticamente");
            List<ServicioInyectable> inyectables = LocalizadorEnsamblados.ObtieneServiciosBusEventos();

            foreach (var item in inyectables)
            {
                string servicio = item.NombreServicio.Split('.')[item.NombreServicio.Split('.').Length - 1];
                string implementacion = item.NombreImplementacion.Split('.')[item.NombreImplementacion.Split('.').Length - 1];
                Log.Logger.Information("{servicio} de bus para el servicio {tipo}", implementacion, servicio);

                var ensamblado = Assembly.LoadFrom(item.RutaEnsamblado);
                services.AddTransient(
                    ensamblado.GetType(item.NombreServicio),
                    ensamblado.GetType(item.NombreImplementacion));
            }

        }

        /// <summary>
        /// Configura el bus de eventos
        /// </summary>
        /// <param name="services"></param>
        public static void ConfiguraBusEventos(this IServiceCollection services, IConfiguration Configuration)
        {
            ConfiguracionEventBus configuracionEventBus = new ConfiguracionEventBus();
            Configuration.GetSection("EventBus").Bind(configuracionEventBus);

            if (configuracionEventBus.Tipo == ConfiguracionEventBus.RABBITMQ)
            {
                Log.Logger.Information("Adiconando Rabbit MQ como bus de eventos");
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = configuracionEventBus.DatosConexion.Url,
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(configuracionEventBus.DatosConexion.Usuario))
                    {
                        factory.UserName = configuracionEventBus.DatosConexion.Usuario;
                    }

                    if (!string.IsNullOrEmpty(configuracionEventBus.DatosConexion.Contrasena))
                    {
                        factory.Password = configuracionEventBus.DatosConexion.Contrasena;
                    }


                    int retryCount = configuracionEventBus.DatosConexion.Reintentos;

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
                RegisterEventBus(services, Configuration);
            }
        }

        private static void RegisterEventBus(IServiceCollection services, IConfiguration Configuration)
        {

            ConfiguracionEventBus configuracionEventBus = new ConfiguracionEventBus();
            Configuration.GetSection("EventBus").Bind(configuracionEventBus);

            if (configuracionEventBus.Tipo == ConfiguracionEventBus.RABBITMQ)
            {
                Log.Logger.Information("Registrando la instancia de Rabbit MQ como bus de eventos");
                services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager,
                        configuracionEventBus.DatosConexion.Id,
                        configuracionEventBus.DatosConexion.Reintentos);
                });
            }

            Log.Logger.Information("Registrando InMemoryEventBusSubscriptionsManager para el bus");
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            List<EnsambladosEvento> servicios = LocalizadorEnsamblados.ObtieneManejadoresEventosBus();

            foreach (var item in servicios)
            {

                Log.Logger.Information("Añadiendo manejador {a} para  {b}",
                    item.NombreHandler.GetNombreCortoEnsamblado(),
                    item.NombreEvento.GetNombreCortoEnsamblado());

                var ensamblado = Assembly.LoadFrom(item.RutaEnsambladoHandler);
                services.AddTransient(ensamblado.GetType(item.NombreHandler));

            }


        }


        /// <summary>
        /// Registra las suscripciones a ventos para el bus
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureEventBus(this IApplicationBuilder app)
        {
            Log.Logger.Information("Registrando suscripciones al bus de eventos");
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            List<EnsambladosEvento> servicios = LocalizadorEnsamblados.ObtieneManejadoresEventosBus();

            foreach (var item in servicios)
            {

                Log.Logger.Information("Añadiendo suscripción {a} para  {b}",
                    item.NombreHandler.GetNombreCortoEnsamblado(),
                    item.NombreEvento.GetNombreCortoEnsamblado());

                var ensamblado = Assembly.LoadFrom(item.RutaEnsambladoHandler);

                eventBus.Subscribe(ensamblado.GetType(item.NombreEvento), ensamblado.GetType(item.NombreHandler));

            }



        }


        /// <summary>
        /// Configura la autenticación de JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfiguraAutenticacionJWT(this IServiceCollection services, IConfiguration configuration)
        {

            Log.Logger.Information("Configurando autenticación JWT");
            ConfiguracionServidor configuracionServidor = new ConfiguracionServidor();
            configuration.GetSection("ConfiguracionServidor").Bind(configuracionServidor);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false
    };

    options.SaveToken = true;
    // base-address of your identityserver
    options.Authority = configuracionServidor.jwtauth;
    // name of the API resource
    options.Audience = configuracionServidor.jwtaud;
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Add the access_token as a claim, as we may actually need it
            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken != null)
            {
                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    identity.AddClaim(new Claim("access_token", accessToken.RawData));
                    identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, accessToken.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).First().Value));
                }
            }

            return Task.CompletedTask;
        }
    };
    options.RequireHttpsMetadata = false;
});

        }





        public static string GetNombreCortoEnsamblado(this string nombre)
        {
            return nombre.Split('.')[nombre.Split('.').Length - 1];
        }

    }
}
