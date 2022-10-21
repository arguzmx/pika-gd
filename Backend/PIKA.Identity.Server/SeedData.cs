// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using PIKA.Identity.Server.Data;
using PIKA.Identity.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Reflection;
using PIKA.Modelo.Seguridad;

namespace PIKA.Identity.Server
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();

            var migrationsAssembly = typeof(SeedData).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer(options =>
            {
            })
                 .AddConfigurationStore(options =>
                 {
                     options.ConfigureDbContext = b => b.UseMySql(connectionString,
                         sql => sql.MigrationsAssembly(migrationsAssembly));
                 })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b => b.UseMySql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    });

            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(connectionString));



            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var contextGrants = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
                    contextGrants.Database.Migrate();

                    var contextConfiguration = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                    contextConfiguration.Database.Migrate();

                    
                    foreach (var client in Config.Clients)
                    {
                        
                        if (!contextConfiguration.Clients.Any(x=>x.ClientId == client.ClientId))
                        {
                            contextConfiguration.Clients.Add(client.ToEntity());
                            contextConfiguration.SaveChanges();
                        }
                    }

                    var cl = contextConfiguration.Clients.Where(x=>x.ClientId == "api-pika-gd-angular").SingleOrDefault();
                    if (cl != null)
                    {
                        string update = $"update ClientGrantTypes set GrantType='authorization_code' where Id={cl.Id}";
                        contextConfiguration.Database.ExecuteSqlRaw(update, new object[] { });  

                        update = "UPDATE ClientRedirectUris SET RedirectUri = CONCAT(RedirectUri, '/') where ClientId=4 AND RedirectUri NOT LIKE '%html' AND RedirectUri NOT LIKE '%/'";
                        contextConfiguration.Database.ExecuteSqlRaw(update, new object[] { });

                        update = "UPDATE ClientPostLogoutRedirectUris SET PostLogoutRedirectUri = CONCAT(PostLogoutRedirectUri, '/') where ClientId=4 AND PostLogoutRedirectUri NOT LIKE '%html' AND PostLogoutRedirectUri NOT LIKE '%/'";
                        contextConfiguration.Database.ExecuteSqlRaw(update, new object[] { });


                        update = $"update Clients set AllowOfflineAccess=1 where Id={cl.Id}";
                        contextConfiguration.Database.ExecuteSqlRaw(update, new object[] { });
                    }

                    if (!contextConfiguration.IdentityResources.Any())
                    {
                        foreach (var resource in Config.Ids)
                        {
                            contextConfiguration.IdentityResources.Add(resource.ToEntity());
                        }
                        contextConfiguration.SaveChanges();
                    }

                    if (!contextConfiguration.ApiResources.Any())
                    {
                        foreach (var resource in Config.Apis)
                        {
                            contextConfiguration.ApiResources.Add(resource.ToEntity());
                        }
                        contextConfiguration.SaveChanges();
                    }


                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var alice = userMgr.FindByNameAsync("administrador").Result;
                    if (alice == null)
                    {
                        alice = new ApplicationUser
                        {
                            UserName = "administrador", GlobalAdmin = true, Eliminada = false,
                            Id = "admin", Inactiva = false
                        };
                        var result = userMgr.CreateAsync(alice, "Pa$$w0rd").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Administrador"),
                        new Claim(JwtClaimTypes.GivenName, "Sistema"),
                        new Claim(JwtClaimTypes.FamilyName, ""),
                        new Claim(JwtClaimTypes.Email, "admin@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, ""),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': '', 'locality': '', 'postal_code': , 'country': '' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("Admin creado");
                    }
                    else
                    {
                        Log.Debug("Admin existe");
                    }

                }
            }
        }
    }
}
