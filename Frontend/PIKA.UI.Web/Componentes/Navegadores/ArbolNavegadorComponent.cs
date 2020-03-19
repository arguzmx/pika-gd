using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PIKA.UI.Web.Componentes
{

    [ViewComponent(Name = "ArbolNavegador")]
    public class ArbolNavegadorComponent : ViewComponent
    {

        public ArbolNavegadorComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(Type Tipo)
        {

            Console.WriteLine($"{Tipo}");
            await Task.Delay(1);
            List < String > items = new List<string>() { "nUno", "dos", "trs" };
            return View(items);
        }
   
    }
}