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
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones;
        protected DbContextContenido contexto;
        public ContextoServicioContenido(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger)
        {
            DbContextContenidoFactory cf = new DbContextContenidoFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
        }

    }
}
