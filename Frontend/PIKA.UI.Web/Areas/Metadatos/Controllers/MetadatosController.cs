using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PIKA.UI.Web.Areas.Metadatos.Controllers
{
    public class MetadatosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}