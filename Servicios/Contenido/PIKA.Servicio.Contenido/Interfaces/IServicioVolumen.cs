using System.Collections.Generic;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch.modelos;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
   public interface IServicioVolumen : IServicioRepositorioAsync<Volumen, string>,
         IServicioValorTextoAsync<Volumen>, IServicioAutenticado<Volumen>
    {
        Task<IGestorES> ObtienInstanciaGestor(string VolumenId);
        Task<List<string>> Purgar();
        Task ActualizaEstadisticas(EstadisticaVolumen s, string Id);
    }
}
