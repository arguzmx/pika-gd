using PIKA.Infraestructura.Comun.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioMenuAplicacion
    {
        Task<MenuAplicacion> ObtieneMenuApp(string Id);
    }
}
