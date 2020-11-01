using System.Collections.Generic;
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
        private readonly IMapper _mapper;
        private readonly GenericRepository<MessageEnt> _messageRepository;
        private readonly ILogger<EfMessageRepository> _logger;

        public GenericRepository<MessageEnt> MessageRepository => _messageRepository ?? new GenericRepository<MessageEnt>(_appDbContext);

        // constructor used to inject the context
        public EfMessageRepository(AppDbContext appDbContext, IMapper mapper, ILogger<EfMessageRepository> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _logger = logger;
            _messageRepository = new GenericRepository<MessageEnt>(_appDbContext);
        }
        public MessageDTO GetLatestMessage()
        {
            _logger?.LogInformation("Using EFMessageRepository to retrieve the latest Message.");
            var messageEnt = MessageRepository.Get(orderBy: m => m.OrderByDescending(ord => ord.MessageId)).FirstOrDefault();
            return _mapper.Map<MessageDTO>(messageEnt);
        }
        public MessageDTO GetLatestUserMessage(int userId)
        {
            _logger?.LogInformation("Using EFMessageRepository to retrieve latest Message by user.");
            var messageEnt = MessageRepository.Get(filter:m=>m.UserId==userId,orderBy:m=>m.OrderByDescending(ord=>ord.MessageId)).FirstOrDefault();
            return _mapper.Map<MessageDTO>(messageEnt);
        }

        public List<MessageDTO> GetApplicableMessages()
        {
            var messageEnts = MessageRepository.Get().ToList();
            return _mapper.Map<List<MessageDTO>>(messageEnts);
        }

        public List<MessageDTO> GetUserMessages(int userId)
        {
            var messageEnts = MessageRepository.Get(m=>m.UserId == userId);
            return _mapper.Map<List<MessageDTO>>(messageEnts);
        }

        public MessageDTO GetMessageById(int id)
        {
            var messageEnt = MessageRepository.GetById(id);
            return _mapper.Map<MessageDTO>(messageEnt);
        }
    }
}
