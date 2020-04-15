using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{
   public class ContextoServicioSeguridad
    {
        private IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones;
        protected IServicioCache cache;
        protected ILogger logger;

        protected DbContextSeguridad contexto;
        public ContextoServicioSeguridad(
            IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
            ILogger Logger,
            IServicioCache servicioCache)
        {
            DbContextSeguridadFactory cf = new DbContextSeguridadFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.cache = servicioCache;
            this.logger = Logger;
        }
    }
}
