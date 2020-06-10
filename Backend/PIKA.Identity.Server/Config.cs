// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace PIKA.Identity.Server
{
    public static class Config
    {

        public const string spaClientUrl = "http://localhost";

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


        //        public static IEnumerable<Client> Clients =>
        //            new Client[]
        //            {
        //                new Client {
        //                ClientId = "api-pika-gd-pass",
        //                ClientName = "Cliente REST API para PIKA Gestión Documental",
        //                AllowOfflineAccess=true,
        //                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
        //                 ClientSecrets =
        //                    {
        //                        new Secret("secret".Sha256())
        //                    },
        //                    AllowedScopes = {
        //                        IdentityServerConstants.StandardScopes.OpenId,
        //                        IdentityServerConstants.StandardScopes.Profile,
        //                        IdentityServerConstants.StandardScopes.Email,
        //                        IdentityServerConstants.StandardScopes.Address,
        //                        PIKAGDNAME
        //                    }
        //                },

        //                // client credentials flow client
        //                new Client
        //                {
        //                    ClientId = "client",
        //                    ClientName = "Client Credentials Client",

        //                    AllowedGrantTypes = GrantTypes.ClientCredentials,
        //                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

        //                    AllowedScopes = { PIKAGDNAME }
        //                },

        //                // MVC client using code flow + pkce
        //                new Client
        //                {
        //                    ClientId = "mvc",
        //                    ClientName = "MVC Client",

        //                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
        //                    RequirePkce = true,
        //                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

        //                    RedirectUris = { "http://localhost:5003/signin-oidc" },
        //                    FrontChannelLogoutUri = "http://localhost:5003/signout-oidc",
        //                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },

        //                    AllowOfflineAccess = true,
        //                    AllowedScopes = { "openid", "profile", PIKAGDNAME }
        //                },

        //                // SPA client using code flow + pkce
        //                new Client
        //                {
        //                    ClientId = "spa",
        //                    ClientName = "SPA Client",
        //                    ClientUri = "http://identityserver.io",

        //                    AllowedGrantTypes = GrantTypes.Code,
        //                    RequirePkce = true,
        //                    RequireClientSecret = false,

        //                    RedirectUris =
        //                    {
        //                        "http://localhost:5002/index.html",
        //                        "http://localhost:5002/callback.html",
        //                        "http://localhost:5002/silent.html",
        //                        "http://localhost:5002/popup.html",
        //                    },

        //                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
        //                    AllowedCorsOrigins = { "http://localhost:5002" },

        //                    AllowedScopes = { "openid", "profile", PIKAGDNAME }
        //                },

        //                       // SPA client using code flow + pkce
        //                new Client
        //                {
        //                    ClientId = "api-pika-gd",
        //                    ClientName = "Cliente PIKA Gestión Documental Web",
        //                    ClientUri = "http://localhost",
        //                    RequireConsent = false,
        //                    AllowedGrantTypes = GrantTypes.Code,
        //                    RequirePkce = true,
        //                    RequireClientSecret = false,

        //                    RedirectUris =
        //                    {
        //                        "http://localhost/index.html",
        //                        "http://localhost/callback.html",
        //                        "http://localhost/silent.html",
        //                        "http://localhost/popup.html",
        //                    },

        //                    PostLogoutRedirectUris = { "http://localhost/index.html" },
        //                    AllowedCorsOrigins = { "http://localhost" },

        //                    AllowedScopes = { "openid", "profile", PIKAGDNAME }
        //                },
        //                 // SPA client using code flow + pkce
        //                new Client
        //                {
        //                    ClientId = "api-pika-gd-angular-old",
        //                    ClientName = "Cliente PIKA Gestión Documental Web",
        //                    ClientUri = "http://localhost",
        //                    RequireConsent = false,
        //                    AllowedGrantTypes = GrantTypes.Code,
        //                    RequirePkce = true,
        //                    RequireClientSecret = false,
        //                    AllowOfflineAccess = true,
        //                    RedirectUris =
        //                    {
        //                        "http://localhost/index.html",
        //                        "http://localhost/callback.html",
        //                        "http://localhost/silent.html",
        //                        "http://localhost/popup.html",
        //                        "http://localhost/signout-callback.html",
        //                        "http://localhost/renew-callback.html"
        //                    },

        //                    PostLogoutRedirectUris = { "http://localhost/" },
        //                    AllowedCorsOrigins = { "http://localhost" },

        //                    AllowedScopes = { "openid",  "profile", PIKAGDNAME }
        //                },
        //                new Client
        //{
        //                    ClientId = "api-pika-gd-angular",
        //                    ClientName = "Cliente PIKA Gestión Documental Web",

        //    AccessTokenType = AccessTokenType.Jwt,
        //    AccessTokenLifetime = 330,// 330 seconds, default 60 minutes
        //    IdentityTokenLifetime = 30,

        //    RequireClientSecret = false,
        //    AllowedGrantTypes = GrantTypes.Code,
        //    RequirePkce = true,

        //    AllowAccessTokensViaBrowser = true,
        //    RedirectUris = new List<string>
        //    {
        //        $"{spaClientUrl}/callback",
        //        $"{spaClientUrl}/silent-renew.html",
        //        "https://localhost:4200",
        //        "https://localhost:4200/silent-renew.html"
        //    },
        //    PostLogoutRedirectUris = new List<string>
        //    {
        //        $"{spaClientUrl}/unauthorized",
        //        $"{spaClientUrl}",
        //        "https://localhost:4200/unauthorized",
        //        "https://localhost:4200"
        //    },
        //    AllowedCorsOrigins = new List<string>
        //    {
        //        $"{spaClientUrl}",
        //        "https://localhost:4200"
        //    },
        //    AllowedScopes = new List<string>
        //    {
        //        IdentityServerConstants.StandardScopes.OpenId,
        //        IdentityServerConstants.StandardScopes.Profile,
        //        PIKAGDNAME
        //    }
        //},
        //            };



        public static IEnumerable<Client> Clients =>
            new Client[]
            {

                new Client
{
                    ClientId = "api-pika-gd-angular",
                    ClientName = "Cliente PIKA Gestión Documental Web",

    AccessTokenType = AccessTokenType.Jwt,
    AccessTokenLifetime = 330,// 330 seconds, default 60 minutes
    IdentityTokenLifetime = 30,

    RequireClientSecret = false,
    AllowedGrantTypes = GrantTypes.Code,
    RequirePkce = false,
     
    AllowAccessTokensViaBrowser = true,
    RedirectUris = new List<string>
    {
        $"{spaClientUrl}",
        $"{spaClientUrl}/acceso/callback",
        $"{spaClientUrl}/acceso/login",


    },
    PostLogoutRedirectUris = new List<string>
    {
        $"{spaClientUrl}",
        $"{spaClientUrl}/acceso/callback",
        $"{spaClientUrl}/acceso/login",
    },
    AllowedCorsOrigins = new List<string>
    {
        $"{spaClientUrl}",

    },
    AllowedScopes = new List<string>
    {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        PIKAGDNAME
    }
},
            };


    }
}