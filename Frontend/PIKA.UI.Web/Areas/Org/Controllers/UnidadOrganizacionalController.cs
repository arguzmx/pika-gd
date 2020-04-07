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
    [Route("Org/UnidadOrganizacional")]
    public class UnidadOrganizacionalController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        public UnidadOrganizacionalController(IStringLocalizer<HomeController> localizer,
            IOptions<ConfiguracionServidor> options)
        {
            _localizer = localizer;
            Console.WriteLine($"-->{options.Value.tamanocache}");
        }
        public IActionResult Index()
        {
            ViewModelX model = new ViewModelX()
            {
                Tipo = typeof(UnidadOrganizacional)
            };
            return View(model);
        }

        [Route("Crear")]
        public IActionResult CrearUO() => new PartialViewResult
        {
            ViewName = "_CrearUO",
            ViewData = ViewData

        };


        [Route("Editar")]
        public IActionResult EditarUO(string Id) => new PartialViewResult
        {
            ViewName = "_EditarUO",
            ViewData = ViewData
        };

        [Route("Eliminar")]
        public IActionResult EliminarUO(string[] Ids) => new PartialViewResult
        {
            ViewName = "_EliminarUO",
            ViewData = ViewData
        };

    }
}