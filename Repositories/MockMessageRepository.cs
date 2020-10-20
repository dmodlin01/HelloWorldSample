using DTOs;
using Microsoft.Extensions.Logging;

namespace Repositories
{
    public class MockMessageRepository : IMessageRepository
    {
        private string _message;
        private readonly ILogger<MockMessageRepository> _logger;
        /// <summary>
        /// implement default constructor to initialize the _message
        ///  </summary>
        public MockMessageRepository(ILogger<MockMessageRepository> logger)
        {
            _logger = logger;
            _message = "Hello World";
        }
        public MockMessageRepository()
        {
            _message = "Hello World";
        }

        /// <summary>
        /// constructor to initialize the _message with the provided parameter
        ///  </summary>
        /// <param name="logger">DI parameter</param>
        /// <param name="message">message parameter</param>
        public MockMessageRepository(ILogger<MockMessageRepository> logger, string message)
        {
            _logger = logger;
            _message = message;
        }
        public MessageDTO GetMessage()
        {
            _logger?.LogInformation("Using MockMessageRepository to retrieve Message.");
            return new MessageDTO { Message = _message };
        }
    }
}