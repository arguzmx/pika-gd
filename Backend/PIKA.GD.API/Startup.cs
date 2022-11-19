using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Middlewares;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.Seguridad;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.Contenido;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using PIKA.GD.API.Servicios;
using Serilog.Events;
using PIKA.Servicio.Contacto;
using PIKA.Servicio.Usuarios;
using PIKA.GD.API.JsonConverters;
using PIKA.Servicio.Seguridad.Servicios;
using PIKA.Servicio.Seguridad.Interfaces;
using PIKA.Servicio.Reportes.Data;
using Microsoft.AspNetCore.HttpOverrides;
using PIKA.ServicioBusqueda.Contenido;
using Microsoft.IdentityModel.Logging;
using PIKA.GD.API.Servicios.TareasAutomaticas;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Reflection;
using System.IO;
using PIKA.GD.API.Servicios.Registro;

namespace PIKA.GD.API
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
         .Enrich.FromLogContext()
         .WriteTo.Console()
         .CreateLogger();
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p =>
                {
                    p.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            }).AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                jsonOptions.JsonSerializerOptions.Converters.Add(new IntNulableConverter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            LocalizadorEnsamblados.ObtieneControladoresACL();


            //registra los ensamblados con módulos administrables para  ACL 
            services.RegistraModulosAdministrados();

            //Registra servicios inyetables
            services.RegistraServiciosIntectables();

            //Servicios de cache de la aplicación basaodo en LazyCache
            services.AddLazyCache();

            // REgistra los serviicos para eventos basados en IEventBusService
            //services.RegistraServiciosParaEventos();


            // Configura el bus de eventos
            //services.ConfiguraBusEventos(this.Configuration);

            // ergistra la instancia del servicio de metadatos en base al tipo 
            services.RegistraServicioDeMetadatos(this.Configuration);


            //Registra el servicio e almacenamiento de contenido elestic search
            services.RegistraServicioDeContenido(this.Configuration);

            //Registra el servicio de bsuqeuda de contenido de elastic search
            services.RegistraServicioBusquedaContenido(this.Configuration);

            services.Configure<ConfiguracionServidor>(o => this.Configuration.GetSection("ConfiguracionServidor").Bind(o));
            services.AddSingleton(typeof(IServicioCache), typeof(CacheMemoria));
            services.AddSingleton(typeof(IAPICache<>), typeof(APICache<>));
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddTransient(typeof(ICompositorConsulta<>), typeof(QueryComposer<>));
            services.AddTransient<IServicioTokenSeguridad, ServicioTokenSeguridad>();
            services.AddTransient<ICacheSeguridad, CacheSeguridadMemoria>();
            services.AddTransient<IServicioPerfilUsuario, ServicioPerfilUsuario>();
            services.AddTransient(typeof(IProveedorMetadatos<>), typeof(ReflectionMetadataExtractor<>));
            services.AddTransient<IServicioBusquedaContenido, ServicioBusquedaContenido>();
            services.AddTransient<IRegistroAuditoria, ServicioEventoAuditoria>();
            services.AddTransient<IServicioInfoAplicacion, ServicioInfoAplicacionReflectivo>();
            services.AddScoped<AsyncACLActionFilter>();
            services.AddScoped<AsyncIdentityFilter>();

            services.AddTransient(typeof(IProveedorOpcionesContexto<>), typeof(ProveedorOpcionesContexto<>));

#if DEBUG
            //services.AddDbContext<DbContextAplicacion>(options =>
            //options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            //            services.AddDbContext<DbContextOrganizacion>(options =>
            //            options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            //            services.AddDbContext<DbContextContacto>(options => 
            //            options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextSeguridad>(options =>
            options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            //services.AddDbContext<DbContextContenido>(options =>
            //options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            //            services.AddDbContext<DbContextReportes>(options =>
            //options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            //services.AddDbContext<DBContextGestionDocumental>(options =>
            //    options.UseMySql(Configuration.GetConnectionString("pika-gd")));


            //services.AddDbContext<DbContextMetadatos>(options =>
            //options.UseMySql(Configuration.GetConnectionString("pika-gd")));

#endif

            services.AddSingleton<IRegistroPIKA, RegistroPIKA>();

            //registra los ensamblados validables
            services.RegistraValidables();
            services.AddControllers();


            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });


            // Servicios de Swaggr OPEN API
            services.AddOpenApiDocument( op=>
            {
                op.Title = "PIKA GD API";
            } );

            ConfiguracionRepoMetadatos elastic = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("RepositorioContenido").Bind(elastic);

            // Configura la autenticación con el servidor de identidad
            services.ConfiguraAutenticacionJWT(this.Configuration);

            services.AddHealthChecks()
                .AddUrlGroup(
                    new Uri(Configuration.GetValue<string>("ConfiguracionServidor:jwtauth").TrimEnd('/') + "/.well-known/openid-configuration"), 
                    name: "identityserver", 
                    failureStatus: 
                    HealthStatus.Unhealthy, 
                    tags: new string[] { "identityserver" })
                .AddElasticsearch(
                    name: "elasticsearch", 
                    elasticsearchUri: elastic.CadenaConexion(), 
                    failureStatus: HealthStatus.Degraded, 
                    tags: new string[] { "elasticsearch" } )
                .AddRabbitMQ(name: "rabbitmq", 
                    failureStatus: HealthStatus.Unhealthy, 
                    rabbitConnectionString: CadenaRabbitMQ(),
                    tags: new string[] { "rabbitmq" })
                .AddMySql(
                    connectionString: Configuration["ConnectionStrings:pika-gd"],
                    name: "mysql",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new string[] { "mysql" });


            // Registro de servicios vía Autofac, es necesario para Rabbit MQ
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());

        }


        private string CadenaRabbitMQ()
        {
            ConfiguracionRepoMetadatos rabbit = new ConfiguracionRepoMetadatos();
            rabbit.Tipo = "rabbitmq";
            Configuration.GetSection("EventBus").Bind(rabbit);
            return rabbit.CadenaConexion();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfiguracionServidor servidor = new ConfiguracionServidor();
            Configuration.GetSection("ConfiguracionServidor").Bind(servidor);

            if (string.IsNullOrEmpty(servidor.healthendpoint)) servidor.healthendpoint = "/health";

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<JWTAuthenticationMiddleware>();
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

#if DEBUG
            IdentityModelEventSource.ShowPII = true;
#endif


            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAll");


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(servidor.healthendpoint, new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();

            //app.ConfigureEventBus();
        }
    }
}
