﻿using AutoMapper;
using CatalogServices;
using HelloWorldWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Repositories;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HelloWorldWeb.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        public HomeController(ILogger<HomeController> logger, MessageService messageService, IMessageRepository repository, IMapper mapper, IAuthorizationService authorizationService) : 
            base(logger, messageService, repository, mapper, authorizationService) { }

        public IActionResult Index()
        {
            ViewData["Title"] = "Hello World Web app";
            return View();
        }
        [Authorize]
        public IActionResult Login()
        {
            ViewData["Title"] = "Hello World Web app";
            return View("Index");
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
