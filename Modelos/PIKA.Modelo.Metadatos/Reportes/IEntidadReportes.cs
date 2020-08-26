using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public interface  IEntidadReportes
    {
        List<IProveedorReporte> Reportes { get; set; }
    }
}
