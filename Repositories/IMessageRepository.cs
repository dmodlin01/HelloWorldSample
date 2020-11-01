using System.Collections.Generic;
using DTOs;
using Serilog;

namespace Repositories
{
    public interface IMessageRepository
    {
        MessageDTO GetLatestMessage();
        MessageDTO GetLatestUserMessage(int userId);
        List<MessageDTO> GetApplicableMessages();
        List<MessageDTO> GetUserMessages(int userId);
    }
}