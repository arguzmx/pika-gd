using Microsoft.Extensions.Logging;
using PIKA.Servicio.Reportes.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Reportes.Servicios
{
   public class ContextoServicioRepoerteEntidad
    {
        protected ILogger logger;
        protected IProveedorOpcionesContexto<DbContextReportes> proveedorOpciones;

        protected DbContextReportes contexto;
        public ContextoServicioRepoerteEntidad(
            IProveedorOpcionesContexto<DbContextReportes> proveedorOpciones,
            ILogger Logger)
        {
            DbContextReportesFactory cf = new DbContextReportesFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
        }

    }
}
