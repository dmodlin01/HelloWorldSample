using DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace Repositories
{
    public class WebApiMessageRepository : IMessageRepository
    {
        private readonly ILogger<WebApiMessageRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebApiMessageRepository()
        {
        }

        public WebApiMessageRepository(ILogger<WebApiMessageRepository> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public MessageDTO GetMessage()
        {
            _logger?.LogInformation("Using WebApiMessageRepository to retrieve Message.");
            var message = new MessageDTO();

            var client = _httpClientFactory.CreateClient("HelloWorldApiClient");
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
