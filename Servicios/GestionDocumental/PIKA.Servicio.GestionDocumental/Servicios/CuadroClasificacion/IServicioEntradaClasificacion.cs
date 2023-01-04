using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioEntradaClasificacion : 
        IServicioRepositorioAsync<EntradaClasificacion, string>,
          IServicioValorTextoAsync<EntradaClasificacion>, IServicioAutenticado<EntradaClasificacion>
    {
        Task<ICollection<string>> Purgar();

    }

}
