using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioElementoTransaccionCarga : ContextoServicioContenido,
        IServicioInyectable, IServicioElementoTransaccionCarga
    {


        private IRepositorioAsync<ElementoTransaccionCarga> repo;
        ConfiguracionServidor configuracionServidor;
        private UnidadDeTrabajo<DbContextContenido> UDT;
      
        public ServicioElementoTransaccionCarga(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IOptions<ConfiguracionServidor> opciones
        ) : base(proveedorOpciones, Logger)
        {
            this.configuracionServidor = opciones.Value;
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ElementoTransaccionCarga>(new QueryComposer<ElementoTransaccionCarga>());
        }


        private void validaEntidad(ElementoTransaccionCarga entity)
        {
            if (string.IsNullOrEmpty(entity.ElementoId))
                throw new ExDatosNoValidos($"Id elemento inválido");

            if (string.IsNullOrEmpty(entity.PuntoMontajeId))
                throw new ExDatosNoValidos($"Id punto de montaje inválido");

            if (string.IsNullOrEmpty(entity.TransaccionId))
                throw new ExDatosNoValidos($"Id transaccióm inválido");

            if (string.IsNullOrEmpty(entity.VolumenId))
                throw new ExDatosNoValidos($"Id volumen inválido");

            if (string.IsNullOrEmpty(entity.NombreOriginal))
                throw new ExDatosNoValidos($"Nombre origina inválido");

            if (entity.TamanoBytes<=0)
                throw new ExDatosNoValidos($"Tamaño inválido");

            if (entity.Indice <= 0)
                throw new ExDatosNoValidos($"Indice inválido");

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

        public async Task EliminarTransaccion(string TransaccionId)
        {
            List<ElementoTransaccionCarga> l = await repo.ObtenerAsync(x => x.TransaccionId == TransaccionId);
            await repo.EliminarRango(l);
            UDT.SaveChanges();
        }

        public async Task<List<ElementoTransaccionCarga>> OtieneElementosTransaccion(string TransaccionId)
        {
            List<ElementoTransaccionCarga> l = await repo.ObtenerAsync(x => x.TransaccionId == TransaccionId);
            return l.OrderBy(x => x.Indice).ToList();
        }

        public async Task ProcesaTransaccion(string TransaccionId, string VolumenId, IGestorES gestor)
        {
            List<ElementoTransaccionCarga> l = await OtieneElementosTransaccion(TransaccionId).ConfigureAwait(false);

            string ruta = Path.Combine(configuracionServidor.ruta_cache_fisico, TransaccionId);
            List<Parte> partes = new List<Parte>();

            l.ForEach(p => {
                string filePath = Path.Combine(ruta, p.Id + Path.GetExtension(p.NombreOriginal));
                if (System.IO.File.Exists(filePath))
                {
                    partes.Add(p.ConvierteParte());
                }

            });

            ComunesPartes hpartes = new ComunesPartes(this.UDT);
            List<Parte> resultados = await hpartes.CrearAsync(partes);

            foreach(Parte p in resultados)
            {
                string filePath = Path.Combine(ruta, p.Id + Path.GetExtension(p.NombreOriginal));
                await gestor.EscribeBytes(p.Id, p.ElementoId, p.VersionId, filePath, new FileInfo(p.NombreOriginal), false).ConfigureAwait(false);
            }

            await this.EliminarTransaccion(TransaccionId);
            try
            {
                Directory.Delete(ruta, true);
            }
            catch (Exception) { }

        }


        public async Task<string> ObtieneVolumenIdTransaccion(string TransaccionId)
        {
            ElementoTransaccionCarga l = await repo.UnicoAsync(x => x.TransaccionId == TransaccionId);
            return l?.VolumenId;
        }


    }

}