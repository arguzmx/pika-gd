using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioSeguridadAplicaciones : ContextoServicioSeguridad,
        IServicioInyectable, IServicioSeguridadAplicaciones
    {
        private IRepositorioAsync<PermisoAplicacion> repo;

        public ServicioSeguridadAplicaciones(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
            ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL)
        {
            this.repo = UDT.ObtenerRepositoryAsync<PermisoAplicacion>(new QueryComposer<PermisoAplicacion>());
        }


        public async Task<ICollection<PermisoAplicacion>> ObtienePermisosAsync(string tipo, string id, string DominioId)
        {
            var lista = await repo.ObtenerAsync(x => x.EntidadAccesoId == id &&
             x.TipoEntidadAcceso == tipo && x.DominioId == DominioId);
            return lista;
        }


        public async Task<int> CrearActualizarAsync(params PermisoAplicacion[] entities)
        {

            int cantidad = 0;
            seguridad.EstableceDatosProceso<PermisoAplicacion>();

            foreach(string key in  entities.ToList().Select(x => x.EntidadAccesoId).Distinct())
            {
                string sqls = $"DELETE FROM {DbContextSeguridad.TablaPermisosApp} where EntidadAccesoId='{key}'";
                await this.UDT.Context.Database.ExecuteSqlRawAsync(sqls).ConfigureAwait(false);
            }

            List<PermisoAplicacion> adicionadas = new List<PermisoAplicacion>();
            foreach (var p in entities)
            {
                PermisoAplicacion permisoAdicionado = adicionadas.Where(x => x.DominioId == RegistroActividad.DominioId
                && x.AplicacionId == p.AplicacionId
                && x.ModuloId == p.ModuloId
                && x.TipoEntidadAcceso == p.TipoEntidadAcceso
                && x.EntidadAccesoId == p.EntidadAccesoId).SingleOrDefault();

                if (permisoAdicionado == null)
                {

                    p.DominioId = RegistroActividad.DominioId;
                    if (p.NegarAcceso)
                    {
                        p.Admin = false;
                        p.Ejecutar = false;
                        p.Eliminar = false;
                        p.Escribir = false;
                        p.Leer = false;
                    }

                    cantidad++;
                    var permiso = new PermisoAplicacion()
                    {
                        Admin = p.Admin,
                        AplicacionId = p.AplicacionId,
                        DominioId = p.DominioId,
                        Ejecutar = p.Ejecutar,
                        Eliminar = p.Eliminar,
                        EntidadAccesoId = p.EntidadAccesoId,
                        Escribir = p.Escribir,
                        Leer = p.Leer,
                        ModuloId = p.ModuloId,
                        NegarAcceso = p.NegarAcceso,
                        TipoEntidadAcceso = p.TipoEntidadAcceso
                    };
                    UDT.Context.PermisosAplicacion.Add(p);
                    UDT.Context.Entry(p).State = EntityState.Added;
                    UDT.SaveChanges();
                    adicionadas.Add(permiso);
                }

                string original = "{}";
                seguridad.IdEntidad = p.EntidadAccesoId;
                seguridad.NombreEntidad = p.ModuloId;
                await seguridad.RegistraEvento(AplicacionSeguridad.EventosAdicionales.CambioACL.GetHashCode(), true, original.JsonDiff(p.Flat()));
            }


            
            return cantidad;
        }

        public async Task<int> EliminarAsync(params PermisoAplicacion[] entities)
        {
            int cantidad = 0;

            foreach(var p in entities)
            {
                var entidad = await repo.UnicoAsync(x => x.DominioId == RegistroActividad.DominioId
                    && x.AplicacionId == p.AplicacionId
                    && x.ModuloId == p.ModuloId
                    && x.TipoEntidadAcceso == p.TipoEntidadAcceso
                    && x.EntidadAccesoId == p.EntidadAccesoId);

                if(entidad!=null)
                {
                    this.UDT.Context.Entry(entidad).State = EntityState.Deleted;
                    cantidad++;
                }

            }
            UDT.SaveChanges();
            return cantidad;
        }

        public Task<PermisoAplicacion> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new System.NotImplementedException();
        }
    }
}
