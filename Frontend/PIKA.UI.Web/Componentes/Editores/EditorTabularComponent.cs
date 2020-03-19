using Microsoft.AspNetCore.Mvc;
using PIKA.Modelo.Infraestructura.UI.EditorTabular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.UI.Web.Componentes.Editores
{
 
    [ViewComponent(Name = "EditorTabular")]
    public class EditorTabularComponent : ViewComponent
    {

        public EditorTabularComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(Type Tipo)
        {

            ModeloEditorTabular m = new ModeloEditorTabular();
            await Task.Delay(1);
   
   
   
            return View(m);
        }

    }
}
