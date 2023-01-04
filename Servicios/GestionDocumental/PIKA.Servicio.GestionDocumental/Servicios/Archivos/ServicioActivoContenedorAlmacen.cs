using DocumentFormat.OpenXml.Vml.Office;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces.Topologia;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioActivoContenedorAlmacen : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivoContenedorAlmacen
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ActivoContenedorAlmacen> repo;
        private ConfiguracionServidor configuracionServidor;
        public ServicioActivoContenedorAlmacen(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IOptions<ConfiguracionServidor> options,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger, 
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
        {
            this.configuracionServidor = options.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync(
                new QueryComposer<ActivoContenedorAlmacen>());
        }


        private async Task EliminaReferenciaActivo(string ActivoId)
        {
            var a = await UDT.Context.Activos.FindAsync(ActivoId);
            if (a != null)
            {
                
                a.ContenedorAlmacenId = null;
                if (configuracionServidor.activo_desasociar_total.HasValue &&
                    configuracionServidor.activo_desasociar_total.Value)
                {
                    a.AlmacenArchivoId = null;
                    a.ZonaAlmacenId = null;
                }
                a.UbicacionCaja = "";
                a.UbicacionRack = "";
                await UDT.Context.SaveChangesAsync();
            }
        }

        private async Task VinculaReferenciaActivo(List<string> ActivoId, string ContenedorId )
        {
            ContenedorAlmacen contenedor = await UDT.Context.ContenedoresAlmacen.FindAsync(ContenedorId);
            if (contenedor != null)
            {
                foreach (var id in ActivoId)
                {
                    var a = await UDT.Context.Activos.FindAsync(id);
                    if (a != null)
                    {
                        a.ContenedorAlmacenId = contenedor.Id;
                        a.ZonaAlmacenId = contenedor.ZonaAlmacenId;
                        a.AlmacenArchivoId = contenedor.AlmacenArchivoId;
                        a.UbicacionCaja = "";
                        a.UbicacionRack = "";
                        await UDT.Context.SaveChangesAsync();
                    }
                }
            }            
        }

        public async Task<bool> Existe(Expression<Func<ActivoContenedorAlmacen, bool>> predicado)
        {
            List<ActivoContenedorAlmacen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<ActivoContenedorAlmacen> CrearAsync(ActivoContenedorAlmacen entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ActivoContenedorAlmacen>();

            var Activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == entity.ActivoId);
            var Contenedor = UDT.Context.ContenedoresAlmacen.FirstOrDefault(x => x.Id == entity.ContenedorAlmacenId);

            if(Activo==null || Contenedor == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.AccesoValidoContenedorAlmacen(Contenedor);
            await seguridad.AccesoValidoActivo(Activo);

            if (!(await Existe(x => x.ContenedorAlmacenId == entity.ContenedorAlmacenId && x.ActivoId == entity.ActivoId)))
            {
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            }
            await VinculaReferenciaActivo(new List<string>() { entity.ActivoId }, entity.ContenedorAlmacenId);

            await seguridad.RegistraEventoCrear(entity.ActivoId, Activo.Nombre);

            return entity;
        }

        public async Task<ICollection<string>> Eliminar(string ContenedorAlmacenId, string[] ids)
        {
            seguridad.EstableceDatosProceso<ActivoContenedorAlmacen>();

            List<ActivoContenedorAlmacen> listaEliminados = new List<ActivoContenedorAlmacen>();
            var Contenedor = UDT.Context.ContenedoresAlmacen.FirstOrDefault(x => x.Id == ContenedorAlmacenId);
            await seguridad.AccesoValidoContenedorAlmacen(Contenedor);

            if (Contenedor == null)
            {
                throw new EXNoEncontrado();
            }


            foreach (var Id in ids)
            {
                ActivoContenedorAlmacen r = await this.repo.UnicoAsync(x => x.ContenedorAlmacenId == ContenedorAlmacenId && x.ActivoId == Id);

                if (r != null)
                {
                    var Activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == r.ActivoId);
                    if (Activo != null)
                    {
                        await seguridad.AccesoValidoActivo(Activo);
                        listaEliminados.Add(r);
                    }
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach (var r in listaEliminados)
                {
                    var Activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == r.ActivoId);
                    UDT.Context.Entry(r).State = EntityState.Deleted;
                    await EliminaReferenciaActivo(r.ActivoId);
                    await seguridad.RegistraEventoEliminar(r.ActivoId, Activo.Nombre);
                }

                UDT.SaveChanges();
            }


            return listaEliminados.Select(x => x.ActivoId).ToList();

        }


        /// <summary>
        ///  Añade una lista de ids de usuarios al rol
        /// </summary>
        /// <param name="rolid"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<int> PostIds(string ContenedorAlmacenId, string[] ids)
        {
            seguridad.EstableceDatosProceso<ActivoContenedorAlmacen>();

            var Contenedor = UDT.Context.ContenedoresAlmacen.FirstOrDefault(x => x.Id == ContenedorAlmacenId);
            await seguridad.AccesoValidoContenedorAlmacen(Contenedor);

            if (Contenedor == null)
            {
                throw new EXNoEncontrado();
            }

            int total = 0;
            foreach (string id in ids)
            {
                if (!(await Existe(x => x.ActivoId == id && x.ContenedorAlmacenId == ContenedorAlmacenId)))
                {
                    var Activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == id);
                    if (Activo != null)
                    {
                        var r = new ActivoContenedorAlmacen() { ActivoId = id, ContenedorAlmacenId = ContenedorAlmacenId };
                        await seguridad.AccesoValidoActivo(Activo);
                        await this.repo.CrearAsync(r);
                        await seguridad.RegistraEventoCrear(r.ActivoId, Activo.Nombre);
                    }
                }
            }

            await VinculaReferenciaActivo(ids.ToList(), ContenedorAlmacenId);
            UDT.SaveChanges();
            return total;
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
        public async Task<IPaginado<ActivoContenedorAlmacen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ActivoContenedorAlmacen>, IIncludableQueryable<ActivoContenedorAlmacen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ActivoContenedorAlmacen>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

        public async Task<ICollection<string>> DeleteIds(string ContenedorAlmacenId, string[] ids)
        {
            seguridad.EstableceDatosProceso<ActivoContenedorAlmacen>();

            var Contenedor = UDT.Context.ContenedoresAlmacen.FirstOrDefault(x => x.Id == ContenedorAlmacenId);
            await seguridad.AccesoValidoContenedorAlmacen(Contenedor);

            if (Contenedor == null)
            {
                throw new EXNoEncontrado();
            }

            List<string> l = new List<string>();
           foreach(string id in ids)
            {
                ActivoContenedorAlmacen ur = await repo.UnicoAsync(x => x.ContenedorAlmacenId == ContenedorAlmacenId && x.ActivoId == id);
                if (ur!=null)
                {
                    var Activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == id);
                    if (Activo != null)
                    {
                        await seguridad.AccesoValidoActivo(Activo);
                        this.UDT.Context.Entry(ur).State = EntityState.Deleted;
                        l.Add(id);
                        await seguridad.RegistraEventoEliminar(ur.ActivoId, Activo.Nombre);
                    }

                    
                }
            }
            this.UDT.SaveChanges();
            foreach (var id in l)
            {
                await EliminaReferenciaActivo(id);
            }
            return l;
        }



        #region sin implemetar

        public Task<List<ActivoContenedorAlmacen>> ObtenerAsync(Expression<Func<ActivoContenedorAlmacen, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task ActualizarAsync(ActivoContenedorAlmacen entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<ActivoContenedorAlmacen>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ActivoContenedorAlmacen>> ObtenerPaginadoAsync(Expression<Func<ActivoContenedorAlmacen, bool>> predicate = null, Func<IQueryable<ActivoContenedorAlmacen>, IOrderedQueryable<ActivoContenedorAlmacen>> orderBy = null, Func<IQueryable<ActivoContenedorAlmacen>, IIncludableQueryable<ActivoContenedorAlmacen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ActivoContenedorAlmacen> UnicoAsync(Expression<Func<ActivoContenedorAlmacen, bool>> predicado = null, Func<IQueryable<ActivoContenedorAlmacen>, IOrderedQueryable<ActivoContenedorAlmacen>> ordenarPor = null, Func<IQueryable<ActivoContenedorAlmacen>, IIncludableQueryable<ActivoContenedorAlmacen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivoContenedorAlmacen>> CrearAsync(params ActivoContenedorAlmacen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivoContenedorAlmacen>> CrearAsync(IEnumerable<ActivoContenedorAlmacen> entities, CancellationToken cancellationToken = default)
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

        public Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<ActivoContenedorAlmacen> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
