using DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

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
            var responseTask = client.GetAsync($"/message/getlatestusermessagebyid/{userId}");
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
        public MessageDTO GetMessageById(int messageId)
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve Message.");
            var message = new MessageDTO();
            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");
            var responseTask = client.GetAsync($"/message/getmessagebyid/{messageId}");
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
        /// If user is admin, all messages should be returned, otherwise messages are returned per the userID (sub, extracted from claims).
        ///Otherwise return all messages.
        /// </summary>
        /// <returns>List of MessageDTO objects</returns>
        public List<MessageDTO> GetAvailableMessages()
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve available Messages.");
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

        public void AddMessage(ref MessageDTO messageDTO)
        {
            _logger?.LogInformation("Using WebApiMessageRepository to add Message.");
            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");

            var serializedMessage = JsonConvert.SerializeObject(messageDTO);
            var request = new HttpRequestMessage(HttpMethod.Post, $"/message/addmessage")
            {
                Content = new StringContent(
                serializedMessage,
                System.Text.Encoding.Unicode,
                "application/json")
            };

            var responseTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var resultMessage = readTask.Result;
                messageDTO = JsonConvert.DeserializeObject<MessageDTO>(resultMessage);
            }
            else
            {
                if (result.StatusCode == HttpStatusCode.Unauthorized || result.StatusCode == HttpStatusCode.Forbidden)
                    throw new UnauthorizedAccessException(result.ToString());
                else
                    throw new Exception(result.ToString());
            }

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
