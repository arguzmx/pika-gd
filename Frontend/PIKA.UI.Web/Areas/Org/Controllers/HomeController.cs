using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion;
using PIKA.UI.Web.Models;

namespace PIKA.UI.Web.Areas.Org.Controllers
{

    [Area("Org")]
    [Route("Org/Home")]
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        public HomeController(IStringLocalizer<HomeController> localizer, IOptions<ConfiguracionServidor> options, 
            IServicioUnidadOrganizacional servicioUnidadOrganizacional)
        {
            _localizer = localizer;
            Console.WriteLine($"-->{options.Value.tamanocache}");

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}