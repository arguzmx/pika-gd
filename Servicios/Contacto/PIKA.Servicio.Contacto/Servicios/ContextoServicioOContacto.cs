using Microsoft.Extensions.Logging;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public class ContextoServicioOContacto
    {
        private IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones;
        protected ILogger logger;

        protected DbContextContacto contexto;
        public ContextoServicioOContacto(
            IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones,
            ILogger Logger)
        {
            DbContextContactoFactory cf = new DbContextContactoFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
        }

    }
}
