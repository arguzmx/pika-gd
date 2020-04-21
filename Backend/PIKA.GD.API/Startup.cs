using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
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

namespace PIKA.GD.API
{
    public class Startup
    {
        public ILifetimeScope AutofacContainer { get; private set; }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public ILogger<Startup> Logger { get; private set; }


        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //// Add any Autofac modules or registrations.
            //// This is called AFTER ConfigureServices so things you
            //// register here OVERRIDE things registered in ConfigureServices.
            ////
            //// You must have the call to AddAutofac in the Program.Main
            //// method or this won't be called.
            //builder.RegisterModule(new AutofacModule());
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            }).AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            ConfiguracionServidor configuracionServidor = new ConfiguracionServidor();
            this.Configuration.GetSection("ConfiguracionServidor").Bind(configuracionServidor);


            List<string> ensambladosValidables = LocalizadorEnsamblados.LocalizaConTipo(LocalizadorEnsamblados.ObtieneRutaBin(), typeof(IValidator));
            List<Assembly> ensambladosValidacion = new List<Assembly>();
            List<ServicioInyectable> inyectables = LocalizadorEnsamblados.ObtieneServiciosInyectables();
            List<TipoAdministradorModulo> ModulosAdministrados = LocalizadorEnsamblados.ObtieneTiposAdministrados();
            

#if DEBUG
            foreach (var t in ModulosAdministrados)
            {
                foreach (var x in t.TiposAdministrados)
                {
                    Console.WriteLine($"{t.ModuloId} === {x.Name}");
                }

            }


            foreach (var t in ensambladosValidables)
            {
                
                    Console.WriteLine($"{t} === V");
                

            }
#endif



          foreach (string item in ensambladosValidables)
            {
                ensambladosValidacion.Add(Assembly.LoadFrom(item));
            }


            foreach (var item in inyectables)
            {
                var ensamblado = Assembly.LoadFrom(item.RutaEnsamblado);
                services.AddTransient(
                    ensamblado.GetType(item.NombreServicio),
                    ensamblado.GetType(item.NombreImplementacion));
            }

            ServicioAplicacion.ModulosAdministrados = ModulosAdministrados;
            services.Configure<ConfiguracionServidor>(o => this.Configuration.GetSection("ConfiguracionServidor").Bind(o));
            services.AddSingleton(typeof(IServicioCache), typeof(CacheMemoria));
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddTransient(typeof(ICompositorConsulta<>), typeof(QueryComposer<>));
            services.AddTransient<IServicioTokenSeguridad, ServicioTokenSeguridad>();
            services.AddTransient<ICacheSeguridad, CacheSeguridadMemoria>();
            services.AddTransient(typeof(IProveedorMetadatos<>), typeof(ReflectionMetadataExtractor<>));

            services.AddTransient<ILocalizadorFiltroACL, LocalizadorFiltroACLReflectivo>();

            services.AddScoped<AsyncACLActionFilter>();

            services.AddTransient(typeof(IProveedorOpcionesContexto<>),typeof(ProveedorOpcionesContexto<>));

            services.AddDbContext<DBContextGestionDocumental>(options =>
                    options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextSeguridad>(options =>
                   options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextMetadatos>(options =>
                   options.UseMySql(Configuration.GetConnectionString("pika-gd")));


            services.AddDbContext<DbContextOrganizacion>(options =>
       options.UseMySql(Configuration.GetConnectionString("pika-gd")));


            services.AddControllers();

            services.AddMvc(options =>
            {
                //options.Conventions.Add(new RouteTokenTransformerConvention(
                //                             new SlugifyParameterTransformer()));

                options.ModelBinderProviders.Insert(0, new DatatablesModelBinderProvider());
                options.ModelBinderProviders.Insert(0, new GenericDataPageModelBinderProvider());

            }).AddFluentValidation(opt =>
            {
                opt.LocalizationEnabled = false;
                opt.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                opt.RegisterValidatorsFromAssemblies(ensambladosValidacion);
            });



            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddOpenApiDocument();

            services.AddAuthentication();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
         .AddJwtBearer(options =>
         {
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
                         }
                     }

                     return Task.CompletedTask;
                 }
             };
             options.RequireHttpsMetadata = false;
         });



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

        }
    }
}
