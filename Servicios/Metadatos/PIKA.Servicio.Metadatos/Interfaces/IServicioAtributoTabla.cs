using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Interfaces
{
    public interface IServicioAtributoTabla : IServicioRepositorioAsync<AtributoTabla, string>, IServicioAutenticado<AtributoTabla>
    {

    }
}
