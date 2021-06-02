using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioElemento : IServicioRepositorioAsync<Elemento, string>
    {
        Task<List<Elemento>> ObtenerPaginadoByIdsAsync(ConsultaAPI q);
        Task<List<string>> Purgar();
    }
}
