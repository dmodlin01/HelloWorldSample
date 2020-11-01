using DTOs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class MockMessageRepository : IMessageRepository
    {
        private static List<MessageDTO> Messages =>
            new List<MessageDTO>
            {
                new MessageDTO {Message = "Hello World", MessageBody = "Hello Everyone", RecipientId = 8902550, MessageId = 1},
                new MessageDTO {Message = "Greetings", MessageBody = "Greeting Jane", RecipientId = 8902550, MessageId = 2},
                new MessageDTO
                {
                    Message = "Invitation",
                    MessageBody = "Jane, you are cordially invited to the Halloween gala. Costumes are encouraged.",
                    RecipientId = 8902550, MessageId = 3
                },
                new MessageDTO
                {
                    Message = "Invitation",
                    MessageBody =
                        "Bob, we are looking forward to seeing you this halloween. We ask that you please wear a fitting costume (preferably not the homeless look again).",
                    RecipientId = 8775895, MessageId = 4
                },
                new MessageDTO
                {
                    Message = "William, it is your mother. Call me!",
                    MessageBody =
                        "William Smith, for nine months I carried you in my belly.. Is calling your mother once a week too much to ask for?.",
                    RecipientId = 8775895, MessageId = 5
                },
            };
        private readonly ILogger<MockMessageRepository> _logger;
        /// <summary>
        /// implement default constructor to initialize the _message
        ///  </summary>
        public MockMessageRepository(ILogger<MockMessageRepository> logger)
        {
            _logger = logger;

        }

        public MockMessageRepository() { }

        public MessageDTO GetLatestMessage()
        {
            _logger?.LogInformation("Using MockMessageRepository to retrieve the latest Message.");
            return Messages.Last();
        }

        public MessageDTO GetLatestUserMessage(int userId)
        {
            _logger?.LogInformation("Using MockMessageRepository to retrieve the latest Message for user.");
            return Messages.Last(m => m.RecipientId == userId);
        }

        public List<MessageDTO> GetApplicableMessages()
        {
            return Messages;
        }

        public List<MessageDTO> GetUserMessages(int userId)
        {
            return Messages.Where(m => m.RecipientId == userId).ToList();
        }
    }
}