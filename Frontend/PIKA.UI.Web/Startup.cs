// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using PIKA.UI.Web.Data;
using PIKA.UI.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PIKA.Servicio.Organizacion;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Reflection;
using System;
using FluentValidation.AspNetCore;
using FluentValidation;
using PIKA.Modelo.Organizacion;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using PIKA.Infraestructura.Comun;
using ExtCore.WebApplication.Extensions;
using RepositorioEntidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PIKA.UI.Web
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        private string extensionsPath; 

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
            this.extensionsPath = environment.ContentRootPath + configuration["Extensions:Path"];
        }

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

        public  void ConfigureServices(IServiceCollection services)
        {
            ConfiguracionServidor configuracionServidor = new ConfiguracionServidor();
            this.Configuration.GetSection("ConfiguracionServidor").Bind(configuracionServidor);

            // List<string> ensambladosValidables = LozalizadorEnsamblados.LocalizaConTipo(LozalizadorEnsamblados.ObtieneRutaBin(), typeof(IValidator));
            // List<Assembly> ensambladosValidacion = new List<Assembly>();
            // List<ServicioInyectable> inyectables = LozalizadorEnsamblados.ObtieneServiciosInyectables();

            // foreach (string item in ensambladosValidables)
            // {
            //     ensambladosValidacion.Add(Assembly.LoadFrom(item));
            // }


            // foreach (var item in inyectables)
            // {
            //     var ensamblado = Assembly.LoadFrom(item.RutaEnsamblado);
            //     services.AddTransient(
            //         ensamblado.GetType(item.NombreServicio), 
            //         ensamblado.GetType(item.NombreImplementacion));
            //}



            services.Configure<ConfiguracionServidor>(o=>this.Configuration.GetSection("ConfiguracionServer").Bind(o));
            services.AddSingleton(typeof(IServicioCache), typeof(CacheMemoria));
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddTransient(typeof ( ICompositorConsulta<>), typeof( QueryComposer<>));


        //services.AddExtCore(this.extensionsPath);


        services
        .AddMvc()
            .AddViewLocalization(
            LanguageViewLocationExpanderFormat.Suffix,
            opts => { 
                opts.ResourcesPath = "Resources"; 
            })
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                {
                    var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
                    return factory.Create(type.GetTypeInfo().Name, assemblyName.Name);
                };
            });


           services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[]
                {
                new CultureInfo("es-MX"),
                new CultureInfo("en-US")
            };
                options.DefaultRequestCulture = new RequestCulture("es-MX");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });


            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddDbContext<DbContextOrganizacion>(options =>
     options.UseMySql(Configuration.GetConnectionString("pika-gd")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

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

            //.AddGoogle(options =>
            //{
            //    // register your IdentityServer with Google at https://console.developers.google.com
            //    // enable the Google+ API
            //    // set the redirect URI to http://localhost:5000/signin-google
            //    options.ClientId = "copy client ID from Google here";
            //    options.ClientSecret = "copy client secret from Google here";
            //});
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            //app.UseExtCore();

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseRequestLocalization(
                app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller}/{action}",
                    defaults: new {controller = "Home" ,  action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "api",
                    pattern: "{controller}/{id?}");
            });


          
        }
    }
}