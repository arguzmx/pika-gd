using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Interfaces
{
    public interface IServicioValorListaPlantilla : IServicioRepositorioAsync<ValorListaPlantilla, string>, IServicioAutenticado<ValorListaPlantilla>
    {

    }
}
