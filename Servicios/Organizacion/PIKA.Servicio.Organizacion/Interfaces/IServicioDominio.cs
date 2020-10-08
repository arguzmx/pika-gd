using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Organizacion;
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



    }

}
