using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.AplicacionPlugin;
using RepositorioEntidades;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public class ContextoServicioAplicacion
    {
        protected IServicioCache cache;
        protected ILogger logger;

        protected DbContextAplicacionPlugin contexto;
        public ContextoServicioAplicacion(
            IProveedorOpcionesContexto<DbContextAplicacionPlugin> proveedorOpciones,
            ILogger Logger,
            IServicioCache servicioCache)
        {
            DbContextAplicacionPluginFactory cf = new DbContextAplicacionPluginFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.cache = servicioCache;
            this.logger = Logger;
        }

    }
}
