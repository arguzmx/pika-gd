using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using Newtonsoft.Json;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using System.Runtime.InteropServices;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioPermisosUnidadAdministrativaArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioPermisosUnidadAdministrativaArchivo
    {
        private const string DEFAULT_SORT_COL = "Id";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PermisosUnidadAdministrativaArchivo> repo;

        public ServicioPermisosUnidadAdministrativaArchivo(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
                cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<PermisosUnidadAdministrativaArchivo>());
        }


        public async Task<bool> Existe(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicado)
        {
            List<PermisosUnidadAdministrativaArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<PermisosUnidadAdministrativaArchivo> CrearAsync(PermisosUnidadAdministrativaArchivo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            await seguridad.AccesoCacheUnidadesAdministrativas(entity.UnidadAdministrativaArchivoId);

            var permiso = await UDT.Context.PermisosUnidadesAdministrativasArchivo.Where(x => x.DestinatarioId == entity.DestinatarioId
                && x.UnidadAdministrativaArchivoId == entity.UnidadAdministrativaArchivoId).SingleOrDefaultAsync();

            if (permiso != null)
            {
                string original = permiso.Flat();

                permiso.ActualizarAcervo = entity.ActualizarAcervo;
                permiso.CrearAcervo = entity.CrearAcervo;
                permiso.ElminarAcervo = entity.ElminarAcervo;
                permiso.LeerAcervo = entity.LeerAcervo;
                UDT.Context.Entry(permiso).State = EntityState.Modified;
                entity = permiso;
                await seguridad.RegistraEventoActualizar(entity.Id, "*", original.JsonDiff(entity.Flat()));
            }
            else
            {
                entity.Id = System.Guid.NewGuid().ToString();
                await this.repo.CrearAsync(entity);
                await seguridad.RegistraEventoCrear(entity.Id, "*");
            }

            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(PermisosUnidadAdministrativaArchivo entity)
        {
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            await seguridad.AccesoCacheUnidadesAdministrativas(entity.UnidadAdministrativaArchivoId);
            PermisosUnidadAdministrativaArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            await seguridad.AccesoCacheUnidadesAdministrativas(o.UnidadAdministrativaArchivoId);
            string original = o.Flat();
            o.ActualizarAcervo = entity.ActualizarAcervo;
            o.CrearAcervo = entity.CrearAcervo;
            o.ElminarAcervo = entity.ElminarAcervo;
            o.LeerAcervo = entity.LeerAcervo;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(entity.Id, "*", original.JsonDiff(o.Flat()));
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
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            Query = GetDefaultQuery(Query);

            var fUAId = Query.Filtros.FirstOrDefault(x => x.Propiedad == "UnidadAdministrativaArchivoId");
            if(fUAId == null)
            {
                throw new ExDatosNoValidos();
            }

            await seguridad.AccesoCacheUnidadesAdministrativas(fUAId.Valor);

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            List<PermisosUnidadAdministrativaArchivo> listaEliminados = new List<PermisosUnidadAdministrativaArchivo>();
            foreach (var Id in ids)
            {
                PermisosUnidadAdministrativaArchivo c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    await seguridad.AccesoCacheUnidadesAdministrativas(c.UnidadAdministrativaArchivoId);
                    listaEliminados.Add(c);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var c in listaEliminados)
                {
                    UDT.Context.Entry(c).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(c.Id, c.DestinatarioId);
                }
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x=>x.Id).ToList();

        }


        public Task<List<PermisosUnidadAdministrativaArchivo>> ObtenerAsync(string SqlCommand)
        {
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<PermisosUnidadAdministrativaArchivo>> ObtenerAsync(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicado)
        {
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<PermisosUnidadAdministrativaArchivo> UnicoAsync(Expression<Func<PermisosUnidadAdministrativaArchivo, bool>> predicado = null, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IOrderedQueryable<PermisosUnidadAdministrativaArchivo>> ordenarPor = null, Func<IQueryable<PermisosUnidadAdministrativaArchivo>, IIncludableQueryable<PermisosUnidadAdministrativaArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<PermisosUnidadAdministrativaArchivo>();
            PermisosUnidadAdministrativaArchivo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
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

        public Task<PermisosUnidadAdministrativaArchivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
