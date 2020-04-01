using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Organizacion;

namespace PIKA.UI.Web.Areas.Org.Controllers
{

    [Area("Org")]
    [Route("Org/Dominio")]
    public class DominioController : Controller
    {

  

        private readonly IStringLocalizer<HomeController> _localizer;
        public DominioController(IStringLocalizer<HomeController> localizer, 
            IOptions<ConfiguracionServidor> options)
        {
            _localizer = localizer;
            Console.WriteLine($"-->{options.Value.tamanocache}");
        }

        public IActionResult Index()
        {
            ViewModelX model = new ViewModelX()
            {
                Tipo = typeof(Dominio)
            };
            return View(model);
        }

        
        [Route("Vista001")]
        public IActionResult Vista001() =>
         new PartialViewResult
         {
             ViewName = "_Vista001",
             ViewData = ViewData,
         };

    }
}