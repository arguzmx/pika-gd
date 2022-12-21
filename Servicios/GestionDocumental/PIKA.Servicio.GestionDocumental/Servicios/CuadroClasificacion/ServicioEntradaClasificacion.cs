using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
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

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioEntradaClasificacion : ContextoServicioGestionDocumental,
          IServicioInyectable, IServicioEntradaClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EntradaClasificacion> repo;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoEC;
        private IRepositorioAsync<ElementoClasificacion> repoEL;
        private IRepositorioAsync<CuadroClasificacion> repoCC;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioEntradaClasificacion(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger, IOptions<ConfiguracionServidor> Config
           ) : base(registroAuditoria, proveedorOpciones, Logger , cache,
               ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF)
        {
            UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            repo = UDT.ObtenerRepositoryAsync(new QueryComposer<EntradaClasificacion>());
            repoEC = UDT.ObtenerRepositoryAsync(new QueryComposer<ValoracionEntradaClasificacion>());
            repoEL = UDT.ObtenerRepositoryAsync(new QueryComposer<ElementoClasificacion>());
            repoTD = UDT.ObtenerRepositoryAsync(new QueryComposer<TipoDisposicionDocumental>());
            repoCC = UDT.ObtenerRepositoryAsync(new QueryComposer<CuadroClasificacion>());
            this.cache = cache;
        }

        public async Task<bool> Existe(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            List<EntradaClasificacion> l = await repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<bool> ExisteCC(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            List<CuadroClasificacion> l = await repoCC.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<EntradaClasificacion> CrearAsync(EntradaClasificacion entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<EntradaClasificacion>();
            await seguridad.AccesoValidoEntradaClasificacion(entity);

            if (await Existelemento(x => x.Id == entity.ElementoClasificacionId && x.Eliminada == true))
                throw new EXNoEncontrado(entity.ElementoClasificacionId);

            if (!await Existelemento(x => x.Id == entity.ElementoClasificacionId))
                throw new EXNoEncontrado(entity.ElementoClasificacionId);

            //if (!string.IsNullOrEmpty(entity.TipoDisposicionDocumentalId))
            //    if (!await ExisteTipoDisposicionDocumental(x => x.Id == entity.TipoDisposicionDocumentalId))
            //        throw new ExErrorRelacional(entity.TipoDisposicionDocumentalId);

            if (!await ExisteCC(x => x.Id == entity.CuadroClasifiacionId))
                throw new ExErrorRelacional(entity.CuadroClasifiacionId);

            if (await Existe(x => x.Clave.Equals(entity.Clave.Trim(),
                StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
                && x.ElementoClasificacionId == entity.ElementoClasificacionId
                ))
            {
                throw new ExElementoExistente(entity.Clave.Trim());
            }

            entity.Id = Guid.NewGuid().ToString();
            entity.Nombre = entity.Nombre.Trim();
            entity.Clave = entity.Clave.Trim();
            entity.Descripcion = string.IsNullOrEmpty(entity.Descripcion) ? "" : entity.Descripcion.Trim();


            contexto.EntradaClasificacion.Add(entity);
            contexto.SaveChanges();

            if (entity.TipoValoracionDocumentalId != null
                && entity.TipoValoracionDocumentalId.Length > 0)
            {
                foreach (string id in entity.TipoValoracionDocumentalId)
                {
                    var tipo = UDT.Context.TipoValoracionDocumental.Where(x => x.Id.Equals(id.Trim(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefaultAsync();
                    if (tipo != null)
                    {
                        ValoracionEntradaClasificacion vec = new ValoracionEntradaClasificacion()
                        {
                            EntradaClasificacionId = entity.Id,
                            TipoValoracionDocumentalId = id.Trim()
                        };
                        contexto.ValoracionEntradaClasificacion.Add(vec);
                        //
                    }
                }
            }
            await Task.Delay(250);
            contexto.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }

        public async Task<bool> Existelemento(Expression<Func<ElementoClasificacion, bool>> predicadoelemento)
        {
            return await repoEL.UnicoAsync(predicadoelemento) == null ? false : true;
        }


        public async Task<bool> ExisteTipoDisposicionDocumental(Expression<Func<TipoDisposicionDocumental, bool>> predicadoelemento)
        {
            return await repoTD.UnicoAsync(predicadoelemento) == null ? false : true;
        }


        public async Task ActualizarAsync(EntradaClasificacion entity)
        {

                seguridad.EstableceDatosProceso<EntradaClasificacion>();
                await seguridad.AccesoValidoEntradaClasificacion(entity);

                EntradaClasificacion o = await repo.UnicoAsync(x => x.Id == entity.Id && x.Eliminada == false);

                string original = o.Flat();

                if (!await Existelemento(x => x.Id == entity.ElementoClasificacionId &&
                        x.Eliminada == false))
                    throw new EXNoEncontrado(entity.ElementoClasificacionId);

                //if (!(await ExisteTipoDisposicionDocumental(x => x.Id == entity.TipoDisposicionDocumentalId)))
                //        throw new ExErrorRelacional(entity.TipoDisposicionDocumentalId);

                if (await Existe(x => x.Id != entity.Id
                    && x.Clave.Equals(entity.Clave.Trim(), StringComparison.InvariantCultureIgnoreCase)
                    && x.Eliminada == false
                    && x.ElementoClasificacionId == entity.ElementoClasificacionId
                    ))
                {
                    throw new ExElementoExistente(entity.Clave);
                }

                o.Nombre = entity.Nombre.Trim();
                o.Clave = entity.Clave.Trim();
                o.DisposicionEntrada = entity.DisposicionEntrada;
                o.VigenciaConcentracion = entity.VigenciaConcentracion;
                o.VigenciaTramite = entity.VigenciaTramite;
                o.Posicion = entity.Posicion;
                o.TipoDisposicionDocumentalId = entity.TipoDisposicionDocumentalId;
                o.Descripcion = string.IsNullOrEmpty(entity.Descripcion) ? "" : entity.Descripcion.Trim();
                o.AbreCon = entity.AbreCon;
                o.CierraCon = entity.CierraCon;
                o.Contiene = entity.Contiene;
                o.InstruccionFinal = entity.InstruccionFinal;

                UDT.Context.Entry(o).State = EntityState.Modified;

                List<ValoracionEntradaClasificacion> lista = await repoEC.ObtenerAsync(x => x.EntradaClasificacionId.Equals(entity.Id.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (entity.TipoValoracionDocumentalId == null) entity.TipoValoracionDocumentalId = new string[0];

                // Añade las faltantes
                foreach (string id in entity.TipoValoracionDocumentalId)
                {

                    if (!lista.Where(x => x.TipoValoracionDocumentalId == id).Any())
                    {
                        ValoracionEntradaClasificacion vec = new ValoracionEntradaClasificacion()
                        {
                            EntradaClasificacionId = entity.Id,
                            TipoValoracionDocumentalId = id.Trim()
                        };
                        UDT.Context.ValoracionEntradaClasificacion.Add(vec);
                        UDT.Context.SaveChanges();
                    }
                }

                // remueve los sobrantes
                foreach (var item in lista)
                {
                    if (entity.TipoValoracionDocumentalId.IndexOf(item.TipoValoracionDocumentalId) < 0)
                    {
                        UDT.Context.Entry(item).State = EntityState.Deleted;
                    }
                }

                UDT.Context.SaveChanges();

                await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(o.Flat()));

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
        public async Task<bool> ValidaElementoCuadroClasificacion(string ElemtoCuadroClasificacionID)
        {
            if (!string.IsNullOrEmpty(ElemtoCuadroClasificacionID))
            {
                ElementoClasificacion C = await repoEL.UnicoAsync(x => x.Id == ElemtoCuadroClasificacionID);
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

        public async Task<IPaginado<EntradaClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<EntradaClasificacion>();
            var CuadrosUsuario = await seguridad.CacheIdEntidadPorDominio<CuadroClasificacion>();
            Query = GetDefaultQuery(Query);

            var filtroC = Query.Filtros.FirstOrDefault(x => x.Propiedad == "CuadroClasifiacionId");
            if (filtroC == null || !CuadrosUsuario.Contains(filtroC.Valor))
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var respuesta = await repo.ObtenerPaginadoAsync(Query, include);
            foreach (var item in respuesta.Elementos)
            {
                item.TipoValoracionDocumentalId = await ObtieneValoracionDocumental(item);
            }
            return respuesta;
        }

        private async Task<string[]> ObtieneValoracionDocumental(EntradaClasificacion item)
        {
            var l = await repoEC.ObtenerAsync(x => x.EntradaClasificacionId.Equals(item.Id, StringComparison.InvariantCultureIgnoreCase));
            item.TipoValoracionDocumentalId = new string[l.Count];
            for (int i = 0; i < l.Count; i++)
            {
                item.TipoValoracionDocumentalId[i] = l[i].TipoValoracionDocumentalId;
            }
            return item.TipoValoracionDocumentalId;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<EntradaClasificacion>();
            List<EntradaClasificacion> listaEliminados = new List<EntradaClasificacion>();
            foreach (var Id in ids)
            {
                EntradaClasificacion c = await repo.UnicoAsync(x => x.Id == Id).ConfigureAwait(false);
                if (c != null)
                {
                    await seguridad.AccesoValidoEntradaClasificacion(c);
                    listaEliminados.Add(c);
                }
            }

            foreach (var c in listaEliminados)
            {
                c.Eliminada = true;
                UDT.Context.Entry(c).State = EntityState.Modified;
                await seguridad.RegistraEventoEliminar(c.Id, c.Nombre);
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }
            return listaEliminados.Select(x => x.Id).ToList();
        }


        private async Task<string> RestaurarNombre(string Clave, string ElementoCuadroClasificacionId, string id, string Nombre)
        {

            if (await Existe(x =>
        x.Id != id && x.Clave.Equals(Clave, StringComparison.InvariantCultureIgnoreCase)
        && x.Eliminada == false
         && x.ElementoClasificacionId == ElementoCuadroClasificacionId))
            {

                Nombre = Nombre + $" {DateTime.Now.Ticks}";
            }
            else
            {
            }
            await Existelemento(x => x.Id == ElementoCuadroClasificacionId && x.Eliminada == true);


            return Nombre;

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<EntradaClasificacion>();
            List<EntradaClasificacion> listaEliminados = new List<EntradaClasificacion>();
            foreach (var Id in ids)
            {
                EntradaClasificacion c = await repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    await seguridad.AccesoValidoEntradaClasificacion(c);
                    listaEliminados.Add(c);
                }
            }

            foreach (var c in listaEliminados)
            {
                string original = c.Flat();
                c.Nombre = await RestaurarNombre(c.Clave, c.ElementoClasificacionId, c.Id, c.Nombre);
                c.Eliminada = false;
                UDT.Context.Entry(c).State = EntityState.Modified;

                await seguridad.RegistraEventoActualizar( c.Id, c.Nombre, original.JsonDiff(c.Flat()));
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x => x.Id).ToList();
        }



        public async Task<EntradaClasificacion> UnicoAsync(Expression<Func<EntradaClasificacion, bool>> predicado = null, Func<IQueryable<EntradaClasificacion>, IOrderedQueryable<EntradaClasificacion>> ordenarPor = null, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<EntradaClasificacion>();
            EntradaClasificacion c = await repo.UnicoAsync(predicado);
            c.TipoValoracionDocumentalId = await ObtieneValoracionDocumental(c);
            await seguridad.AccesoValidoEntradaClasificacion(c);
            return c.Copia();
        }



        public Task<List<EntradaClasificacion>> ObtenerAsync(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            return repo.ObtenerAsync(predicado);
        }
        public Task<List<EntradaClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return repo.ObtenerAsync(SqlCommand);
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<EntradaClasificacion>();
            string buscado = "";
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    buscado = Query.Filtros[i].Valor;
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }

            List<EntradaClasificacion> r;
            var cc = Query.Filtros.Where(x => x.Propiedad.Equals("CuadroClasificacionId", StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
            if (cc != null)
            {
                r = await UDT.Context.EntradaClasificacion.Where(x => x.CuadroClasifiacionId == cc.Valor && x.Eliminada == false && (x.Nombre.Contains(buscado) || x.Clave.Contains(buscado))).ToListAsync();
            }
            else
            {
                r = await UDT.Context.EntradaClasificacion.Where(x => x.Eliminada == false && (x.Nombre.Contains(buscado) || x.Clave.Contains(buscado))).ToListAsync();
            }



            List<ValorListaOrdenada> l = r.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = $"{x.Clave} {x.Nombre}"
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await repo.ObtenerAsync(x => Lista.Contains(x.Id.Trim()));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = $"{x.Clave} {x.Nombre}"
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }


        public async Task<ICollection<string>> Purgar()
        {
            // No se pueden purgar las entradas de clasificación
            throw new NotImplementedException();
        }


        #region No implmentados



        public Task<IEnumerable<EntradaClasificacion>> CrearAsync(params EntradaClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EntradaClasificacion>> CrearAsync(IEnumerable<EntradaClasificacion> entities, CancellationToken cancellationToken = default)
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







        public Task<IPaginado<EntradaClasificacion>> ObtenerPaginadoAsync(Expression<Func<EntradaClasificacion, bool>> predicate = null, Func<IQueryable<EntradaClasificacion>, IOrderedQueryable<EntradaClasificacion>> orderBy = null, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<EntradaClasificacion> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }








        #endregion
    }
}