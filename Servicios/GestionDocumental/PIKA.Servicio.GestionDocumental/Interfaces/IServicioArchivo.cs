using PIKA.Infraestrctura.Reportes;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioArchivo : IServicioRepositorioAsync<Archivo, string>,
        IServicioValorTextoAsync<Archivo>
    {
        Task<ICollection<string>> Purgar();

        Task<byte[]> ReporteGuiaSimpleArchivo(string ArchivoId, List<ElementoReporte> reportes);

        Task<string> ReporteGuiaInventario(string ArchivoId);

    }
}
