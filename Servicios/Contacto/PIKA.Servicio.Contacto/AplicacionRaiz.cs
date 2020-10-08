using PIKA.Constantes.Aplicaciones.Contacto;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public class AplicacionRaiz
    {

        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = ConstantesAppContacto.MODULO_BASE,
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
