using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.Extensiones;
using PIKA.Modelo.Contenido.ui;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioVisor: ContextoServicioContenido,
        IServicioInyectable, IServicioVisor
    {
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private readonly IAppCache lazycache;
        private IOptions<ConfiguracionServidor> opciones;

        public ServicioVisor(
           IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
           ILogger<ServicioLog> Logger,
           IAppCache lazycache,
           IOptions<ConfiguracionServidor> opciones
       ) : base(proveedorOpciones, Logger)
        {
            this.opciones = opciones;
            this.lazycache = lazycache;
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
        }


        public async Task<Documento> ObtieneDocumento(string IdElemento)
        {
            ComunesElemento elementos = new ComunesElemento(UDT);
            Documento d = null;
            Elemento e = await elementos.ObtieneElemento(IdElemento);

            if (e != null)
            {
                d = new Documento() { Id = e.Id, Nombre = e.Nombre, VersionId = e.VersionId, Paginas = new List<Pagina>() };
            }
            return d;
        }
    }
}
