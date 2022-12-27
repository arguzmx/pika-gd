using System.Collections.Generic;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.Request;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
   public interface IServicioCarpeta : IServicioRepositorioAsync<Carpeta, string>,
        IServicioJerarquia<string>,
        IRepositorioJerarquia<Carpeta, string, string>, IServicioAutenticado<Carpeta>
    {
        Task<List<string>> Purgar();

        Task<Carpeta> ObtenerCrearPorRuta(CarpetaDeRuta entidad);

    }
}
