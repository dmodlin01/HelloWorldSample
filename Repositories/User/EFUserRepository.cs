using AutoMapper;
using DTOs;
using Microsoft.Extensions.Logging;
using Repositories.Domain;
using Repositories.EF;
using System.Collections.Generic;

namespace Repositories.User
{
    public class EfUserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly GenericRepository<UserEnt> _userRepository;
        private readonly ILogger<EfUserRepository> _logger;

        public GenericRepository<UserEnt> UserRepository => _userRepository ?? new GenericRepository<UserEnt>(_appDbContext);

        // constructor used to inject the context
        public EfUserRepository(AppDbContext appDbContext, IMapper mapper, ILogger<EfUserRepository> logger)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _logger = logger;
            _userRepository = new GenericRepository<UserEnt>(_appDbContext);
        }
        public void AddUser(ref UserDTO userDTO)
        {
            var userEnt = _mapper.Map<UserEnt>(userDTO);
            UserRepository.Insert(userEnt);
            UserRepository.SaveChanges();
            userDTO.UserId = userEnt.UserId; //assign back the id
        }

        UserDTO IUserRepository.GetUser(int userId)
        {
            var userEnt = UserRepository.GetById(userId);
            return _mapper.Map<UserDTO>(userEnt);
        }

        List<UserDTO> IUserRepository.GetUsers()
        {
            var userEnts = UserRepository.Get();
            return _mapper.Map<List<UserDTO>>(userEnts);
        }
    }
}
