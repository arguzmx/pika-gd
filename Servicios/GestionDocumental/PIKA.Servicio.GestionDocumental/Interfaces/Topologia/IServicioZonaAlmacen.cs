using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioZonaAlmacen : IServicioRepositorioAsync<ZonaAlmacen, string>, IServicioValorTextoAsync<ZonaAlmacen>
    {
    }
}