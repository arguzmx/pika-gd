using LazyCache;
using Microsoft.EntityFrameworkCore;
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
    public class ServicioPosicionAlmacen : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioPosicionAlmacen
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PosicionAlmacen> repo;

        public ServicioPosicionAlmacen(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
        {
            this.repo = UDT.ObtenerRepositoryAsync<PosicionAlmacen>(new QueryComposer<PosicionAlmacen>());
        }

        public async Task<bool> Existe(Expression<Func<PosicionAlmacen, bool>> predicado)
        {
            List<PosicionAlmacen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<PosicionAlmacen> CrearAsync(PosicionAlmacen entity, CancellationToken cancellationToken = default)
        {

            seguridad.EstableceDatosProceso<PosicionAlmacen>();
            ZonaAlmacen z = await this.UDT.Context.ZonasAlmacen.SingleOrDefaultAsync(x => x.Id == entity.ZonaAlmacenId);

            if(z==null)
            {
                throw new ExErrorRelacional("La zona de almacén no existe");
            }
            await seguridad.AccesoValidoZonaAlmacen(z);

            entity.AlmacenArchivoId = z.AlmacenArchivoId;
            entity.ArchivoId = z.ArchivoId;
            entity.ZonaAlmacenId = z.Id;
            await seguridad.AccesoValidoPosicionAlmacen(entity);
            


            if (await Existe(x => x.ZonaAlmacenId == z.Id && 
                                x.Indice == entity.Indice &&
                                x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

     

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);

            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);
            return entity.Copia();
        }



        public async Task ActualizarAsync(PosicionAlmacen entity)
        {
            seguridad.EstableceDatosProceso<PosicionAlmacen>();
            await seguridad.AccesoValidoPosicionAlmacen(entity);
            PosicionAlmacen o = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if(o.ZonaAlmacenId != entity.ZonaAlmacenId)
            {
                ZonaAlmacen z = await this.UDT.Context.ZonasAlmacen.SingleOrDefaultAsync(x => x.Id == entity.ZonaAlmacenId);

                if (z == null)
                {
                    throw new ExErrorRelacional("La zona de almacén no existe");
                }
                await seguridad.AccesoValidoZonaAlmacen(z);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.ZonaAlmacenId == o.ZonaAlmacenId
            && x.Indice == entity.Indice 
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            string original = o.Flat();

            o.Nombre = entity.Nombre;
            o.CodigoBarras = entity.CodigoBarras;
            o.Localizacion = entity.Localizacion;
            o.CodigoElectronico = entity.CodigoElectronico;
            o.Indice  = entity.Indice;
            // Recalcula la ocupacion
            if (o.IncrementoContenedor != entity.IncrementoContenedor)
            {
                var l = UDT.Context.ContenedoresAlmacen.Where(x => x.PosicionAlmacenId == o.Id).ToList();
                decimal ocupacion = l.Count() * entity.IncrementoContenedor;
                o.Ocupacion = ocupacion;
                o.IncrementoContenedor = entity.IncrementoContenedor;
            }  else
            {
                o.Ocupacion = entity.Ocupacion;
            }

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id,  o.Nombre, original.JsonDiff(o.Flat()));
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
        public async Task<IPaginado<PosicionAlmacen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PosicionAlmacen>, IIncludableQueryable<PosicionAlmacen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PosicionAlmacen>();
            Query = GetDefaultQuery(Query);

            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "ZonaAlmacenId");
            if (f == null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*","*");
            }

            var permitir = await seguridad.AccesoCacheZonasAlmacen(f.Valor);
            if (!permitir)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*","*");
            }

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<PosicionAlmacen>> CrearAsync(params PosicionAlmacen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PosicionAlmacen>> CrearAsync(IEnumerable<PosicionAlmacen> entities, CancellationToken cancellationToken = default)
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
            seguridad.EstableceDatosProceso<PosicionAlmacen>();
            List<PosicionAlmacen> listaEliminados = new List<PosicionAlmacen>();

            foreach (var Id in ids)
            {
                PosicionAlmacen o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {

                    await seguridad.AccesoValidoPosicionAlmacen(o);
                    listaEliminados.Add(o);
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

        public Task<List<PosicionAlmacen>> ObtenerAsync(Expression<Func<PosicionAlmacen, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<PosicionAlmacen>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<PosicionAlmacen>> ObtenerPaginadoAsync(Expression<Func<PosicionAlmacen, bool>> predicate = null, Func<IQueryable<PosicionAlmacen>, IOrderedQueryable<PosicionAlmacen>> orderBy = null, Func<IQueryable<PosicionAlmacen>, IIncludableQueryable<PosicionAlmacen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<PosicionAlmacen> UnicoAsync(Expression<Func<PosicionAlmacen, bool>> predicado = null, Func<IQueryable<PosicionAlmacen>, IOrderedQueryable<PosicionAlmacen>> ordenarPor = null, Func<IQueryable<PosicionAlmacen>, IIncludableQueryable<PosicionAlmacen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            PosicionAlmacen a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        public async Task EliminarRelaciones(List<PosicionAlmacen> ids)
        {
            if (ids.Count > 0)
            {
         
            }
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<PosicionAlmacen>();
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }

            Query = GetDefaultQuery(Query);

            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "ZonaAlmacenId");
            if (f == null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*","*");
            }

            var permitir = await seguridad.AccesoCacheZonasAlmacen(f.Valor);
            if (!permitir)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = $"{x.Nombre}-{x.Indice}"
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
                Texto = $"{x.Nombre}-{x.Indice}"
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public Task<PosicionAlmacen> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
    }
}

