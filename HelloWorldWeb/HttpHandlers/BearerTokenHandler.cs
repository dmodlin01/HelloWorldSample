﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorldWeb.HttpHandlers
{

    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor; //needed to acces HttpContext
        private readonly IHttpClientFactory _httpClientFactory;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor,
                   IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClientFactory = httpClientFactory ??
                 throw new ArgumentNullException(nameof(httpClientFactory));
        }


        /// <summary>
        /// Retrieve the Access Token and apply the token onto the request as a bearerToken
        /// </summary>
        /// <param name="request">request to apply the token to</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(  //override the base async token
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken); //add the access token as a bearer token o
            }

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Retrieve the Access Token from httpContext
        /// if the token is expired
        /// </summary>
        /// <returns>AccessToken</returns>
        public async Task<string> GetAccessTokenAsync()
        {
            // get the expires_at value & parse it
            var expiresAt = await _httpContextAccessor
                .HttpContext.GetTokenAsync("expires_at");

            var expiresAtAsDateTimeOffset =
                DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);

            if (expiresAtAsDateTimeOffset.AddSeconds(-60).ToUniversalTime() > DateTime.UtcNow) //check if the token won't to expire within the next 60 sec
            {
                // no need to refresh, return the access token
                return await _httpContextAccessor
                       .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }

            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            // get the discovery document
            var discoveryReponse = await idpClient.GetDiscoveryDocumentAsync(); //need it to determine the IDP token

            // refresh the tokens
            var refreshToken = await _httpContextAccessor  //Request refreshtoken from IDP
                       .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var refreshResponse = await idpClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = discoveryReponse.TokenEndpoint,
                    ClientId = "helloworldwebclient",
                    ClientSecret = "secret",
                    RefreshToken = refreshToken
                });

            // store the updated tokens             
            var updatedTokens = new List<AuthenticationToken>();
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = refreshResponse.IdentityToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = refreshResponse.AccessToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = refreshResponse.RefreshToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn)).
                        ToString("o", CultureInfo.InvariantCulture)
            });

            // get authenticate result, containing the current principal & 
            // properties
            var currentAuthenticateResult = await _httpContextAccessor
                .HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // store the updated tokens
            currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

            // sign in
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                currentAuthenticateResult.Principal,
                currentAuthenticateResult.Properties);

            return refreshResponse.AccessToken;
        }
    }
}

