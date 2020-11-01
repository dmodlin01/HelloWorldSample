﻿using AutoMapper;
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

                message = userId == 0 ? _messageRepository.GetLatestMessage() : _messageRepository.GetLatestUserMessage(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(message);
        }
        [Route("{id}")]
        public IActionResult GetLatestUserMessage(int id)
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

                message = _messageRepository.GetLatestUserMessage(id);
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

                messages = userId == 0 ? _messageRepository.GetApplicableMessages() : _messageRepository.GetUserMessages(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(messages);
        }
        [Route("{id}")]
        public IActionResult GetUserMessages(int id)
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

                messages = _messageRepository.GetUserMessages(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
            return Ok(messages);
        }
    }
}