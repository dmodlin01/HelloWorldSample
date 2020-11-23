using AutoMapper;
using CatalogServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;

namespace HelloWorldWeb.Controllers
{
    public class BaseController<T> : Controller
    {
        internal readonly ILogger<T> Logger;
        internal readonly IMessageRepository MessageRepository;
        internal readonly MessageService MessageService;
        internal readonly IMapper Mapper;
        internal readonly IAuthorizationService AuthorizationService;
        public BaseController(ILogger<T> logger, MessageService messageService, IMessageRepository repository, IMapper mapper, IAuthorizationService authorizationService)
        {
            Logger = logger;
            MessageRepository = repository;
            MessageService = messageService;
            Mapper = mapper;
            AuthorizationService = authorizationService;
        }
    }
}
