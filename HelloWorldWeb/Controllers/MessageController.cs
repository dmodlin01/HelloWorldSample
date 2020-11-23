using AutoMapper;
using CatalogServices;
using DTOs;
using HelloWorldWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using Repositories.EF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldWeb.Controllers
{
    public class MessageController : BaseController<MessageController>
    {
        internal IUserRepository UserRepository;
        private List<SelectListItem> _userItems;
        internal List<SelectListItem> UserItems => _userItems ??= UserRepository.GetUsers()
            .Select(u => new SelectListItem(u.FullName, u.UserId.ToString())).ToList();




        public MessageController(ILogger<MessageController> logger, MessageService messageService, IMessageRepository messageRepo, IMapper mapper, IUserRepository userRepo, IAuthorizationService authorizationService)
            : base(logger, messageService, messageRepo, mapper, authorizationService)
        {
            UserRepository = userRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> GetMessages()
        {
            await WriteOutIdentityInformation();
            ViewData["Title"] = "Hello World Web app";
            try
            {
                var userId = 0;
                if (User.Claims.Any(c => c.Type == "sub")) //check if the claim containing userID exists
                {
                    var idVal = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "0";
                    userId = int.Parse(idVal);
                }
                var message = userId == 0 ? MessageService.GetLatestMessage() : MessageService.GetLatestMessageForUser(userId);

                var allowed = await AuthorizationService.AuthorizeAsync(User, "CanViewAllMessages");

                var messages = allowed.Succeeded ? MessageService.GetAvailableMessages() :
                    userId != 0 ? MessageService.GetMessagesForUser(userId) : new List<MessageDTO>();

                var messageVm = new MessagesVM { LatestMessage = message, RemainingMessages = messages.Where(m => m.MessageId != message.MessageId).ToList() };
                if (messageVm.LatestMessage == null)
                    messageVm.addMessageVM = new AddMessageVM() { Users = UserItems };
                return View("Index", messageVm);
            }
            catch (Exception e) when (e is AggregateException && e.InnerException is SecurityTokenExpiredException || e is UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var message = MessageService.GetMessageById(id);
            var messageVm = Mapper.Map<MessageVM>(message);

            if (message.RecipientId.HasValue)
                messageVm.Recipient = UserRepository.GetUser(message.RecipientId.Value);

            return View(messageVm);


        }

        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "CanAddMessage")]
        public IActionResult AddMessage()
        {
            return View(new AddMessageVM() { Users = UserItems });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanAddMessage")]
        public IActionResult AddMessage(AddMessageVM addMessageVM)
        {

            if (!ModelState.IsValid)
            {
                var users = UserRepository.GetUsers();
                var userSelectListItems = users.Select(u => new SelectListItem(u.FullName, u.UserId.ToString())).ToList();
                return View(new AddMessageVM() { Users = UserItems });
            }
            var messageDTO = Mapper.Map<MessageDTO>(addMessageVM);
            try
            {
                MessageRepository.AddMessage(ref messageDTO);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                ViewBag.Error = $"Error: {e.Message}";
                return View(new AddMessageVM() { Users = UserItems });
            }
            return RedirectToAction("GetMessages");
        }



        public async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);//retrieve the saved identity token

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
