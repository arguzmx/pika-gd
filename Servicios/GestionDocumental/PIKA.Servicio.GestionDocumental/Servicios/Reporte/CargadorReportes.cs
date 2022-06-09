using PIKA.Modelo.Reportes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Servicios.Reporte
{

    /// <summary>
    /// DEfine los reportes del servicio para ser cargados vía reflexión
    /// </summary>
    public class CargadorReportes : IRepoReportes
    {

        private List<ReporteEntidad> GuiaSimpleArchivo()
        {
            return new List<ReporteEntidad>
            {
                new ReporteEntidad()
                {
                    Descripcion = "Reporte guía simple de archivo",
                    Entidad = "Archivo",
                    Id = "guiasimplearchivo",
                    Nombre = "Guía simple de archivo",
                    OrigenId = "*",
                    TipoOrigenId = "*",
                    Plantilla = "",
                    ExtensionSalida = ".docx",
                    Bloqueado = false,
                    GrupoReportes = "guiasimplearchivo",
                    SubReporte = false
                },


                new ReporteEntidad()
                {
                    Descripcion = "Subreporte guía simple unidades administrativas",
                    Entidad = "Archivo",
                    Id = "guiasimplearchivo-ua",
                    Nombre = "Subreporte unidades administrativas guía simple de archivo",
                    OrigenId = "*",
                    TipoOrigenId = "*",
                    Plantilla = "",
                    ExtensionSalida = ".docx",
                    Bloqueado = false,
                    GrupoReportes = "guiasimplearchivo",
                    SubReporte = false
                },
                new ReporteEntidad()
                {
                    Descripcion = "Subreporte guía simple secciones",
                    Entidad = "Archivo",
                    Id = "guiasimplearchivo-seccion",
                    Nombre = "Subreporte secciones guía simple de archivo",
                    OrigenId = "*",
                    TipoOrigenId = "*",
                    Plantilla = "",
                    ExtensionSalida = ".docx",
                    Bloqueado = false,
                    GrupoReportes = "guiasimplearchivo",
                    SubReporte = false
                }

            };

        }

        private ReporteEntidad CaratulaActivoAcervo()
        {
            return new ReporteEntidad()
            {
                Descripcion = "Reporte caratula acervo",
                Entidad = "Activo",
                Id = "caratulaactivo",
                Nombre = "Reporte caratula acervo",
                OrigenId = "*",
                TipoOrigenId = "*",
                Plantilla = "",
                ExtensionSalida = ".docx",
                Bloqueado = false,
                GrupoReportes = null,
                SubReporte = false
            };
        }

        private ReporteEntidad CaratulaActivoContenedorAlmacen()
        {
            return new ReporteEntidad()
            {
                Descripcion = "Reporte caratula de la caja",
                Entidad = "ContenedorAlmacen",
                Id = "caratula-cont-almacen",
                Nombre = "Reporte caratula de la caja",
                OrigenId = "*",
                TipoOrigenId = "*",
                Plantilla = "",
                ExtensionSalida = ".docx",
                Bloqueado = false,
                GrupoReportes = null,
                SubReporte = false
            };
        }


        private ReporteEntidad ReportePrestamo()
        {
            return new ReporteEntidad()
            {
                Descripcion = "Reporte préstamo",
                Entidad = "Prestamo",
                Id = "reporte-prestamo",
                Nombre = "Reporte préstamo",
                OrigenId = "*",
                TipoOrigenId = "*",
                Plantilla = "",
                ExtensionSalida = ".docx",
                Bloqueado = false,
                GrupoReportes = null,
                SubReporte = false
            };
        }

        public List<ReporteEntidad> ObtieneReportes()
        {
            List<ReporteEntidad> l = new List<ReporteEntidad>();
            l.Add(CaratulaActivoAcervo());
            l.Add(ReportePrestamo());
            l.Add(CaratulaActivoContenedorAlmacen());
            l.AddRange(GuiaSimpleArchivo());
            return l;
        }
    }
}
