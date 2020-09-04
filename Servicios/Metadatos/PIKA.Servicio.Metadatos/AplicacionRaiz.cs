using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos
{
    public class AplicacionRaiz
    {
        public const string APP_ID = "PIKA-GD-METADATOS";
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = APP_ID,
                Descripcion = "Permite la gestión modelos de metadatos de la aplicación",
                Nombre = "Administrador de metadatos",
                UICulture = "es-MX",
                Version = Version,
                ReleaseIndex = 0
            };
            return a;
        }
    }
}
