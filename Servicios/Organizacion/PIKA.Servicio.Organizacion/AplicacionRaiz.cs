using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Infraestructura.Comun;
 

namespace PIKA.Servicio.Organizacion
{
    public class AplicacionRaiz
    {
      
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = ConstantesAppOrganizacion.APP_ID,
                Descripcion = "Permite la gestión de elemtos de la organización",
                Nombre = "Administrador de organización",
                UICulture = "es-MX",
                Version = Version,
                ReleaseIndex = 0
            };
            return a;
        }
    }
}
