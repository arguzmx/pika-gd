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
        protected ILogger logger;
        protected DbContextSeguridad contexto;
        public ContextoServicioSeguridad(
            IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
            ILogger Logger
            )
        {
            DbContextSeguridadFactory cf = new DbContextSeguridadFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
        }

    }
}
