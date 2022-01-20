using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.AplicacionPlugin;
using RepositorioEntidades;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public class ContextoServicioAplicacion
    {
        protected IServicioCache cache;
        ILogger<ServicioLog> logger;

        protected DbContextAplicacion contexto;
        public ContextoServicioAplicacion(
            IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IServicioCache servicioCache)
        {
            try
            {
                DbContextAplicacionFactory cf = new DbContextAplicacionFactory(proveedorOpciones);
                this.contexto = cf.Crear();
                this.cache = servicioCache;
                this.logger = Logger;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }

    }
}
