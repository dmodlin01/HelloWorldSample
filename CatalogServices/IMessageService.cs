using System;
using DTOs;
using Microsoft.Extensions.Logging;
using Repositories;

namespace CatalogServices
{
    public abstract class MessageService
    {
        private readonly IMessageRepository _messageRepository;
        protected readonly ILogger Logger;

        protected MessageService(ILogger logger, IMessageRepository messageRepository)
        {
            Logger = logger;
            _messageRepository = messageRepository;
        }

        public  MessageDTO GetMessage()
        {
            Logger?.LogInformation($"Using {this.GetType()} with {_messageRepository.GetType()} to retrieve message");
            MessageDTO message = null;
            try
            {
                message = _messageRepository.GetMessage();
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message, e);
            }
            return message;
        }
    }
}
