// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using DTOs;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using Newtonsoft.Json;

namespace Dmitriy.IDP.Quickstart
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new AddressDTO
                {
                    StreetAddress = "1 Cherry Ln",
                    Locality = "Apple Ville",
                    PostalCode = 12345,
                    Country = "Wonderland"
                };

                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "8775895",
                        Username = "bob",
                        Password = "smith",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Robert"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.PhoneNumber,"847-847-8477"),
                            new Claim(JwtClaimTypes.Email, "bobsmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://bobsmith.com"),
                            new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(address), IdentityServerConstants.ClaimValueTypes.Json)
                        }
                    },

                };
            }
        }
    }
}