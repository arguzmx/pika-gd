using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
   public interface IServicioVolumen : IServicioRepositorioAsync<Volumen, string>,
         IServicioValorTextoAsync<Volumen>
    {
        Task<IGestorES> ObtienInstanciaGestor(string VolumenId);
        Task<List<string>> Purgar();
    }
}
