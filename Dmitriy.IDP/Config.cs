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
                new IdentityResource("userlevel","user level",
                    new List<string>{"userlevel" })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("helloworldapi","Hello World API",
                    new List<string>{"role","userlevel"}
                    )
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
                new Client //list of clients that would have access
                {
                    //IdentityTokenLifetime = 300, //(in seconds) identitytoken is provided to the application upon initial authorization with the IDP
                    //AuthorizationCodeLifetime =300, //(in seconds) happens when authorization endpoint is called
                    AccessTokenLifetime = 1800, //lifetime of an access token (default is 1 hour)
                    AllowOfflineAccess = true, //needed to allow accessing the refresh token (refresh token is used to request new ID, access and refresh tokens). At this point the client is not connected to IDP (offline)
                    RefreshTokenExpiration = TokenExpiration.Sliding, //expiration window will slide out (for as long as client is active)
                    //SlidingRefreshTokenLifetime = 432000,//determines how far the expiration will slide, default is 15 days
                    UpdateAccessTokenClaimsOnRefresh = true, //new claims are obtained upon refresh (to ensure they are up-to-date)
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
                        "helloworldapi", //for authorizing the requests to the API
                        "userlevel", //for attribute authorization
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }

            };



    }
}