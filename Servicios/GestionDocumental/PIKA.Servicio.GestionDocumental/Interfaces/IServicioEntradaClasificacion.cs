using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioEntradaClasificacion : 
        IServicioRepositorioAsync<EntradaClasificacion, string>,
          IServicioValorTextoAsync<EntradaClasificacion>
    {
        Task<ICollection<string>> Purgar();

    }

}
