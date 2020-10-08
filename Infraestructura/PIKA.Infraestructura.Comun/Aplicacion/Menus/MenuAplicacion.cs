using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Menus
{
    public class MenuAplicacion
    {
        public MenuAplicacion()
        {
            Elementos = new List<ElementoMenu>();
        }

        public string AppId { get; set; }
        public string AppNombre { get; set; }
        public List<ElementoMenu> Elementos { get; set; }
    }
}
