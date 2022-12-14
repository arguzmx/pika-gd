using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
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

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioElementoClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioElementoClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IRepositorioAsync<ElementoClasificacion> repo;
        private IOCuadroClasificacion ioCuadroClasificacion;

        public ServicioElementoClasificacion(
            IOptions<ConfiguracionServidor> Config,
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger, cache,
                ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF)
        {
            this.ConfiguracionServidor = Config.Value;
            repo = UDT.ObtenerRepositoryAsync(new QueryComposer<ElementoClasificacion>());
            ioCuadroClasificacion = new IOCuadroClasificacion(this.registroAuditoria, logger, proveedorOpciones);
        }


        public async Task<bool> Existe(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            List<ElementoClasificacion> l = await repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<ElementoClasificacion> CrearAsync(ElementoClasificacion entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            await seguridad.PadreContieneId<CuadroClasificacion>(entity.CuadroClasifiacionId, OrigenEntidad.dominio);
            
            if (entity.EsRaiz)
            {
                entity.ElementoClasificacionId = null;
            }
            else
            {
                if (string.IsNullOrEmpty(entity.ElementoClasificacionId))
                {
                    throw new ExDatosNoValidos("ElementoClasificacionId");
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.ElementoClasificacionId))
                        if (await ValidaElementoCuadroClasificacion(entity.ElementoClasificacionId))
                            throw new ExDatosNoValidos(entity.ElementoClasificacionId);

                    entity.ElementoClasificacionId = entity.ElementoClasificacionId.Trim();
                }
            }

            if (await ValidaCuadroClasificacion(entity.CuadroClasifiacionId))
                throw new ExDatosNoValidos(entity.CuadroClasifiacionId);



            if (await Existe(x => x.Clave.Equals(entity.Clave.Trim(),
                StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
                && x.CuadroClasifiacionId == entity.CuadroClasifiacionId
                ))
            {
                throw new ExElementoExistente(entity.Clave);
            }

            entity.Id = Guid.NewGuid().ToString().Trim();
            entity.Clave = entity.Clave.Trim();
            entity.Nombre = entity.Nombre.Trim();
            entity.CuadroClasifiacionId = entity.CuadroClasifiacionId.Trim();


            await repo.CrearAsync(entity);
            UDT.SaveChanges();

            await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(entity.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);


            return entity.Copia();

        }


        private async Task<bool> ValidaCuadroClasificacion(string CuadroClasificacionID)
        {
            if (!string.IsNullOrEmpty(CuadroClasificacionID))
            {

                CuadroClasificacion C = await UDT.Context.CuadrosClasificacion.FirstOrDefaultAsync(x => x.Id == CuadroClasificacionID);
                if (C != null)
                {
                    if (C.Eliminada == true)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> ValidaElementoCuadroClasificacion(string ElemtoCuadroClasificacionID)
        {
            if (!string.IsNullOrEmpty(ElemtoCuadroClasificacionID))
            {
                ElementoClasificacion C = await repo.UnicoAsync(x => x.Id == ElemtoCuadroClasificacionID);
                if (C != null)
                {
                    if (C.Eliminada == true)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        public async Task<bool> Hijos(string ElementoClasificacionId, string idPadre)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            var cuadros = await seguridad.CacheIdEntidadPorDominio<CuadroClasificacion>();

            List<ElementoClasificacion> ListHijos = UDT.Context.ElementosClasificacion.Where(x =>
            cuadros.Contains(x.CuadroClasifiacionId) &&
            x.ElementoClasificacionId == ElementoClasificacionId).ToList();

            if (ListHijos.Count > 0)
            {
                foreach (var item in ListHijos)
                {
                    if (item.Eliminada)
                        return false;
                    if (item.ElementoClasificacionId == idPadre) { return false; }
                    if (!string.IsNullOrEmpty(item.ElementoClasificacionId) && item.Eliminada != true)
                    {
                        ElementoClasificacion e = await repo.UnicoAsync(x => x.Id == ElementoClasificacionId);
                        if (!string.IsNullOrEmpty(e.ElementoClasificacionId))
                        {

                            if (e.Id == ElementoClasificacionId)
                            {
                                return false;
                            }
                            else
                                return true;
                        }
                        else
                        {
                            await Hijos(e.ElementoClasificacionId, e.Id);
                            return true;
                        }
                    }
                    else
                        return true;
                }
                return false;
            }
            else
            {
                ElementoClasificacion e = await repo.UnicoAsync(x => cuadros.Contains(x.CuadroClasifiacionId) && x.Id == ElementoClasificacionId);
                if (e.Id == idPadre)
                    return false;
                if (e.Eliminada)
                    return false;
                if (!string.IsNullOrEmpty(e.ElementoClasificacionId))
                    return true;
                else
                    await Hijos(e.ElementoClasificacionId, e.Id);

                return false;

            }

        }


        public async Task ActualizarAsync(ElementoClasificacion entity)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            await seguridad.PadreContieneId<CuadroClasificacion>(entity.CuadroClasifiacionId, OrigenEntidad.dominio);
            
            ElementoClasificacion o = await repo.UnicoAsync(x => x.Id == entity.Id);
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            string original = System.Text.Json.JsonSerializer.Serialize(o.Copia());

            if (await Existe(x =>
            x.Id != entity.Id && x.Clave.Equals(entity.Clave.Trim(),
            StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
             && x.CuadroClasifiacionId == entity.CuadroClasifiacionId))
            {
                throw new ExElementoExistente(entity.Clave);
            }

            if (!string.IsNullOrEmpty(entity.ElementoClasificacionId))
                if (entity.Id == entity.ElementoClasificacionId) { throw new ExElementoExistente(entity.Id); }

            if (!string.IsNullOrEmpty(entity.CuadroClasifiacionId))
                if (await ValidaCuadroClasificacion(entity.CuadroClasifiacionId))
                {
                    throw new ExElementoExistente(entity.CuadroClasifiacionId);
                }

            if (!string.IsNullOrEmpty(entity.ElementoClasificacionId))
                if (await ValidaElementoCuadroClasificacion(entity.ElementoClasificacionId))
                    throw new ExElementoExistente(entity.ElementoClasificacionId);
            if (entity.Posicion < 0)
                throw new ExElementoExistente(entity.Posicion.ToString());

            o.Nombre = entity.Nombre.Trim();
            o.Eliminada = entity.Eliminada;
            o.Posicion = entity.Posicion;
            o.Clave = entity.Clave.Trim();
            if (!string.IsNullOrEmpty(entity.ElementoClasificacionId))
                o.ElementoClasificacionId = entity.ElementoClasificacionId.Trim();

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(entity.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(JsonConvert.SerializeObject(o.Copia())));
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
        public async Task<IPaginado<ElementoClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            var cuadros = await seguridad.CacheIdEntidadPorDominio<CuadroClasificacion>();
            Query = GetDefaultQuery(Query);

            var filtroC = Query.Filtros.FirstOrDefault(x => x.Propiedad == "CuadroClasifiacionId");
            if (filtroC == null || !cuadros.Contains(filtroC.Valor))
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            Query = GetDefaultQuery(Query);
            var respuesta = await repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            List<ElementoClasificacion> listaEliminados = new List<ElementoClasificacion>();
            foreach (var Id in ids)
            {
                ElementoClasificacion c = await repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    await seguridad.PadreContieneId<CuadroClasificacion>(c.CuadroClasifiacionId, OrigenEntidad.dominio);
                    listaEliminados.Add(c);
                }
            }

            foreach (var c in listaEliminados)
            {
                string original = System.Text.Json.JsonSerializer.Serialize(c);

                c.Nombre = await RestaurarNombre(c.Clave, c.CuadroClasifiacionId, c.Id, c.Nombre);
                c.Eliminada = false;
                UDT.Context.Entry(c).State = EntityState.Modified;
                await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(c.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
                await seguridad.RegistraEventoActualizar( c.Id, c.Nombre,  original.JsonDiff(JsonConvert.SerializeObject(c)));
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }
            return listaEliminados.Select(x => x.Id).ToList();
        }


        public async Task<List<ElementoClasificacion>> ObtenerHijosAsync(string PadreId, string JerquiaId)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            var cuadros = await seguridad.CacheIdEntidadPorDominio<CuadroClasificacion>() ;
            // Verifica que el dominio este en la lista de las del usuario
            if (cuadros.IndexOf(JerquiaId) < 0)
            {
                await seguridad.RegistraEvento((int)EventosComunesAuditables.Leer, false,null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            var l = await repo.ObtenerAsync(x => x.CuadroClasifiacionId == JerquiaId
            && x.ElementoClasificacionId == PadreId && x.Eliminada == false);

            return l.ToList().OrderBy(x => x.NombreJerarquico).ToList();
        }

        public async Task<List<ElementoClasificacion>> ObtenerRaicesAsync(string JerquiaId)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            var cuadros = await seguridad.CacheIdEntidadPorDominio<CuadroClasificacion>();
            if (cuadros.IndexOf(JerquiaId) < 0)
            {
                await seguridad.RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }


            var l = await repo.ObtenerAsync(x => x.CuadroClasifiacionId == JerquiaId
            && x.EsRaiz == true && x.Eliminada == false);

            return l.ToList().OrderBy(x => x.NombreJerarquico).ToList();

        }
        private async Task<string> RestaurarNombre(string Clave, string CuadroClasificacionId, string id, string Nombre)
        {

            if (await Existe(x =>
        x.Id != id && x.Clave.Equals(Clave,
        StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
         && x.CuadroClasifiacionId == CuadroClasificacionId))
            {
                Nombre = Nombre + $" {DateTime.Now.Ticks}";
            }

            return Nombre;

        }
        public async Task<ElementoClasificacion> UnicoAsync(Expression<Func<ElementoClasificacion, bool>> predicado = null, Func<IQueryable<ElementoClasificacion>, IOrderedQueryable<ElementoClasificacion>> ordenarPor = null, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ElementoClasificacion c = await repo.UnicoAsync(predicado);
            await seguridad.PadreContieneId<CuadroClasificacion>(c.CuadroClasifiacionId, OrigenEntidad.dominio);
            return c.Copia();
        }

        public Task<List<ElementoClasificacion>> ObtenerAsync(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            return repo.ObtenerAsync(predicado);
        }

        public Task<List<ElementoClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return repo.ObtenerAsync(SqlCommand);
        }


        public async Task<string[]> Purgar()
        {

            throw new NotImplementedException();
        }



        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();
            ICollection<string> listaEliminados = new HashSet<string>();

            foreach (var Id in ids)
            {
                bool done = await EliminarJerarquia(Id);
                if (done)
                {
                    listaEliminados.Add(Id);
                }
            }

            return listaEliminados;
        }



        private async Task<bool> EliminarJerarquia(string id)
        {
            seguridad.EstableceDatosProceso<ElementoClasificacion>();

            var entrada = UDT.Context.ElementosClasificacion.FirstOrDefault(x => x.Id == id);
            if (entrada == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.AccesoValidoElementoClasificacion(entrada);
            

            List<string> ids = new List<string>() { id };
            var r =  await ObtieneIdHijosRecursivo(id);
            if (r.Count > 0) ids.AddRange(r);


            // Marca los elementos y entradas hijos como elomiminados
            ids.ForEach(id =>
            {
                string sqls = $"UPDATE {DBContextGestionDocumental.TablaEntradaClasificacion} SET Eliminada=1  WHERE ElementoClasificacionId ='{id}'";
                UDT.Context.Database.ExecuteSqlRaw(sqls);

                sqls = $"UPDATE {DBContextGestionDocumental.TablaElementosClasificacion} SET Eliminada=1 WHERE Id ='{id}'"; 
                UDT.Context.Database.ExecuteSqlRaw(sqls);
            });


            return true;
        }




        private async Task<List<string>> ObtieneIdHijosRecursivo(string id)
        {
            var cuadros = await seguridad.CacheIdEntidadPorDominio<CuadroClasificacion>();

            List<string> ids = new List<string>();
            var hijos = UDT.Context.ElementosClasificacion.Where(x => cuadros.Contains(x.CuadroClasifiacionId) && x.ElementoClasificacionId == id)
                .Select(e => e.Id).ToList();

            ids.AddRange(hijos);
           
            foreach (var h in hijos)
            {
                var r = await ObtieneIdHijosRecursivo(h);
                if (r.Count > 0)
                {
                    ids.AddRange(r);
                }
            }
            return ids;
        }


        #region Sin implementar
        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ElementoClasificacion>> CrearAsync(params ElementoClasificacion[] entities)
        {
            throw new NotImplementedException();
        }
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ElementoClasificacion>> ObtenerPaginadoAsync(Expression<Func<ElementoClasificacion, bool>> predicate = null, Func<IQueryable<ElementoClasificacion>, IOrderedQueryable<ElementoClasificacion>> orderBy = null, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ElementoClasificacion>> CrearAsync(IEnumerable<ElementoClasificacion> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ElementoClasificacion> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
