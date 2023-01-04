using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioElementoClasificacion : IServicioRepositorioAsync<ElementoClasificacion, string>
        , IRepositorioJerarquia<ElementoClasificacion, string, string>, IServicioAutenticado<ElementoClasificacion>
    {
        Task<string[]> Purgar();

    }
}
