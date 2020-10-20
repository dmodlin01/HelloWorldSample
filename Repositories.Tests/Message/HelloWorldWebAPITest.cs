using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using HelloWorldWebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace Repositories.Tests.Message
{
    public class HelloWorldWebAPITest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public HelloWorldWebAPITest()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ReturnHelloWorld()
        {
            // Act
            var response = await _client.GetAsync("/helloworld");
            response.EnsureSuccessStatusCode();
            var resultMessage = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageDTO>(resultMessage);
            // Assert
            Assert.Equal("Hello World", message.Message);
        }
    }
}
