using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ContextoServicioContenido
    {
        protected IServicioCache cache;
        protected ILogger logger;

        protected DbContextContenido contexto;
        public ContextoServicioContenido(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger Logger,
            IServicioCache servicioCache)
        {
            DbContextContenidoFactory cf = new DbContextContenidoFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.cache = servicioCache;
            this.logger = Logger;
        }

    }
}
