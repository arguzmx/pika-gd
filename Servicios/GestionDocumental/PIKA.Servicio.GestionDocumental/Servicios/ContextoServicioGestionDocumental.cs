using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ContextoServicioGestionDocumental
    {
        protected ILogger<ServicioLog> logger;
        protected IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones;

        protected DBContextGestionDocumental contexto;
        public ContextoServicioGestionDocumental(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger)
        {
            DbContextGestionDocumentalFactory cf = new DbContextGestionDocumentalFactory(proveedorOpciones);
            this.contexto = cf.Crear();
            this.logger = Logger;
            this.proveedorOpciones = proveedorOpciones;
        }

    }
}

