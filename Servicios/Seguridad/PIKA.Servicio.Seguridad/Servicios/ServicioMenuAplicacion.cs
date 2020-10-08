using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Menus;
using PIKA.Servicio.Seguridad.Interfaces;
using PIKA.Servicio.Seguridad.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioMenuAplicacion : IServicioMenuAplicacion, IServicioInyectable
    {
        public async Task<MenuAplicacion> ObtieneMenuApp(string Id)
        {

            MenuAplicacionPIKAAngular menu = new MenuAplicacionPIKAAngular();
            return await menu.ObtieneMenuApp(Id);
        }


    }
}
