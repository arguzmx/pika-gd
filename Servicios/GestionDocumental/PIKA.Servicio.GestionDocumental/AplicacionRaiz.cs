using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental
{
    public class AplicacionRaiz
    {
        public const string APP_ID = "PIKA-GD-GDOCUMENTAL";
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = APP_ID,
                Descripcion = "Permite los procesos de gestión documental en la aplicación",
                Nombre = "Administrador de gestión documental",
                UICulture = "es-MX",
                Version = Version,
                ReleaseIndex = 0
            };
            return a;
        }
    }
}
