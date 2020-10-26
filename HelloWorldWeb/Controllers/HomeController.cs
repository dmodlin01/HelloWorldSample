using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelloWorldWeb.Models;
using HelloWorldWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Repositories;

namespace HelloWorldWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMessageRepository _repository;
        private readonly MessageService _messageService;
        public HomeController(ILogger<HomeController> logger, MessageService messageService, IMessageRepository repository)
        {
            _logger = logger;
            _repository = repository;
            _messageService = messageService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Hello World Web app";
            //var message = _messageService.GetMessage();
            //var messageVm = new MessageVM {Caption = "Message of the day: ", Message = message};
            //return View(messageVm);
            return View();
        }
        [Authorize]
        public async Task<IActionResult> GetMessage()
        {
            await WriteOutIdentityInformation();
            ViewData["Title"] = "Hello World Web app";
            var message = _messageService.GetMessage();
            var messageVm = new MessageVM { Caption = "Message of the day: ", Message = message };
            return View("Index",messageVm);
        }
        public async Task Logout()
        //public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logging out of the session.");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //sign out by clearing app Cookies
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme); //sign out by having the OIDC cookie cleared.
            //return RedirectToAction(actionName: "Index", controllerName: "Home");
            //return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
