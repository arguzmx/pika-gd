using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Menus;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public class ServicioSeguridadAplicaciones : ContextoServicioSeguridad,
        IServicioInyectable, IServicioSeguridadAplicaciones
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


        public async Task<ICollection<PermisoAplicacion>> ObtienePermisosAsync(string tipo, string id, string dominioid)
        {
            var lista = await repo.ObtenerAsync(x => x.EntidadAccesoId == id &&
             x.TipoEntidadAcceso == tipo && x.DominioId == dominioid);
            return lista;
        }


        public async Task<int> CrearActualizarAsync(string DominioId, params PermisoAplicacion[] entities)
        {

            int cantidad = 0;

            foreach(string key in  entities.ToList().Select(x => x.EntidadAccesoId).Distinct())
            {
                string sqls = $"DELETE FROM {DbContextSeguridad.TablaPermisosApp} where EntidadAccesoId='{key}'";
                await this.UDT.Context.Database.ExecuteSqlRawAsync(sqls).ConfigureAwait(false);
            }

            List<PermisoAplicacion> adicionadas = new List<PermisoAplicacion>();
            foreach (var p in entities)
            {
                PermisoAplicacion permisoAdicionado = adicionadas.Where(x => x.DominioId == DominioId
                && x.AplicacionId == p.AplicacionId
                && x.ModuloId == p.ModuloId
                && x.TipoEntidadAcceso == p.TipoEntidadAcceso
                && x.EntidadAccesoId == p.EntidadAccesoId).SingleOrDefault();

                if (permisoAdicionado == null)
                {

                    p.DominioId = DominioId;
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
                    await repo.CrearAsync(permiso);
                    adicionadas.Add(permiso);
                }
            }


            UDT.SaveChanges();
            return cantidad;
        }

        public async Task<int> EliminarAsync(string DominioId, params PermisoAplicacion[] entities)
        {
            int cantidad = 0;

            foreach(var p in entities)
            {
                var entidad = await repo.UnicoAsync(x => x.DominioId == DominioId
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
