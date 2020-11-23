using AutoMapper;
using CatalogServices;
using DTOs;
using HelloWorldWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorldWeb.Components
{
    public class MessageSummary : ViewComponent
    {
        private readonly ILogger<MessageSummary> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly MessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MessageSummary(ILogger<MessageSummary> logger, MessageService messageService, IMessageRepository messageRepo, IMapper mapper, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _messageRepository = messageRepo;
            _messageService = messageService;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async System.Threading.Tasks.Task<IViewComponentResult> InvokeAsync() //method that gets called when viewcomponent is being invoked
        {
            var userId = 0;
            if (_httpContextAccessor.HttpContext.User.Claims.Any(c => c.Type == "sub")) //check if the claim containing userID exists
            {
                var idVal = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "0";
                userId = int.Parse(idVal);
            }
            var allowed = await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, "CanViewAllMessages");
            var messageVm = new MessageSummaryVM();
            try
            {
                var messages = allowed.Succeeded ? _messageService.GetAvailableMessages() :
                    userId != 0 ? _messageService.GetMessagesForUser(userId) : new List<MessageDTO>();
                messageVm.Messages = messages;
                
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
            }

            return View(messageVm);
        }
    }
}
