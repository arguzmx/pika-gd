// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace PIKA.Identity.Server
{
    public static class Config
    {

        public const string spaClientUrl = "http://localhost:4200";

        public const string PIKAGDNAME = "pika-gd";
        //public const string PIKAGDCLIENTNAME = "api-pika-gd";

        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource(PIKAGDNAME, "PIKA Gestión documental")
            };



        public static IEnumerable<Client> Clients =>
            new Client[]
            {
new Client {
    ClientId = "angular_spa",
    ClientName = "Angular 4 Client",
    AllowedGrantTypes = GrantTypes.Code,
    RequirePkce = true,
    RequireClientSecret = false,
    AllowedScopes = new List<string> {"openid", "profile", "pika-gd"},
    RedirectUris = new List<string> {"http://localhost:4200/index.html", "http://localhost:4200/silent-refresh.html"},
    PostLogoutRedirectUris = new List<string> {"http://localhost:4200/"},
    AllowedCorsOrigins = new List<string> {"http://localhost:4200"},
    AllowAccessTokensViaBrowser = true
},

                //Cliente para autenticación de POSTMAN
                        new Client {
                        ClientId = "api-pika-gd-pass",
                        ClientName = "Cliente REST API para PIKA Gestión Documental",
                        AllowOfflineAccess=true,
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                         ClientSecrets =
                            {
                                new Secret("secret".Sha256())
                            },
                            AllowedScopes = {
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile,
                                IdentityServerConstants.StandardScopes.Email,
                                IdentityServerConstants.StandardScopes.Address,
                                PIKAGDNAME
                            }
                        },

                // Cliente aplicacion angular
                new Client
{
                    ClientId = "api-pika-gd-angular",
                    ClientName = "Cliente PIKA Gestión Documental Web",

    AccessTokenType = AccessTokenType.Jwt,
    AccessTokenLifetime = 3600,// 330 seconds, default 60 minutes
    IdentityTokenLifetime = 3600,

    RequireClientSecret = false,
    AllowedGrantTypes = GrantTypes.Implicit,
    RequirePkce = true,
    RequireConsent =false,
    AllowAccessTokensViaBrowser = true,
    RedirectUris = new List<string>
    {
        "http://localhost/silent-refresh.html",
        "http://localhost/index.html",
        "http://localhost",
    },
    PostLogoutRedirectUris = new List<string>
    {
        $"{spaClientUrl}",
        "http://localhost/silent-refresh.html",
        "http://localhost/index.html",
        "http://localhost",
    },
    AllowedCorsOrigins = new List<string>
    {
        $"http://localhost",

    },
    AllowedScopes = new List<string>
    {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        IdentityServerConstants.StandardScopes.OfflineAccess,
        PIKAGDNAME
    }, AlwaysIncludeUserClaimsInIdToken = true, AlwaysSendClientClaims = true
},



            };

        
    }
}