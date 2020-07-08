using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Servicio.Organizacion;
using PIKA.Servicio.Seguridad;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Usuarios
{

    public class ContextoServicioUsuarios
    {
        protected ILogger logger;
        protected DbContextSeguridad contextoSeguridad;
        protected DbContextOrganizacion contextoOrganizacion;
        public ContextoServicioUsuarios(
            IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpcionesOrg,
            IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
            ILogger Logger
            )
        {
            DbContextSeguridadFactory cf = new DbContextSeguridadFactory(proveedorOpciones);
            this.contextoSeguridad = cf.Crear();

            DbContextOrganizacionFactory cforg = new DbContextOrganizacionFactory(proveedorOpcionesOrg);
            this.contextoOrganizacion = cforg.Crear();

            this.logger = Logger;
        }

    }
}
