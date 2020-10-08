using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad
{
    public class AplicacionRaiz
    {
        
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = ConstantesAppSeguridad.APP_ID,
                Descripcion = "Permite la gestión de la seguridad de la aplicacion",
                Nombre = "Administrador de seguridad",
                UICulture = "es-MX",
                Version = Version,
                ReleaseIndex = 0
            };
            return a;
        }
    }
}
