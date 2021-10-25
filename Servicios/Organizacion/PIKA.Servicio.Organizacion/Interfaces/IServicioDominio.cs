using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
using PIKA.Modelo.Organizacion.Estructura;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion
{
 

    public interface IServicioDominio : IServicioRepositorioAsync<Dominio, string>
    {
        Task<string[]> Purgar();
        Task<bool> ActualizaDominioOU(ActDominioOU request, string DominioId, string OUId);
        Task<ActDominioOU> OntieneDominioOU(string DominioId, string OUId);

    }

}
