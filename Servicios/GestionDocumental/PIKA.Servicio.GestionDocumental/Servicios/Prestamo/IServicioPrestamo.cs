using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioPrestamo : IServicioRepositorioAsync<Prestamo, string>, IServicioAutenticado<Prestamo>
    {
        Task<List<string>> Purgar();
        Task<Prestamo> CrearDesdeTemaAsync(Prestamo entity, string TemaId, CancellationToken cancellationToken = default);
        Task<RespuestaComandoWeb> ComandoWeb(string command, object payload);
        Task<byte[]> ReportePrestamo(string ExtensionSalida, string PrestamoId, UsuarioPrestamo Prestador, UsuarioPrestamo Prestatario, string Dominio = "*", string UnidadOrganizacinal = "*");
    }
}
