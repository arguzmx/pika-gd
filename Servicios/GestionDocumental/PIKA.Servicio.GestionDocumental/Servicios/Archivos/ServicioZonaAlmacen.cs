using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
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
    public class ServicioZonaAlmacen : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioZonaAlmacen
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ZonaAlmacen> repo;
        public ServicioZonaAlmacen(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO)
        {
            this.repo = UDT.ObtenerRepositoryAsync<ZonaAlmacen>(new QueryComposer<ZonaAlmacen>());
        }

        public async Task<bool> Existe(Expression<Func<ZonaAlmacen, bool>> predicado)
        {
            List<ZonaAlmacen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ZonaAlmacen> CrearAsync(ZonaAlmacen entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ZonaAlmacen>();
            var almacen = UDT.Context.AlmacenesArchivo.SingleOrDefault(x => x.Id == entity.AlmacenArchivoId);
            entity.AlmacenArchivoId = almacen.Id;
            entity.ArchivoId = almacen.ArchivoId;

            await seguridad.AccesoValidoZonaAlmacen(entity);

            if (almacen == null)
            {
                throw new ExErrorRelacional("El almacén no existe");
            }

            await seguridad.AccesoValidoAlmacen(almacen);

            if (await Existe(x => x.AlmacenArchivoId == entity.AlmacenArchivoId && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear( entity.Id, entity.Nombre);

            return entity;
        }

        public async Task ActualizarAsync(ZonaAlmacen entity)
        {
            seguridad.EstableceDatosProceso<ZonaAlmacen>();
            await seguridad.AccesoValidoZonaAlmacen(entity);

            ZonaAlmacen o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            string original = JsonConvert.SerializeObject(o);

            if (await Existe(x =>
            x.Id != entity.Id && x.AlmacenArchivoId == o.AlmacenArchivoId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar( o.Id, o.Nombre, original.JsonDiff(JsonConvert.SerializeObject(o)));

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
        public async Task<IPaginado<ZonaAlmacen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ZonaAlmacen>, IIncludableQueryable<ZonaAlmacen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ZonaAlmacen>();
            Query = GetDefaultQuery(Query);

            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "AlmacenArchivoId");
            if (f == null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var permitir = await seguridad.AccesoCacheAlmacenArchivo(f.Valor);
            if (!permitir)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ZonaAlmacen>> CrearAsync(params ZonaAlmacen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ZonaAlmacen>> CrearAsync(IEnumerable<ZonaAlmacen> entities, CancellationToken cancellationToken = default)
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<ZonaAlmacen>();
            List<ZonaAlmacen> listaEliminados = new List<ZonaAlmacen>();
            foreach (var Id in ids)
            {
                ZonaAlmacen o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    if (o != null)
                    {
                        await seguridad.AccesoValidoZonaAlmacen(o);
                        if(UDT.Context.Activos.Any(x=>x.ZonaAlmacenId == o.Id))
                        {
                            throw new ExElementoExistente();
                        }
                        listaEliminados.Add(o);
                    }
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var c in listaEliminados)
                {
                    await this.repo.Eliminar(c);
                    await seguridad.RegistraEventoEliminar(c.Id, c.Nombre);
                }

                UDT.SaveChanges();
            }

            return listaEliminados.Select(x=>x.Id).ToList();
        }

        public Task<List<ZonaAlmacen>> ObtenerAsync(Expression<Func<ZonaAlmacen, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<ZonaAlmacen>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ZonaAlmacen>> ObtenerPaginadoAsync(Expression<Func<ZonaAlmacen, bool>> predicate = null, Func<IQueryable<ZonaAlmacen>, IOrderedQueryable<ZonaAlmacen>> orderBy = null, Func<IQueryable<ZonaAlmacen>, IIncludableQueryable<ZonaAlmacen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ZonaAlmacen> UnicoAsync(Expression<Func<ZonaAlmacen, bool>> predicado = null, Func<IQueryable<ZonaAlmacen>, IOrderedQueryable<ZonaAlmacen>> ordenarPor = null, Func<IQueryable<ZonaAlmacen>, IIncludableQueryable<ZonaAlmacen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ZonaAlmacen a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<ZonaAlmacen>();
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }

            Query = GetDefaultQuery(Query);

            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "AlmacenArchivoId");
            if (f == null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*",  "*");
            }

            var permitir = await seguridad.AccesoCacheAlmacenArchivo(f.Valor);
            if (!permitir)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            logger.LogInformation($"{l.Count}");


            return l.OrderBy(x => x.Texto).ToList();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id.Trim()));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public Task<ZonaAlmacen> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
    }
}

