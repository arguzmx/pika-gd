using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestrctura.Reportes;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.Reportes.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental.Seguridad;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioPermisosUnidadAdministrativaArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioPermisosUnidadAdministrativaArchivo
    {
        private const string DEFAULT_SORT_COL = "Id";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PermisosUnidadAdministrativaArchivo> repo;
        private IOptions<ConfiguracionServidor> Config;
        private IServicioReporteEntidad ServicioReporteEntidad;

        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioPermisosUnidadAdministrativaArchivo(IRegistroAuditoria registroAuditoria,
            IServicioReporteEntidad ServicioReporteEntidad,
            IOptions<ConfiguracionServidor> Config,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger)
        {
            this.ServicioReporteEntidad = ServicioReporteEntidad;
            this.Config = Config;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<PermisosUnidadAdministrativaArchivo>(new QueryComposer<PermisosUnidadAdministrativaArchivo>());
        }


        public async Task<bool> Existe(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicado)
        {
            List<PermisosUnidadAdministrativaArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<PermisosUnidadAdministrativaArchivo> CrearAsync(PermisosUnidadAdministrativaArchivo entity, CancellationToken cancellationToken = default)
        {
            var permiso = await UDT.Context.PermisosUnidadesAdministrativasArchivo.Where(x => x.DestinatarioId == entity.DestinatarioId
      && x.UnidadAdministrativaArchivoId == entity.UnidadAdministrativaArchivoId).SingleOrDefaultAsync();

            if (permiso != null)
            {
                permiso.ActualizarAcervo = entity.ActualizarAcervo;
                permiso.CrearAcervo = entity.CrearAcervo;
                permiso.ElminarAcervo = entity.ElminarAcervo;
                permiso.LeerAcervo = entity.LeerAcervo;
                UDT.Context.Entry(permiso).State = EntityState.Modified;
                entity = permiso;

            }
            else
            {
                entity.Id = System.Guid.NewGuid().ToString();
                await this.repo.CrearAsync(entity);
            }


            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(PermisosUnidadAdministrativaArchivo entity)
        {
            try
            {
                PermisosUnidadAdministrativaArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.Id);
                }

                o.ActualizarAcervo = entity.ActualizarAcervo;
                o.CrearAcervo = entity.CrearAcervo;
                o.ElminarAcervo = entity.ElminarAcervo;
                o.LeerAcervo = entity.LeerAcervo;

                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

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

        public async Task<IPaginado<PermisosUnidadAdministrativaArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IIncludableQueryable<PermisosUnidadAdministrativaArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            PermisosUnidadAdministrativaArchivo c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    try
                    {
                        UDT.Context.Entry(c).State = EntityState.Deleted;
                        listaEliminados.Add(c.Id);
                    }
                    catch (Exception)
                    { }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }
        public Task<List<PermisosUnidadAdministrativaArchivo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<PermisosUnidadAdministrativaArchivo>> ObtenerAsync(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<PermisosUnidadAdministrativaArchivo> UnicoAsync(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicado = null, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IOrderedQueryable<PermisosUnidadAdministrativaArchivo>> ordenarPor = null, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IIncludableQueryable<PermisosUnidadAdministrativaArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            PermisosUnidadAdministrativaArchivo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

 

        private string[] ListaIdEliminar(string[]ids) 
        {
            return ids;
        }
        private async Task<ICollection<string>> EliminarArchivo(string[] ids)
        {
            PermisosUnidadAdministrativaArchivo o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                try
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id);
                    if (o != null)
                    {
                        await this.repo.Eliminar(o);
                        listaEliminados.Add(o.Id);
                    }
                    this.UDT.SaveChanges();

                }
                catch (DbUpdateException)
                {
                    throw new ExErrorRelacional(Id);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        #region Sin Implementación
        public Task<IPaginado<PermisosUnidadAdministrativaArchivo>> ObtenerPaginadoAsync(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicate = null, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IOrderedQueryable<PermisosUnidadAdministrativaArchivo>> orderBy = null, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IIncludableQueryable<PermisosUnidadAdministrativaArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<PermisosUnidadAdministrativaArchivo>> CrearAsync(params PermisosUnidadAdministrativaArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PermisosUnidadAdministrativaArchivo>> CrearAsync(IEnumerable<PermisosUnidadAdministrativaArchivo> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
