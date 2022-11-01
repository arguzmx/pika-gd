using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.Registro
{
    public interface IRegistroPIKA
    {
        Task<string> ObtenerFingerprint();
        Task<bool> LicenciaValida();
        Task<bool> ActivarLicencia(string CodigoActivacion);

    }
}
