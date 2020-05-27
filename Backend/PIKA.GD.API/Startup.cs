using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Organizacion;
using RepositorioEntidades;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.Seguridad;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Seguridad.Data;
using PIKA.Servicio.AplicacionPlugin;
using PIKA.Servicio.Contenido;
using PIKA.Servicio.Metadatos.ElasticSearch;
using PIKA.Infrastructure.EventBusRabbitMQ;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc.Formatters;
using PIKA.Infrastructure.EventBus;
using PIKA.Infrastructure.EventBus.Abstractions;
using Autofac;
using PIKA.Servicio.Metadatos.EventosBus;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Autofac.Core;

namespace PIKA.GD.API
{
    public class Startup
    {
        
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

       
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
            Log.Logger = new LoggerConfiguration()
         .Enrich.FromLogContext()
         .WriteTo.Console()
         .CreateLogger();
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            
            services.AddCors();
            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            }).AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


            //registra los ensamblados validables
            services.RegistraValidables();

            //registra los ensamblados con módulos administrables para  ACL 
            services.RegistraMódulosAdministrados();

            //Registra servicios inyetables
            services.RegistraServiciosIntectables();

            //Servicios de cache de la aplicación basaodo en LazyCache
            services.AddLazyCache();

            // REgistra los serviicos para eventos basados en IEventBusService
            services.RegistraServiciosParaEventos();

            // ergistra la instancia del servicio de metadatos en base al tipo 
            services.RegistraServicioDeMetadatos(this.Configuration);

            // Configura el bus de eventos
            services.ConfiguraBusEventos(this.Configuration);


            services.Configure<ConfiguracionServidor>(o => this.Configuration.GetSection("ConfiguracionServidor").Bind(o));
            services.AddSingleton(typeof(IServicioCache), typeof(CacheMemoria));
            services.AddSingleton(typeof(IAPICache<>), typeof(APICache<>));
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddTransient(typeof(ICompositorConsulta<>), typeof(QueryComposer<>));
            services.AddTransient<IServicioTokenSeguridad, ServicioTokenSeguridad>();
            services.AddTransient<ICacheSeguridad, CacheSeguridadMemoria>();
            services.AddTransient(typeof(IProveedorMetadatos<>), typeof(ReflectionMetadataExtractor<>));

            services.AddTransient<ILocalizadorFiltroACL, LocalizadorFiltroACLReflectivo>();
            services.AddScoped<AsyncACLActionFilter>();

            services.AddTransient(typeof(IProveedorOpcionesContexto<>),typeof(ProveedorOpcionesContexto<>));

     
            services.AddDbContext<DbContextSeguridad>(options =>
                 options.UseMySql(Configuration.GetConnectionString("pika-gd")));

    
            services.AddDbContext<DbContextAplicacionPlugin>(options =>
                  options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextOrganizacion>(options =>
                    options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextContenido>(options =>
               options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DBContextGestionDocumental>(options =>
               options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextMetadatos>(options =>
           options.UseMySql(Configuration.GetConnectionString("pika-gd")));


            services.AddControllers();


            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });


            // Servicios de Swaggr OPEN API
            services.AddOpenApiDocument();


            // Configura la autenticación con el servidor de identidad
            services.AddAuthentication();
            services.ConfiguraAutenticaciónJWT(this.Configuration);
                        

            // Registro de servicios vía Autofac, es necesario para Rabbit MQ
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());

        }

     
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();

            //app.UseHealthChecks("/health");


            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.ConfigureEventBus();
        }
    }
}
