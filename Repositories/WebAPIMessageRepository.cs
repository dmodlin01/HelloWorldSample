using System;
using System.Collections.Generic;
using System.Net;
using DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Repositories
{
    public class WebApiMessageRepository : IMessageRepository
    {
        private readonly ILogger<WebApiMessageRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebApiMessageRepository()
        {
        }

        public WebApiMessageRepository(ILogger<WebApiMessageRepository> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public MessageDTO GetLatestMessage()
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve Message.");
            var message = new MessageDTO();


            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");
            //client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", AccessToken);
            //client.DefaultRequestHeaders.Add("Bearer", AccessToken);
            var responseTask = client.GetAsync("/message/getlatestmessage");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var resultMessage = readTask.Result;
                message = JsonConvert.DeserializeObject<MessageDTO>(resultMessage);
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException(result.Content.ToString());
            }
            return message;
        }

       

        public MessageDTO GetLatestUserMessage(int userId)
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve Message.");
            var message = new MessageDTO();


            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");
            //client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", AccessToken);
            //client.DefaultRequestHeaders.Add("Bearer", AccessToken);
            var responseTask = client.GetAsync($"/message/getlatestusermessage/{userId}");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var resultMessage = readTask.Result;
                message = JsonConvert.DeserializeObject<MessageDTO>(resultMessage);
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException(result.Content.ToString());
            }
            return message;
        }

        /// <summary>
        /// If user claims are populated with userID (sub), then only return messages belonging to the user.
        ///Otherwise return all messages.
        /// </summary>
        /// <returns>List of MessageDTO objects</returns>
        public List<MessageDTO> GetApplicableMessages()
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve applicable Messages.");
            var messages = new List<MessageDTO>();
            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");
            var responseTask = client.GetAsync("/message/getmessages");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var resultMessage = readTask.Result;
                messages = JsonConvert.DeserializeObject<List<MessageDTO>>(resultMessage);
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException(result.Content.ToString());
            }
            return messages;
        }

        public List<MessageDTO> GetUserMessages(int userId)
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve user Messages.");
            var messages = new List<MessageDTO>();
            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");
            var responseTask = client.GetAsync($"/message/getusermessages/{userId}");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var resultMessage = readTask.Result;
                messages = JsonConvert.DeserializeObject<List<MessageDTO>>(resultMessage);
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException(result.Content.ToString());
            }
            return messages;
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
