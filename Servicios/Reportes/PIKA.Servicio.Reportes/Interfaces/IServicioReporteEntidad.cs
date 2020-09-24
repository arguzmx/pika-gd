using PIKA.Modelo.Reportes;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Reportes.Interfaces
{
  public  interface IServicioReporteEntidad : IServicioRepositorioAsync<ReporteEntidad, string>
    {
    }
}
