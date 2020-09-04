using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public class AplicacionRaiz
    {
        public const string APP_ID = "PIKA-GD-CONTACTO";
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = APP_ID,
                Descripcion = "Permite la gestión de contactos de la aplicación",
                Nombre = "Administrador de contactos",
                UICulture = "es-MX",
                Version = Version,
                ReleaseIndex = 0
            };
            return a;
        }
    }
}
