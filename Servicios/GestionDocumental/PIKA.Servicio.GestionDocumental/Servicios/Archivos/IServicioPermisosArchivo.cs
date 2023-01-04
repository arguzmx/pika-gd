using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;



namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public interface IServicioPermisosArchivo : IServicioRepositorioAsync<PermisosArchivo, string>
        , IServicioAutenticado<PermisosArchivo>
    {
    }
}
