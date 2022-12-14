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
    public class ServicioCuadroClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioCuadroClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<CuadroClasificacion> repo;
        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IOCuadroClasificacion ioCuadroClasificacion;

        public ServicioCuadroClasificacion(
           IRegistroAuditoria registroAuditoria,
           IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger,
           IOptions<ConfiguracionServidor> Config
           ) : base(registroAuditoria, proveedorOpciones, Logger,
               null, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF)
        {
            ConfiguracionServidor = Config.Value;
            UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            repo = UDT.ObtenerRepositoryAsync(new QueryComposer<CuadroClasificacion>());
            ioCuadroClasificacion = new IOCuadroClasificacion(registroAuditoria, Logger, proveedorOpciones);
        }


        public async Task<bool> Existe(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            List<CuadroClasificacion> l = await repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<CuadroClasificacion> CrearAsync(CuadroClasificacion entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            await seguridad.IdEnDominio(entity.OrigenId);
            
            if (await Existe(x => x.OrigenId == entity.OrigenId && x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada != true))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = Guid.NewGuid().ToString();

            entity.EstadoCuadroClasificacionId = EstadoCuadroClasificacion.ESTADO_ACTIVO;
            entity.Nombre = entity.Nombre.Trim();
            entity.OrigenId = entity.OrigenId.Trim();
            entity.TipoOrigenId = entity.TipoOrigenId.Trim();

            await repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id,entity.Nombre);

            return entity.Copia();
        }


        public async Task ActualizarAsync(CuadroClasificacion entity)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            CuadroClasificacion o = await repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            await seguridad.IdEnDominio(o.OrigenId);
            
            string original = System.Text.Json.JsonSerializer.Serialize(o);

            if(!UDT.Context.EstadosCuadroClasificacion.Any(x => x.Id == entity.EstadoCuadroClasificacionId))
            {
                throw new ExDatosNoValidos(entity.EstadoCuadroClasificacionId.Trim());
            }

            if (await Existe(x => x.Id != entity.Id.Trim() &&
            string.Equals(x.Nombre, entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.OrigenId == entity.OrigenId && x.TipoOrigenId == entity.TipoOrigenId && x.Eliminada != true))
            {
                throw new ExElementoExistente(entity.Nombre.Trim());
            }

            o.Nombre = entity.Nombre.Trim();
            o.Eliminada = entity.Eliminada;
            if (!string.IsNullOrEmpty(entity.EstadoCuadroClasificacionId.Trim()))
                o.EstadoCuadroClasificacionId = entity.EstadoCuadroClasificacionId.Trim();

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(JsonConvert.SerializeObject(o)));
            await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(entity.Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
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

            query.Filtros.RemoveAll(x => x.Propiedad == "OrigenId");
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "OrigenId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.DominioId });
            ;

            return query;
        }


        public async Task<IPaginado<CuadroClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            Query = GetDefaultQuery(Query);
            var respuesta = await repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            List<CuadroClasificacion> lista = new List<CuadroClasificacion>();

            foreach (var Id in ids)
            {
                CuadroClasificacion c = await repo.UnicoAsync(x => x.Id == Id.Trim());
                if (c != null)
                {

                    await seguridad.IdEnDominio(c.OrigenId);
                    
                    lista.Add(c);
                }
            }

            foreach (var c in lista)
            {
                c.Eliminada = true;
                UDT.Context.Entry(c).State = EntityState.Modified;

                await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(c.Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
                await seguridad.RegistraEventoEliminar(c.Id, c.Nombre);
            }

            if (lista.Count > 0) UDT.SaveChanges();

            return lista.Select(x => x.Id).ToList();
        }



        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            List<CuadroClasificacion> lista = new List<CuadroClasificacion>();

            foreach (var Id in ids)
            {
                CuadroClasificacion c = await repo.UnicoAsync(x => x.Id == Id.Trim());
                if (c != null)
                {
                    await seguridad.IdEnDominio(c.OrigenId);
                    lista.Add(c);
                }
            }

            foreach (var c in lista)
            {
                string original = System.Text.Json.JsonSerializer.Serialize(c);
                c.Nombre = await RestaurarNombre(c.Nombre, c.OrigenId); ;
                c.Eliminada = false;
                UDT.Context.Entry(c).State = EntityState.Modified;

                await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(c.Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
                await seguridad.RegistraEventoActualizar(c.Id, c.Nombre, original.JsonDiff(JsonConvert.SerializeObject(c)));

            }

            if (lista.Count > 0) UDT.SaveChanges();

            return lista.Select(x => x.Id).ToList();

        }


        private async Task<string> RestaurarNombre(string nombre, string OrigenId)
        {
            CuadroClasificacion cc = await UnicoAsync(x => x.OrigenId == OrigenId && x.Nombre == nombre && x.Eliminada != false);
            if (cc != null)
                nombre = cc.Nombre + $" {DateTime.Now.Ticks}";


            return nombre;

        }


        public async Task<CuadroClasificacion> UnicoAsync(Expression<Func<CuadroClasificacion, bool>> predicado = null, Func<IQueryable<CuadroClasificacion>, IOrderedQueryable<CuadroClasificacion>> ordenarPor = null, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            CuadroClasificacion c = await repo.UnicoAsync(predicado);
            await seguridad.IdEnDominio(c.OrigenId);
            return c.Copia();
        }



        public Task<List<CuadroClasificacion>> ObtenerAsync(Expression<Func<CuadroClasificacion, bool>> predicado)
        {
            return repo.ObtenerAsync(predicado);
        }
        public Task<List<CuadroClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return repo.ObtenerAsync(SqlCommand);
        }



        public async Task<byte[]> ExportarCuadroCalsificacionExcel(string CuadroClasificacionId)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            var cc = await UnicoAsync(x => x.Id == CuadroClasificacionId);
            await seguridad.IdEnDominio(cc.OrigenId);

            IOCuadroClasificacion iocuadro = new IOCuadroClasificacion(registroAuditoria, logger, proveedorOpciones);

            return await iocuadro.ExportarCuadroCalsificacionExcel(CuadroClasificacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
        }

        public async Task<string[]> Purgar()
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            List<string> ids = new List<string>();

            var ds = usuario.Accesos.Select(x => x.Dominio).ToList();
            var cuadros = UDT.Context.CuadrosClasificacion.Where(x => ds.Contains(x.OrigenId) && x.Eliminada == true).ToList();

            foreach (var c in cuadros)
            {
                await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(c.Id, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);

                string sqls = $"DELETE FROM {DBContextGestionDocumental.TablaEntradaClasificacion} WHERE CuadroClasifiacionId='{c.Id}'";
                UDT.Context.Database.ExecuteSqlRaw(sqls);

                UDT.Context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0;");

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaElementosClasificacion} WHERE CuadroClasifiacionId='{c.Id}'";
                UDT.Context.Database.ExecuteSqlRaw(sqls);

                UDT.Context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 1;");

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaCuadrosClasificacion} WHERE Id='{c.Id}'";
                UDT.Context.Database.ExecuteSqlRaw(sqls);

                ids.Add(c.Id);

                await seguridad.RegistraEventoPurgar(c.Id, c.Nombre);
            }

            return ids.ToArray();
        }



        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<CuadroClasificacion>();
            var ds = usuario.Accesos.Select(x => x.Dominio).ToList();
            List<Expression<Func<CuadroClasificacion, bool>>> filtros = new List<Expression<Func<CuadroClasificacion, bool>>>
            {
                x => ds.Contains(x.OrigenId)
            };


            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = false,
                    Operador = "eq",
                    Valor = "false"
                });
            }
            Query = GetDefaultQuery(Query);
            var resultados = await repo.ObtenerPaginadoAsync(Query, null, filtros);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }



        #region No implmentados

        public Task<IEnumerable<CuadroClasificacion>> CrearAsync(params CuadroClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CuadroClasificacion>> CrearAsync(IEnumerable<CuadroClasificacion> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<CuadroClasificacion>> ObtenerPaginadoAsync(Expression<Func<CuadroClasificacion, bool>> predicate = null, Func<IQueryable<CuadroClasificacion>, IOrderedQueryable<CuadroClasificacion>> orderBy = null, Func<IQueryable<CuadroClasificacion>, IIncludableQueryable<CuadroClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<CuadroClasificacion> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
