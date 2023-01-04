using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioElementoTransaccionCarga : ContextoServicioContenido,
        IServicioInyectable, IServicioElementoTransaccionCarga
    {
        private IRepositorioAsync<ElementoTransaccionCarga> repo;

        public ServicioElementoTransaccionCarga(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ElementoTransaccionCarga>(new QueryComposer<ElementoTransaccionCarga>());
        }


        public async Task<ElementoTransaccionCarga> CrearAsync(ElementoTransaccionCarga entity, CancellationToken cancellationToken = default)
        {
            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCarga = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ProcesoElemento(string ElementoId, bool Error, string Motivo)
        {
            ElementoTransaccionCarga el = await repo.UnicoAsync(x => x.Id == ElementoId);
            if (el != null)
            {
                el.Procesado = true;
                el.Error = Error;
                if (Error)
                {
                    el.Info = Motivo;
                }

                UDT.Context.Entry(el).State = EntityState.Modified;
                UDT.SaveChanges();
            } else
            {
                throw new EXNoEncontrado(ElementoId);
            }
        }

        public async Task EliminarTransaccion(string TransaccionId,string VolId, long CuentaBytes)
        {

            ComunesPartes hpartes = new ComunesPartes(this.UDT);
            await hpartes.ActualizaTamanoVolumen(VolId, CuentaBytes, true);
            List<ElementoTransaccionCarga> l = await repo.ObtenerAsync(x => x.TransaccionId == TransaccionId);
            await repo.EliminarRango(l);
            UDT.SaveChanges();
        }

        public async Task<List<ElementoTransaccionCarga>> OtieneElementosTransaccion(string TransaccionId)
        {
            List<ElementoTransaccionCarga> l = await repo.ObtenerAsync(x => x.TransaccionId == TransaccionId);
            return l.OrderBy(x => x.Indice).ToList();
        }

        public Task<ElementoTransaccionCarga> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
    }

}