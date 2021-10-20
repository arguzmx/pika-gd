// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using PIKA.Identity.Server.Data;
using PIKA.Identity.Server.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.Configuration;
using PIKA.Modelo.Seguridad;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using IdentityServer4.Hosting;
using PIKA.Identity.Server.Services;
using PIKA.Servicio.Usuarios;
using RepositorioEntidades;

namespace PIKA.Identity.Server
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
          services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddCors();

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

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string dbconnstr = Configuration.GetConnectionString("pika-gd");

            services.AddControllersWithViews();

            services.AddTransient<IServicioPerfilUsuario, ServicioPerfilUsuario>();
            services.AddTransient(typeof(IProveedorOpcionesContexto<>), typeof(ProveedorOpcionesContexto<>));

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseMySql(dbconnstr));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            services.AddHealthChecks();

            var builder = services.AddIdentityServer(options =>
                {
                    options.PublicOrigin = Configuration["PublicOrigin"];
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction = new UserInteractionOptions()
                    {
                        LogoutUrl = "/account/logout",
                        LoginUrl = "/account/login",
                        LoginReturnUrlParameter = "returnUrl"
                    };
                })
                 .AddConfigurationStore(options =>
                 {
                     options.ConfigureDbContext = b => b.UseMySql(dbconnstr,
                         sql => sql.MigrationsAssembly(migrationsAssembly));
                 })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b => b.UseMySql(dbconnstr,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<PikaProfileService>();


            builder.Services.ConfigureExternalCookie(options => {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Unspecified; 
            });

            builder.Services.ConfigureApplicationCookie(options => {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Unspecified; 
            });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

 
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            

            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseRequestLocalization(
                app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            app.UseHealthChecks("/health");

            app.UseRouting();
            app.UseIdentityServer();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "areaRoute",
                //    pattern: "{area:exists}/{controller}/{action}",
                //    defaults: new {controller = "Home" ,  action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                //endpoints.MapControllerRoute(
                //    name: "api",
                //    pattern: "{controller}/{id?}");
            });


          
        }
    }
}