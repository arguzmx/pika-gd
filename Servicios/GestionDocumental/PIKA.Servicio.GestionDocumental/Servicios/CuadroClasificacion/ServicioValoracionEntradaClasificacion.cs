using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
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
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioValoracionEntradaClasificacion : ContextoServicioGestionDocumental,
         IServicioInyectable, IServicioValoracionEntradaClasificacion
    {
        private const string DEFAULT_SORT_COL = "EntradaClasificacionId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ValoracionEntradaClasificacion> repo;
        private IRepositorioAsync<EntradaClasificacion> repoEC;
        private IRepositorioAsync<TipoValoracionDocumental> repoTV;

        public ServicioValoracionEntradaClasificacion(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger,
           IOptions<ConfiguracionServidor> Config
           ) : base(registroAuditoria, proveedorOpciones, Logger,
               cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF)
        {
            UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            repo = UDT.ObtenerRepositoryAsync(new QueryComposer<ValoracionEntradaClasificacion>());
            repoEC = UDT.ObtenerRepositoryAsync(new QueryComposer<EntradaClasificacion>());
            repoTV = UDT.ObtenerRepositoryAsync(new QueryComposer<TipoValoracionDocumental>());
        }

        public async Task<bool> Existe(Expression<Func<ValoracionEntradaClasificacion, bool>> predicado)
        {
            List<ValoracionEntradaClasificacion> l = await repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<ValoracionEntradaClasificacion> CrearAsync(ValoracionEntradaClasificacion entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ValoracionEntradaClasificacion>();
            if (!await ExisteEntradaClasificacion(x => x.Id == entity.EntradaClasificacionId))
                throw new ExElementoExistente(entity.EntradaClasificacionId);

            if (!await ExisteTipoValoracionDocumental(x => x.Id == entity.TipoValoracionDocumentalId))
                throw new ExElementoExistente(entity.TipoValoracionDocumentalId);


            if (!await Existe(x => x.EntradaClasificacionId == entity.EntradaClasificacionId
           && x.TipoValoracionDocumentalId == entity.TipoValoracionDocumentalId))
            {
                entity.EntradaClasificacionId = entity.EntradaClasificacionId.Trim();
                entity.TipoValoracionDocumentalId = entity.TipoValoracionDocumentalId.Trim();
                await repo.CrearAsync(entity);
                entity.EntradaClasificacionId.Trim();
                UDT.SaveChanges();
            }
            return entity.Copia();
        }

        public async Task<bool> ExisteEntradaClasificacion(Expression<Func<EntradaClasificacion, bool>> predicadoelemento)
        {
            List<EntradaClasificacion> l = await repoEC.ObtenerAsync(predicadoelemento);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteTipoValoracionDocumental(Expression<Func<TipoValoracionDocumental, bool>> predicadoelemento)
        {
            List<TipoValoracionDocumental> l = await repoTV.ObtenerAsync(predicadoelemento);
            if (l.Count() == 0) return false;
            return true;
        }



        public async Task ActualizarAsync(ValoracionEntradaClasificacion entity)
        {
            if (!await ExisteEntradaClasificacion(x => x.Id == entity.EntradaClasificacionId))
                throw new ExElementoExistente(entity.EntradaClasificacionId);

            if (!await ExisteTipoValoracionDocumental(x => x.Id == entity.TipoValoracionDocumentalId))
                throw new ExElementoExistente(entity.TipoValoracionDocumentalId);

            //if (!await Existe(x => x.EntradaClasificacionId == entity.EntradaClasificacionId ))
            //    throw new EXNoEncontrado(entity.EntradaClasificacionId);

            //if (!await Existe(x => x.TipoValoracionDocumentalId == entity.TipoValoracionDocumentalId))
            //    throw new EXNoEncontrado(entity.TipoValoracionDocumentalId);

            if (await ExisteEntradaClasificacion(x => x.Id == entity.EntradaClasificacionId && x.Eliminada == true))
                throw new ExElementoExistente(entity.EntradaClasificacionId);

            ValoracionEntradaClasificacion o = await repo.UnicoAsync(x => x.EntradaClasificacionId == entity.EntradaClasificacionId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.EntradaClasificacionId);
            }


            if (!await Existe(x => x.EntradaClasificacionId == entity.EntradaClasificacionId
             && x.TipoValoracionDocumentalId == entity.TipoValoracionDocumentalId))
            {
                o.EntradaClasificacionId = entity.EntradaClasificacionId.Trim();
                o.TipoValoracionDocumentalId = entity.TipoValoracionDocumentalId.Trim();

                UDT.Context.Entry(o).State = EntityState.Modified;
            }
            UDT.SaveChanges();

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
        public async Task<IPaginado<ValoracionEntradaClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ValoracionEntradaClasificacion>, IIncludableQueryable<ValoracionEntradaClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ValoracionEntradaClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            List<string> Ids1 = new List<string>();
            List<string> Ids2 = new List<string>();
            string idRemplazado = "";
            foreach (var Id in ids)
            {
                if (Id.Contains('|'))
                {
                    idRemplazado = Id.Replace('|', ' ');
                    Ids2.Add(idRemplazado.Trim());
                }
                else
                {
                    idRemplazado = Id.Trim();
                    Ids1.Add(idRemplazado);
                }

            }
            if (Ids1.Count() > 0 && Ids2.Count() > 0)
            {
                foreach (var id1 in Ids1)
                {
                    foreach (var id2 in Ids2)
                    {
                        c = await repo.UnicoAsync(x => x.EntradaClasificacionId == id1
                        && x.TipoValoracionDocumentalId == id2);
                        if (c != null)
                        {
                            UDT.Context.Entry(c).State = EntityState.Deleted;
                            listaEliminados.Add(c.EntradaClasificacionId + " / " + c.TipoValoracionDocumentalId);
                        }
                    }

                }

            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<ICollection<string>> EliminarEntradas(string[] ids)
        {
            ValoracionEntradaClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await repo.UnicoAsync(x => x.EntradaClasificacionId == Id);
                if (c != null)
                {
                    await repo.Eliminar(c);
                    listaEliminados.Add(c.EntradaClasificacionId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public async Task<ValoracionEntradaClasificacion> UnicoAsync(Expression<Func<ValoracionEntradaClasificacion, bool>> predicado = null, Func<IQueryable<ValoracionEntradaClasificacion>, IOrderedQueryable<ValoracionEntradaClasificacion>> ordenarPor = null, Func<IQueryable<ValoracionEntradaClasificacion>, IIncludableQueryable<ValoracionEntradaClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ValoracionEntradaClasificacion c = await repo.UnicoAsync(predicado);
            return c.Copia();
        }
        public Task<List<ValoracionEntradaClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return repo.ObtenerAsync(SqlCommand);

        }

        #region No implmentados



        public Task<IEnumerable<ValoracionEntradaClasificacion>> CrearAsync(params ValoracionEntradaClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValoracionEntradaClasificacion>> CrearAsync(IEnumerable<ValoracionEntradaClasificacion> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<List<ValoracionEntradaClasificacion>> ObtenerAsync(Expression<Func<ValoracionEntradaClasificacion, bool>> predicado)
        {
            throw new NotImplementedException();
        }



        public Task<IPaginado<ValoracionEntradaClasificacion>> ObtenerPaginadoAsync(Expression<Func<ValoracionEntradaClasificacion, bool>> predicate = null, Func<IQueryable<ValoracionEntradaClasificacion>, IOrderedQueryable<ValoracionEntradaClasificacion>> orderBy = null, Func<IQueryable<ValoracionEntradaClasificacion>, IIncludableQueryable<ValoracionEntradaClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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

