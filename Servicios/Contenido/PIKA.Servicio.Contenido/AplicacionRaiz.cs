using PIKA.Infraestructura.Comun;
using PIKA.Constantes.Aplicaciones.Contenido;

namespace PIKA.Servicio.Contenido
{
    public class AplicacionRaiz
    {
        
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = ConstantesAppContenido.APP_ID,
                Descripcion = "Permite la gestión del contenido y la administración de respositorios documentales",
                Nombre = "Administrador de Contenido",
                UICulture = "es-MX",
                Version = Version,
                ReleaseIndex = 0
            };
            return a;
        }
    }
}
