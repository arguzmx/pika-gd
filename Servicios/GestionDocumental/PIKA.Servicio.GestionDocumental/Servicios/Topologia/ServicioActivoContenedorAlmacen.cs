using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.GestionDocumental.Interfaces.Topologia;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private ConfiguracionServidor configuracionServidor;
        public ServicioActivoContenedorAlmacen(
            IRegistroAuditoria registroAuditoria,
            IOptions<ConfiguracionServidor> options,
        IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
        ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger)
        {
            this.configuracionServidor = options.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ActivoContenedorAlmacen>(
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
            if (!(await Existe(x => x.ContenedorAlmacenId == entity.ContenedorAlmacenId && x.ActivoId == entity.ActivoId)))
            {
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            }
            await VinculaReferenciaActivo(new List<string>() { entity.ActivoId }, entity.ContenedorAlmacenId);
            return entity;
        }

        public async Task<ICollection<string>> Eliminar(string ContenedorAlmacenId, string[] ids)
        {
            try
            {
                ActivoContenedorAlmacen r;
                ICollection<string> listaEliminados = new HashSet<string>();
                foreach (var Id in ids)
                {


                    r = await this.repo.UnicoAsync(x => x.ContenedorAlmacenId == ContenedorAlmacenId && x.ActivoId== Id);

                    if (r != null)
                    {
                        UDT.Context.Entry(r).State = EntityState.Deleted;
                        listaEliminados.Add(r.ActivoId);
                    }
                }
                UDT.SaveChanges();

                foreach(var id in listaEliminados)
                {
                    await EliminaReferenciaActivo(id);
                }

                return listaEliminados;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

  
        /// <summary>
        ///  Añade una lista de ids de usuarios al rol
        /// </summary>
        /// <param name="rolid"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<int> PostIds(string ContenedorAlmacenId, string[] ids)
        {
            int total= 0;
            foreach(string id in ids)
            {
                if(!(await Existe(x => x.ActivoId == id && x.ContenedorAlmacenId == ContenedorAlmacenId)))
                {
                    await this.repo.CrearAsync(new ActivoContenedorAlmacen() { ActivoId = id, ContenedorAlmacenId  = ContenedorAlmacenId });
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
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

        public async Task<ICollection<string>> DeleteIds(string ContenedorAlmacenId, string[] ids)
        {
            List<string> l = new List<string>();
           foreach(string id in ids)
            {
                ActivoContenedorAlmacen ur = await repo.UnicoAsync(x => x.ContenedorAlmacenId == ContenedorAlmacenId && x.ActivoId == id);
                if (ur!=null)
                {
                    this.UDT.Context.Entry(ur).State = EntityState.Deleted;
                    l.Add(id);
                }
            }
            this.UDT.SaveChanges();
            foreach (var id in l)
            {
                await EliminaReferenciaActivo(id);
            }
            return l;
        }



        public async Task<List<string>> IdentificadoresRolesUsuario(string uid)
        {

            var roles = await repo.ObtenerAsync(x =>
            x.ActivoId.Equals(uid, StringComparison.InvariantCultureIgnoreCase));
            return roles.ToList().Select(x=> x.ContenedorAlmacenId ).ToList();
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
        #endregion

    }
}
