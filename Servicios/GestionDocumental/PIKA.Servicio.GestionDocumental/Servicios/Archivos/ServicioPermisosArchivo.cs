using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using LazyCache;
using Newtonsoft.Json;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioPermisosArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioPermisosArchivo
    {
        private const string DEFAULT_SORT_COL = "Id";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private IRepositorioAsync<PermisosArchivo> repo;

        public ServicioPermisosArchivo(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
        {
            this.repo = UDT.ObtenerRepositoryAsync<PermisosArchivo>(new QueryComposer<PermisosArchivo>());
        }


        public async Task<bool> Existe(Expression<Func<PermisosArchivo, bool>> predicado)
        {
            List<PermisosArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<PermisosArchivo> CrearAsync(PermisosArchivo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            await seguridad.AccesoValidoPermisosArchivo(entity);

            var permiso = await UDT.Context.PermisosArchivo.Where(x => x.DestinatarioId == entity.DestinatarioId
                && x.ArchivoId == entity.ArchivoId).SingleOrDefaultAsync();

            if (permiso != null)
            {
                string original = JsonConvert.SerializeObject(permiso);
                permiso.ActualizarAcervo = entity.ActualizarAcervo;
                permiso.CrearAcervo = entity.CrearAcervo;
                permiso.ElminarAcervo = entity.ElminarAcervo;
                permiso.LeerAcervo = entity.LeerAcervo;
                permiso.CancelarTrasnferencia = entity.CancelarTrasnferencia;
                permiso.CrearTrasnferencia = entity.CrearTrasnferencia;
                permiso.EliminarTrasnferencia= entity.EliminarTrasnferencia;
                permiso.EnviarTrasnferencia = entity.EnviarTrasnferencia;
                permiso.RecibirTrasnferencia = entity.RecibirTrasnferencia;

                UDT.Context.Entry(permiso).State = EntityState.Modified;
                entity = permiso;
                await seguridad.RegistraEventoActualizar(permiso.Id, "*", original.JsonDiff(JsonConvert.SerializeObject(permiso)));

            }
            else
            {
                entity.Id = System.Guid.NewGuid().ToString();
                await this.repo.CrearAsync(entity);
                await seguridad.RegistraEventoCrear( entity.Id, "*");
            }


            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(PermisosArchivo entity)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            await seguridad.AccesoValidoPermisosArchivo(entity);

            PermisosArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            string original = JsonConvert.SerializeObject(o);

            o.ActualizarAcervo = entity.ActualizarAcervo;
            o.CrearAcervo = entity.CrearAcervo;
            o.ElminarAcervo = entity.ElminarAcervo;
            o.LeerAcervo = entity.LeerAcervo;
            o.CancelarTrasnferencia = entity.CancelarTrasnferencia;
            o.CrearTrasnferencia = entity.CrearTrasnferencia;
            o.EliminarTrasnferencia = entity.EliminarTrasnferencia;
            o.EnviarTrasnferencia = entity.EnviarTrasnferencia;
            o.RecibirTrasnferencia = entity.RecibirTrasnferencia;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar( o.Id, "*", original.JsonDiff(JsonConvert.SerializeObject(o)));
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

        private async Task VerificaFiltro(Consulta Query)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "ArchivoId");
            if (f == null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }
        }

        public async Task<IPaginado<PermisosArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PermisosArchivo>, IIncludableQueryable<PermisosArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            Query = GetDefaultQuery(Query);
            await VerificaFiltro(Query);

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            List<PermisosArchivo> listaEliminados = new List<PermisosArchivo>();
            foreach (var Id in ids)
            {
                PermisosArchivo c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                   await seguridad.AccesoValidoPermisosArchivo(c);     
                   listaEliminados.Add(c);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var c in listaEliminados)
                {
                    UDT.Context.Entry(c).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(c.Id, "*");
                }
                UDT.SaveChanges();
            }
            

            return listaEliminados.Select(x=>x.Id).ToList();

        }

        public Task<List<PermisosArchivo>> ObtenerAsync(string SqlCommand)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<PermisosArchivo>> ObtenerAsync(Expression<Func<PermisosArchivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<PermisosArchivo> UnicoAsync(Expression<Func<PermisosArchivo, bool>> predicado = null, Func<IQueryable<PermisosArchivo>, IOrderedQueryable<PermisosArchivo>> ordenarPor = null, Func<IQueryable<PermisosArchivo>, IIncludableQueryable<PermisosArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<PermisosArchivo>();
            PermisosArchivo a = await this.repo.UnicoAsync(predicado);
            await seguridad.AccesoValidoPermisosArchivo(a);
            return a.Copia();
        }

        #region Sin Implementación
        public Task<IPaginado<PermisosArchivo>> ObtenerPaginadoAsync(Expression<Func<PermisosArchivo, bool>> predicate = null, Func<IQueryable<PermisosArchivo>, IOrderedQueryable<PermisosArchivo>> orderBy = null, Func<IQueryable<PermisosArchivo>, IIncludableQueryable<PermisosArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<PermisosArchivo>> CrearAsync(params PermisosArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PermisosArchivo>> CrearAsync(IEnumerable<PermisosArchivo> entities, CancellationToken cancellationToken = default)
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

        public Task<PermisosArchivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
