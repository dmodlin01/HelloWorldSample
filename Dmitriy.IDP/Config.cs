// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Dmitriy.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile() //to support profile scope/ work with user profile claims (i.e. Name.GivenName, see TestUsers).
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            { };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client //list of clients that would have access
                {
                    ClientName = "HelloWorldWebAPI",
                    ClientId = "HelloWorldWebAPIClient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RedirectUris =
                    {
                        "http://localhost:52267/signin-oidc"
                    },
                    AllowedScopes = //the scope the client can access
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new Client //list of clients that would have access
                {
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
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }

            };
    }
}