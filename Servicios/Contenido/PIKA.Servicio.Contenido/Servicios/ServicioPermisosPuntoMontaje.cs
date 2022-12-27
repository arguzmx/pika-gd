using Bogus.Extensions;
using ImageMagick;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using MySqlConnector.Logging;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioPermisosPuntoMontaje : ContextoServicioContenido,
        IServicioInyectable, IServicioPermisosPuntoMontaje
    {

        private const string DEFAULT_SORT_COL = "DestinatarioId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PermisosPuntoMontaje> repo;
        
        public ServicioPermisosPuntoMontaje(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION)
        {
            this.repo = UDT.ObtenerRepositoryAsync<PermisosPuntoMontaje>(new QueryComposer<PermisosPuntoMontaje>());
        }

        public async Task<bool> Existe(Expression<Func<PermisosPuntoMontaje, bool>> predicado)
        {
            List<PermisosPuntoMontaje> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<PermisosPuntoMontaje> CrearAsync(PermisosPuntoMontaje entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PermisosPuntoMontaje>();
            var p = await UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == entity.PuntoMontajeId);
            if(p == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.IdEnUnidadOrg(p.OrigenId);

            var pm = await this.UDT.Context.PermisosPuntoMontaje.FirstOrDefaultAsync(x => x.PuntoMontajeId == entity.PuntoMontajeId && x.DestinatarioId == entity.DestinatarioId);
            if (pm != null)
            {
                string original = p.Flat();
                entity.Id = pm.Id;
                UDT.Context.Entry(entity).State = EntityState.Modified;
                await seguridad.RegistraEventoActualizar(entity.Id, p.Nombre, original.JsonDiff(entity.Flat()));
            }
            else
            {
                entity.Id = Guid.NewGuid().ToString();
                await this.repo.CrearAsync(entity);
                await seguridad.RegistraEventoCrear(entity.Id, p.Nombre);
            }

            UDT.SaveChanges();

           

            return entity.Copia();

        }


        public async Task ActualizarAsync(PermisosPuntoMontaje entity)
        {
            seguridad.EstableceDatosProceso<PermisosPuntoMontaje>();
            var p = await UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == entity.PuntoMontajeId);
            if (p == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.IdEnUnidadOrg(p.OrigenId);

            var pm = await UDT.Context.PermisosPuntoMontaje.FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (pm != null)
            {
                string original = p.Flat();
                UDT.Context.Entry(pm).State = EntityState.Modified;

                pm.Crear = entity.Crear;
                pm.Actualizar = entity.Actualizar;
                pm.Elminar = entity.Elminar;
                pm.GestionContenido = entity.GestionContenido;
                pm.GestionMetadatos = entity.GestionMetadatos;
                pm.Leer = entity.Leer;
                pm.PuntoMontajeId = entity.PuntoMontajeId;


                try
                {
                    UDT.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }

                

                await seguridad.RegistraEventoActualizar(entity.Id, p.Nombre, original.JsonDiff(entity.Flat()));
            }
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<PermisosPuntoMontaje>();
            List<PermisosPuntoMontaje> listaEliminados = new List<PermisosPuntoMontaje>();
            foreach (var Id in ids.LimpiaIds())
            {
                PermisosPuntoMontaje d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    var p = await UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == d.PuntoMontajeId);
                    if (p == null)
                    {
                        throw new EXNoEncontrado();
                    }

                    await seguridad.IdEnUnidadOrg(p.OrigenId);
                    listaEliminados.Add(d);
                }
            }
            if (listaEliminados.Count > 0)
            {
                foreach(var o in listaEliminados)
                {
                    this.UDT.Context.Entry(o).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(o.Id, o.DestinatarioId);
                }
                UDT.SaveChanges();
            }
            return listaEliminados.Select(x=>x.Id).ToList();
        }



        private Consulta GetDefaultQuery(Consulta query)
        {
            if (query != null)
            {
                query.indice = query.indice < 0 ? 0 : query.indice;
                query.tamano = query.tamano <= 0 ? 20 : query.tamano;
                query.ord_columna = string.IsNullOrEmpty(query.ord_columna) ? DEFAULT_SORT_COL : query.ord_columna;
                query.ord_direccion = string.IsNullOrEmpty(query.ord_direccion) ? DEFAULT_SORT_DIRECTION : query.ord_direccion;
            }
            else
            {
                query = new Consulta() { indice = 0, tamano = 20, ord_columna = DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }

        public async Task<IPaginado<PermisosPuntoMontaje>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PermisosPuntoMontaje>, IIncludableQueryable<PermisosPuntoMontaje, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {

            seguridad.EstableceDatosProceso<PermisosPuntoMontaje>();
            Query = GetDefaultQuery(Query);

            if(!Query.Filtros.Any(f=>f.Propiedad == "PuntoMontajeId"))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }
        public async  Task<PermisosPuntoMontaje> UnicoAsync(Expression<Func<PermisosPuntoMontaje, bool>> predicado = null, Func<IQueryable<PermisosPuntoMontaje>, IOrderedQueryable<PermisosPuntoMontaje>> ordenarPor = null, Func<IQueryable<PermisosPuntoMontaje>, IIncludableQueryable<PermisosPuntoMontaje, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            PermisosPuntoMontaje d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);
            var p = await UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == d.PuntoMontajeId);
            if (p == null)
            {
                throw new EXNoEncontrado();
            }
            await seguridad.IdEnUnidadOrg(p.OrigenId);
            return d.Copia();
        }

        #region NO Implementados


        public Task<IEnumerable<PermisosPuntoMontaje>> CrearAsync(params PermisosPuntoMontaje[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PermisosPuntoMontaje>> CrearAsync(IEnumerable<PermisosPuntoMontaje> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }




        public Task<List<PermisosPuntoMontaje>> ObtenerAsync(Expression<Func<PermisosPuntoMontaje, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<PermisosPuntoMontaje>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<PermisosPuntoMontaje>> ObtenerPaginadoAsync(Expression<Func<PermisosPuntoMontaje, bool>> predicate = null, Func<IQueryable<PermisosPuntoMontaje>, IOrderedQueryable<PermisosPuntoMontaje>> orderBy = null, Func<IQueryable<PermisosPuntoMontaje>, IIncludableQueryable<PermisosPuntoMontaje, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<PermisosPuntoMontaje> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}

