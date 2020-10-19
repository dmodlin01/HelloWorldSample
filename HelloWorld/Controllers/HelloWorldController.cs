using System;
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

        public HelloWorldController(ILogger<HelloWorldController> logger, IMessageRepository repository)
        {
            _logger = logger;
            _messageRepository = repository;

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
