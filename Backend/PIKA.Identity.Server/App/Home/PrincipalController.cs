using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PIKA.Identity.Server.App.Home
{
    [Authorize]
    public class PrincipalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}