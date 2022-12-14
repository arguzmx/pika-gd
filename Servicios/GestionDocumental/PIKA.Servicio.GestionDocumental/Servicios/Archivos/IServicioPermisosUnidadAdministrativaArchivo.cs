using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;


namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioPermisosUnidadAdministrativaArchivo: IServicioRepositorioAsync<PermisosUnidadAdministrativaArchivo, string>
        , IServicioAutenticado<PermisosUnidadAdministrativaArchivo>
    {
    }
}
