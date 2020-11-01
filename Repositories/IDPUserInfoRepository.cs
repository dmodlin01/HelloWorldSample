using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Repositories
{
    public class IDPUserInfoRepository : IUserInfoRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WebApiMessageRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IDPUserInfoRepository(ILogger<WebApiMessageRepository> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpContextAccessor = httpContextAccessor;
        }
        public  IEnumerable<Claim> RetrieveUserInfoClaims()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            var metaDataResponseTask = idpClient.GetDiscoveryDocumentAsync();
            metaDataResponseTask.Wait(); //wait for execution completion
            var mdrResult = metaDataResponseTask.Result;
            if (mdrResult.IsError)
            {
                throw new Exception(
                    "Problem accessing the discovery endpoint."
                    , mdrResult.Exception);
            }
            var userInfoResponseTask = idpClient.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Address = mdrResult.UserInfoEndpoint, //https://localhost:5001/connect/userinfo
                    Token = AccessToken
                });
            userInfoResponseTask.Wait();
            var userInfoResult = userInfoResponseTask.Result;
            if (userInfoResult.IsError)
            {
                throw new Exception(
                    "Problem accessing the UserInfo endpoint."
                    , userInfoResult.Exception);
            }
            return userInfoResult.Claims;
        }

        private string AccessToken
        {
            get
            {
                var accessTokenTask =
                    _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames
                        .AccessToken); //Retrieve access token (needed to access the IDP)
                accessTokenTask.Wait(); //wait for completion
                return accessTokenTask.Result;
            }
        }
    }
}
