﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldWeb.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
