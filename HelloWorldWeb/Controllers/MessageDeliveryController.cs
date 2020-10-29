using AutoMapper;
using DTOs;
using HelloWorldWeb.Models;
using HelloWorldWeb.ViewModels;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HelloWorldWeb.Controllers
{
    [Authorize]
    public class MessageDeliveryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public MessageDeliveryController(ILogger<HomeController> logger, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ??
                                 throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> GetMessageDeliveryInfo()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient");

            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();

            if (metaDataResponse.IsError)
            {
                throw new Exception(
                    "Problem accessing the discovery endpoint."
                    , metaDataResponse.Exception);
            }

            var accessToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);//Retrieve access token (needed to access the IDP)

            var userInfoResponse = await idpClient.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Address = metaDataResponse.UserInfoEndpoint, //https://localhost:5001/connect/userinfo
                    Token = accessToken
                });

            if (userInfoResponse.IsError)
            {
                throw new Exception(
                    "Problem accessing the UserInfo endpoint."
                    , userInfoResponse.Exception);
            }

            var fullName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var address = userInfoResponse.Claims
                .FirstOrDefault(c => c.Type == "address")?.Value;
            var phone = userInfoResponse.Claims
                .FirstOrDefault(c => c.Type == "phone_number")?.Value;
            var email = userInfoResponse.Claims
                .FirstOrDefault(c => c.Type == "email")?.Value;
            AddressDTO addressDto = null;
            if (address != null)
                addressDto = JsonConvert.DeserializeObject<AddressDTO>(address);
            return View("MessageDeliveryInfo", new MessageDeliveryInfoVM(fullName, _mapper.Map<AddressVM>(addressDto), phone, email));
        }
    }
}