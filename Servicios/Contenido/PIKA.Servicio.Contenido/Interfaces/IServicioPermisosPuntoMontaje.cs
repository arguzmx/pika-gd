using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioPermisosPuntoMontaje: IServicioRepositorioAsync<PermisosPuntoMontaje, string>, IServicioAutenticado<PermisosPuntoMontaje>
    {
    }
}
