using AutoMapper;
using DTOs;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Repositories
{
    public class EFMessageRepository : IMessageRepository
    {
        private readonly AppDbContext _appDbContext;
        private IMapper _mapper;
        private ILogger<EFMessageRepository> _logger;

        // constructor used to inject the context
        public EFMessageRepository(AppDbContext appDbContext, IMapper mapper, ILogger<EFMessageRepository> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _logger = logger;
        }
        public MessageDTO GetMessage()
        {
            _logger?.LogInformation("Using EFMessageRepository to retrieve Message.");
            var messageEnt = _appDbContext.Messages.FirstOrDefault();
            return _mapper.Map<MessageDTO>(messageEnt);


        }
        public MessageDTO GetMessageById(int id)
        {
            var messageEnt = _appDbContext.Messages.FirstOrDefault(m => m.MessageId == id);
            return _mapper.Map<MessageDTO>(messageEnt);
        }
    }
}
