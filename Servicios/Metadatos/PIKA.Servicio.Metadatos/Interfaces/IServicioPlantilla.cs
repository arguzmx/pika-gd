using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.Interfaces
{
    public interface IServicioPlantilla : IServicioRepositorioAsync<Plantilla, string>
    {
        Task Purgar();

        Task<List<ValorListaPlantilla>> ObtenerValores(string PropiedadId);
    }
}
