using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
   public interface IServicioEstadisticaClasificacionAcervo 
    {
        Task<byte[]> ReporteEstadisticaArchivoCuadro(string ArchivoId, string CuadroClasificacionId, bool IncluirCeros);
        
        Task ActualizarConteos(string ArchivoId);

        Task ActualizaEstadistica(EstadisticaClasificacionAcervo estadistica, int adicionar, int eliminar);
        Task AdicionaEstadistica(EstadisticaClasificacionAcervo estadistica);
        Task EliminaEstadistica(EstadisticaClasificacionAcervo estadistica, bool CambioDocumental = false);

    }
}
