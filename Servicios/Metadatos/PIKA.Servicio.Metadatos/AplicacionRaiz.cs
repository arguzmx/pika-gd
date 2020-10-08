using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public class AplicacionRaiz
    {
        
        public const string Version = "1.0";
        public static Aplicacion ObtieneAplicacionRaiz()
        {
            Aplicacion a = new Aplicacion()
            {
                Id = ConstantesAppMetadatos.APP_ID,
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
