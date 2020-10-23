using System.Collections.Generic;
using System.Threading.Tasks;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivo : IServicioRepositorioAsync<Activo, string>
    {
        Task<byte[]> ImportarActivos(byte[]file,string ArchivId,string TipoId, string OrigenId,string formatoFecha);
        Task<List<string>> Purgar();
        Task<byte[]> ReporteCaratulaActivo(string Dominio, string UnidadOrganizacinal, string ActivoId);

    }
}