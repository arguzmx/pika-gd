using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioSeguridadAplicaciones: ContextoServicioSeguridad, IServicioInyectable, IServicioSeguridadAplicaciones
    {
        private IRepositorioAsync<PermisoAplicacion> repo;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;

        public ServicioSeguridadAplicaciones(
        IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
        ILogger<ServicioAplicacion> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<PermisoAplicacion>(new QueryComposer<PermisoAplicacion>());
        }


        public async Task<ICollection<PermisoAplicacion>> ObtienePermisosAsync(string tipo, string rolId)
        {
            return await repo.ObtenerAsync(x => x.EntidadAccesoId == rolId && 
            x.TipoEntidadAcceso == tipo);
        }

 

        public async Task<int> CrearActualizarAsync(params PermisoAplicacion[] entities)
        {
            int cantidad = 0;
            foreach(var tipo in  entities.ToList()
                .GroupBy(x => new { x.DominioId, x.AplicacionId })
                .Select(g => new { cantidad = g.Count(), key = g.Key }).ToList())
            {
                var lista = await repo.ObtenerAsync(x => x.DominioId == tipo.key.DominioId
                                && x.AplicacionId == tipo.key.AplicacionId);
                
                foreach (var p in entities.Where(x => x.DominioId == tipo.key.DominioId
                                && x.AplicacionId == tipo.key.AplicacionId))
                {
                    cantidad++;
                    PermisoAplicacion permisoExistente = lista.Where(x => x.DominioId == p.DominioId
                    && x.AplicacionId == p.AplicacionId
                    && x.ModuloId == p.ModuloId
                    && x.TipoEntidadAcceso == p.TipoEntidadAcceso
                    && x.EntidadAccesoId == p.EntidadAccesoId).SingleOrDefault();

                    if (permisoExistente==null)
                    {
                        if(p.Leer || p.Escribir || p.Ejecutar || p.Eliminar || p.Admin || p.NegarAcceso)
                        await repo.CrearAsync(p);
                    } else
                    {
                        if (p.NegarAcceso)
                        {
                            p.Admin = false;
                            p.Ejecutar = false;
                            p.Eliminar = false;
                            p.Escribir = false;
                            p.Leer = false;
                        }
                        permisoExistente.Admin = p.Admin;
                        permisoExistente.Ejecutar  = p.Ejecutar;
                        permisoExistente.Eliminar = p.Eliminar;
                        permisoExistente.Escribir  = p.Escribir;
                        permisoExistente.Leer  = p.Leer;
                        permisoExistente.NegarAcceso = p.NegarAcceso;
                        this.UDT.Context.Entry(permisoExistente).State = EntityState.Modified;
                    }
                }
            
                
            }

            UDT.SaveChanges();
            return cantidad;
        }

        public async Task<int> EliminarAsync(params PermisoAplicacion[] entities)
        {
            int cantidad = 0;

            foreach(var p in entities)
            {
                var entidad = await repo.UnicoAsync(x => x.DominioId == p.DominioId
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
        
    }
}
