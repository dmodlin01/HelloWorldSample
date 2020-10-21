using System;
using AutoMapper;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;

namespace HelloWorldWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : Controller
    {
        private IMessageRepository _messageRepository;

        private readonly ILogger<HelloWorldController> _logger;
        private IMapper _mapper;

        public HelloWorldController(ILogger<HelloWorldController> logger, IMessageRepository repository, IMapper mapper)
        {
            _logger = logger;
            _messageRepository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetMessage()
        {
            MessageDTO message = null;
            try
            {
                _logger.LogInformation("Firing the HelloWorld/Get");
                message = _messageRepository.GetMessage();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(message);
        }
    }
}
