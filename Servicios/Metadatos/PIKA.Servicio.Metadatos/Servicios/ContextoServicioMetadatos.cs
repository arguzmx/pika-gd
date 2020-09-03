using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Metadatos.Data;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Servicios
{
  public  class ContextoServicioMetadatos
    {
        protected ILogger logger;

        protected DbContextMetadatos contexto;
        public ContextoServicioMetadatos(
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger Logger)
        {
            DbContextMetadatosFactory cf = new DbContextMetadatosFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
        }
    }
}
