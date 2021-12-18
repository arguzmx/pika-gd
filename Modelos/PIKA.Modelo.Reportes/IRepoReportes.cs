using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Reportes
{
    public interface IRepoReportes
    {
        List<ReporteEntidad> ObtieneReportes();
    }
}
