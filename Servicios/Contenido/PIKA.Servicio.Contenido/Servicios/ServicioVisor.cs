using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
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
            ComunesPartes partes = new ComunesPartes(UDT);
            Documento d = null;
            List<Parte> ps = null;
            Elemento e = await elementos.ObtieneElemento(IdElemento);

            if (e != null)
            {
                Modelo.Contenido.Version v = e.Versiones.Where(x => x.Activa == true).FirstOrDefault();
                if (v!=null)
                {
                    ps = await partes.ObtienePartesVersion(e.Id, v.Id);
                }

                d = new Documento() { Id = e.Id, Nombre = e.Nombre, Paginas = new List<Pagina>() };

                if (ps != null)
                {
                    ps.ForEach(p =>
                   {
                       d.Paginas.Add(p.APagina());
                   });
                }
                
            }
            return d;
        }
    }
}
