using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HelloWorldWeb.Models;
using HelloWorldWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using AutoMapper;

namespace HelloWorldWeb.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger, MessageService messageService, IMessageRepository repository, IMapper mapper) : base(logger, messageService, repository, mapper){}

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Hello World Web app";
            //var message = _messageService.GetMessage();
            //var messageVm = new MessageVM {Caption = "Message of the day: ", Message = message};
            //return View(messageVm);
            return View();
        }
        public async Task Logout()
        //public async Task<IActionResult> Logout()
        {
            Logger.LogInformation("Logging out of the session.");
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

    }
}
