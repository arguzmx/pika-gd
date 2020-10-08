using PIKA.Infraestructura.Comun;
using PIKA.Constantes.Aplicaciones.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public class AplicacionRaiz
    {
        
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = ConstantesAppGestionDocumental.APP_ID,
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
