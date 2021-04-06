using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Modelo.Metadatos
{
    public interface IServicioBusqueda<T, U>
    {
        Task<MetadataInfo> ObtieneMetadatosBusqueda();
        Task<IPaginado<T>> ObtenerPaginadoAsync(Consulta consulta, CancellationToken tokenCancelacion = default);
        Task<IPaginado<U>> ObtenerPaginadoIds(Consulta consulta, CancellationToken tokenCancelacion = default);
        Task<IPaginado<T>> ObtenerConteoAsync(Consulta consulta, CancellationToken tokenCancelacion = default);
    }
}
