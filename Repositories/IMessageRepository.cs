using DTOs;
using Serilog;

namespace Repositories
{
    public interface IMessageRepository
    {
        MessageDTO GetMessage();
    }
}