using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion
{

    public class ContextoServicioOrganizacion
    {
        private IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones;
        protected ILogger logger;

        protected DbContextOrganizacion contexto;
        public ContextoServicioOrganizacion(
            IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
            ILogger Logger)
        {
            DbContextOrganizacionFactory cf = new DbContextOrganizacionFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
        }

    }
}
