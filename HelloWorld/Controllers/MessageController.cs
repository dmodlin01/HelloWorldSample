using AutoMapper;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HelloWorldWebAPI.Controllers
{
    [Route("[controller]")]
    [Route("[controller]/[Action]")]
    [ApiController]
    [Authorize] //service will be authorized
    public class MessageController : Controller
    {
        private IMessageRepository _messageRepository;

        private readonly ILogger<MessageController> _logger;
        private IMapper _mapper;

        public MessageController(ILogger<MessageController> logger, IMessageRepository repository, IMapper mapper)
        {
            _logger = logger;
            _messageRepository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetLatestMessage()
        {

            MessageDTO message = null;
            try
            {
                var userId = 0;
                if (User.Claims.Any())
                {
                    var client = User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                    var idValue = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "0";
                    userId = int.Parse(idValue);
                    _logger.LogInformation($"UserId {idValue} originating from {client} is firing the Message/GetLatestMessage");
                }
                else
                    _logger.LogInformation("Firing the Message/Get");
                message = User.IsInRole("Admin")
                    ? _messageRepository.GetLatestMessage()
                    : userId != 0 ? _messageRepository.GetLatestUserMessage(userId) : null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(message);
        }


        
        [HttpGet("GetLatestUserMessageById/{userid}")]
        //[Route("GetLatestUserMessageById")]
        [Authorize(Policy = "CanViewSpecificMessage")]
        public IActionResult GetLatestUserMessageById(int userid)
        {

            MessageDTO message = null;
            try
            {

                if (User.Claims.Any())
                {
                    var client = User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                    var userIdVal = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    _logger.LogInformation($"UserId {userIdVal} originating from {client} is firing the Message/GetLatestUserMessage");
                }
                else
                    _logger.LogInformation("Firing the Message/Get");

                message = _messageRepository.GetLatestUserMessage(userid);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(message);
        }

        [HttpGet("{messageId}")]
        public IActionResult GetMessageById(int messageId)
        {

            MessageDTO message = null;
            try
            {

                if (User.Claims.Any())
                {
                    var client = User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                    var userIdVal = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    _logger.LogInformation($"UserId {userIdVal} originating from {client} is firing the Message/GetMessageById");
                }
                else
                    _logger.LogInformation("Firing the Message/Get");

                message = _messageRepository.GetMessageById(messageId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(message);
        }
        /// <summary>
        /// If user claims are populated with userID (sub), then only return messages belonging to the user.
        ///Otherwise return all messages.
        /// </summary>
        /// <returns>List of MessageDTO objects</returns>
        [HttpGet]
        [Route("GetMessages")]
        public IActionResult GetMessages()
        {
            List<MessageDTO> messages = null;
            try
            {
                var userId = 0;
                if (User.Claims.Any())
                {
                    var client = User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                    var idValue = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "0";
                    userId = int.Parse(idValue);
                    _logger.LogInformation($"UserId {idValue} originating from {client} is firing the Message/Get");
                }
                else
                    _logger.LogInformation("Firing the Messages/Get");
                messages = User.IsInRole("Admin")
                   ? _messageRepository.GetAvailableMessages()
                   : userId != 0 ? _messageRepository.GetUserMessages(userId) : null;
            }
            catch (Exception e)
           {
               _logger.LogError(e.Message, e);
           }
           return Ok(messages);
       }

        [HttpGet("{userid}")]
        [Authorize(Policy = "CanViewSpecificMessage")]
        public IActionResult GetUserMessages(int userid)
        {

            List<MessageDTO> messages = null;
            try
            {

                if (User.Claims.Any())
                {
                    var client = User.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;
                    var idValue = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    _logger.LogInformation($"UserId {idValue} originating from {client} is firing the Message/Get");
                }
                else
                    _logger.LogInformation("Firing the Messages/Get");

                messages = _messageRepository.GetUserMessages(userid);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(messages);
        }
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "CanAddMessage")]
        public IActionResult AddMessage([FromBody] MessageDTO messageDTO)
        {
            _logger.LogInformation("Firing the Message/AddMessage");
            _messageRepository.AddMessage(ref messageDTO);

            return Ok(messageDTO); ;
        }

    }
}
