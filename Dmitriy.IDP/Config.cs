// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace Dmitriy.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(), //to support profile scope/ work with user profile claims (i.e. Name.GivenName, see TestUsers).
                new IdentityResources.Email(), //add e-mail information to support email scope
                new IdentityResources.Address(), //add Address info to support address scope
                new IdentityResources.Phone(),
                new IdentityResource("roles","app roles", 
                    new List<string>{"role"}), //new roles resource to hold a list of role claims
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("helloworldapi","Hello World API"), 
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("helloworldapi", "Hello World API"){
                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Profile,
                    },
                    Scopes =
                    {
                        "helloworldapi"
                    }
                }
            };
        public static IEnumerable<Client> Clients =>
            new[]
            {
                //new Client //list of clients that would have access to IDP
                //{
                //    ClientName = "Hello World API Client",
                //    ClientId = "helloworldapi",
                //    AllowedGrantTypes = GrantTypes.Code,
                //    RequirePkce = true,
                //    RedirectUris =
                //    {
                //        "http://localhost:52267/signin-oidc"
                //    },
                //    AllowedScopes = //the scope the client can access
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                        
                //    },
                //    ClientSecrets =
                //    {
                //        new Secret("secret".Sha256())
                //    }
                //},
                new Client //list of clients that would have access
                {
                    //AccessTokenLifetime = 120,
                    //AllowOfflineAccess = true, //needed to refresh access token
                    //UpdateAccessTokenClaimsOnRefresh = true, 
                    ClientName = "Hello World Web Client",
                    ClientId = "helloworldwebclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RedirectUris =
                    {
                        "https://localhost:44304/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                        {
                            "https://localhost:44304/signout-callback-oidc"
                        },
                    AllowedScopes = //the scope the client can access
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "roles", //role-based authorization
                        "helloworldapi" //for authorizing the requests to the API
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }

            };

        
      
    }
}