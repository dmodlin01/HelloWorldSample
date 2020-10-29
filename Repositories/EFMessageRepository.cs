using AutoMapper;
using DTOs;
using System.Linq;
using Microsoft.Extensions.Logging;
using Repositories.Domain;
using Repositories.EF;

namespace Repositories
{
    public class EfMessageRepository : IMessageRepository
    {
        private readonly AppDbContext _appDbContext;
        private IMapper _mapper;
        private GenericRepository<MessageEnt> _messageRepository;
        private ILogger<EfMessageRepository> _logger;

        public GenericRepository<MessageEnt> MessageRepository => _messageRepository ?? new GenericRepository<MessageEnt>(_appDbContext);

        // constructor used to inject the context
        public EfMessageRepository(AppDbContext appDbContext, IMapper mapper, ILogger<EfMessageRepository> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _logger = logger;
            _messageRepository = new GenericRepository<MessageEnt>(_appDbContext);
        }
        public MessageDTO GetMessage()
        {
            _logger?.LogInformation("Using EFMessageRepository to retrieve Message.");
            //var messageEnt = _appDbContext.Messages.FirstOrDefault();
            var messageEnt = MessageRepository.FirstOrDefault();
            return _mapper.Map<MessageDTO>(messageEnt);

        }
        public MessageDTO GetMessageById(int id)
        {
            //var messageEnt = _appDbContext.Messages.FirstOrDefault(m => m.MessageId == id);
            var messageEnt = MessageRepository.GetById(id);
            return _mapper.Map<MessageDTO>(messageEnt);
        }
    }
}
