using PIKA.Modelo.Reportes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Reportes
{
    public static partial class Extensiones
    {
        public static ReporteEntidad Copia(this ReporteEntidad a)
        {
            if (a == null) return null;
            return new ReporteEntidad()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                OrigenId = a.OrigenId,
                TipoOrigenId = a.TipoOrigenId,
                Descripcion = a.Descripcion,
                Entidad = a.Entidad,
                Plantilla = a.Plantilla,
                Bloqueado = a.Bloqueado,
                ExtensionSalida = a.ExtensionSalida,
                GrupoReportes = a.GrupoReportes,
                SubReporte = a.SubReporte,
            };
        }
    }
}