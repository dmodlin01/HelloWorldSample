using DTOs;
using System.Collections.Generic;

namespace Repositories
{
    public interface IMessageRepository
    {
        MessageDTO GetLatestMessage();
        MessageDTO GetLatestUserMessage(int userId);
        List<MessageDTO> GetAvailableMessages();
        List<MessageDTO> GetUserMessages(int userId);
        void AddMessage(ref MessageDTO messageDTO);
        MessageDTO GetMessageById(int messageId);
    }
}