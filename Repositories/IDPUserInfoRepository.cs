using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;

namespace Repositories
{
    public class IDPUserInfoRepository : IUserInfoRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WebApiMessageRepository> _logger;

        public IDPUserInfoRepository(ILogger<WebApiMessageRepository> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }
        public  IEnumerable<Claim> RetrieveUserInfoClaims(string accessToken)
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
                    Token = accessToken
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
    }
}
