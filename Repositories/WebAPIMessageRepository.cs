using System;
using System.Net.Http;
using DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Repositories
{
    public class WebApiMessageRepository : IMessageRepository
    {
        private readonly ILogger<WebApiMessageRepository> _logger;

        public WebApiMessageRepository(ILogger<WebApiMessageRepository> logger)
        {
            _logger = logger;
        }
        public MessageDTO GetMessage()
        {
            _logger.LogInformation("Using WebApiMessageRepository to retrieve Message.");
            var message = new MessageDTO();
            using var client = new HttpClient {BaseAddress = new Uri("https://localhost:44349/helloworld/")};
            //HTTP GET
            var responseTask = client.GetAsync("/helloworld");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                var resultMessage = readTask.Result;
                message = JsonConvert.DeserializeObject<MessageDTO>(resultMessage);
            }
            return message;
        }
    }
}
