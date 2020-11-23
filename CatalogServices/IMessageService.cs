using System;
using System.Collections.Generic;
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

        public MessageDTO GetLatestMessage()
        {
            Logger?.LogInformation($"Using {this.GetType()} with {_messageRepository.GetType()} to retrieve message");
            MessageDTO message = null;
            try
            {
                message = _messageRepository.GetLatestMessage();
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message, e);
                throw;
            }
            return message;
        }
        public MessageDTO GetLatestMessageForUser(int userId)
        {
            Logger?.LogInformation($"Using {this.GetType()} with {_messageRepository.GetType()} to retrieve message");
            MessageDTO message = null;
            try
            {
                message = _messageRepository.GetLatestUserMessage(userId);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message, e);
                throw;
            }
            return message;
        }
        public List<MessageDTO> GetAvailableMessages()
        {
            Logger?.LogInformation($"Using {this.GetType()} with {_messageRepository.GetType()} to retrieve message");
            List<MessageDTO> messages;
            try
            {
                messages = _messageRepository.GetAvailableMessages();
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message, e);
                throw;
            }
            return messages;
        }
        public List<MessageDTO> GetMessagesForUser(int userId)
        {
            Logger?.LogInformation($"Using {this.GetType()} with {_messageRepository.GetType()} to retrieve message");
            List<MessageDTO> messages;
            try
            {
                messages = _messageRepository.GetUserMessages(userId);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message, e);
                throw;
            }
            return messages;
        }

        public MessageDTO GetMessageById(int messageId)
        {
            Logger?.LogInformation($"Using {this.GetType()} with {_messageRepository.GetType()} to retrieve message");
            MessageDTO message;
            try
            {
                message = _messageRepository.GetMessageById(messageId);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message, e);
                throw;
            }
            return message;
        }
    }
}
