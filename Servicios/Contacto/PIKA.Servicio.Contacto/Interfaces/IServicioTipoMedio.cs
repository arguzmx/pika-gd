using PIKA.Modelo.Contacto;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contacto
{
    public interface IServicioTipoMedio : IServicioRepositorioAsync<TipoMedio, string>,
        IServicioValorTextoAsync<TipoMedio>
    {

    }
}
