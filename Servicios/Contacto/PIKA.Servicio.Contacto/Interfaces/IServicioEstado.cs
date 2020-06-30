using PIKA.Modelo.Contacto;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contacto
{
    public interface  IServicioEstado: IServicioRepositorioAsync<Estado, string>, 
        IServicioValorTextoAsync<Estado>
    {
        Task<List<Estado>> ObtienePorPadre(string padreId);
    }
}
