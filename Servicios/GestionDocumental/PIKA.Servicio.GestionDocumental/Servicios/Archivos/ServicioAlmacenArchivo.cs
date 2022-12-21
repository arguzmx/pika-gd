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
    public class ServicioAlmacenArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioAlmacenArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private IRepositorioAsync<AlmacenArchivo> repo;

        public ServicioAlmacenArchivo(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO)
        {
            this.repo = UDT.ObtenerRepositoryAsync<AlmacenArchivo>(new QueryComposer<AlmacenArchivo>());
        }

        public async Task<bool> Existe(Expression<Func<AlmacenArchivo, bool>> predicado)
        {
            List<AlmacenArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<AlmacenArchivo> CrearAsync(AlmacenArchivo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<AlmacenArchivo>();

            await seguridad.AccesoValidoAlmacen(entity);

            if (await Existe(x => x.ArchivoId == entity.ArchivoId && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity;
        }

        public async Task ActualizarAsync(AlmacenArchivo entity)
        {
            seguridad.EstableceDatosProceso<AlmacenArchivo>();

            AlmacenArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            string original = o.Flat();
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            await seguridad.AccesoValidoAlmacen(o);

            if (await Existe(x =>
            x.Id != entity.Id && x.ArchivoId == o.ArchivoId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Clave= entity.Clave;
            o.FolioActualContenedor = entity.FolioActualContenedor;
            o.HabilitarFoliado = entity.HabilitarFoliado;
            o.MacroFolioContenedor = entity.MacroFolioContenedor;
            o.Ubicacion = entity.Ubicacion;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

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
        public async Task<IPaginado<AlmacenArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AlmacenArchivo>, IIncludableQueryable<AlmacenArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<AlmacenArchivo>();

            Query = GetDefaultQuery(Query);

            if(!Query.Filtros.Any(x=>x.Propiedad == "ArchivoId"))
            {
                await seguridad.EmiteDatosSesionIncorrectos("*","*");
            }

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<AlmacenArchivo>> CrearAsync(params AlmacenArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AlmacenArchivo>> CrearAsync(IEnumerable<AlmacenArchivo> entities, CancellationToken cancellationToken = default)
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
            seguridad.EstableceDatosProceso<AlmacenArchivo>();

            List<AlmacenArchivo> listaEliminados = new List<AlmacenArchivo>();
            foreach (var Id in ids)
            {
                AlmacenArchivo o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id);
                    if (o != null)
                    {
                        await seguridad.AccesoValidoAlmacen(o);

                        // no es posible eliminar el contenedor si:
                        // Tiene activos alojados
                        if(UDT.Context.ActivoContenedorAlmacen.Any(x=>x.ContenedorAlmacenId == o.Id))
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

        public Task<List<AlmacenArchivo>> ObtenerAsync(Expression<Func<AlmacenArchivo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<AlmacenArchivo>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<AlmacenArchivo>> ObtenerPaginadoAsync(Expression<Func<AlmacenArchivo, bool>> predicate = null, Func<IQueryable<AlmacenArchivo>, IOrderedQueryable<AlmacenArchivo>> orderBy = null, Func<IQueryable<AlmacenArchivo>, IIncludableQueryable<AlmacenArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<AlmacenArchivo> UnicoAsync(Expression<Func<AlmacenArchivo, bool>> predicado = null, Func<IQueryable<AlmacenArchivo>, IOrderedQueryable<AlmacenArchivo>> ordenarPor = null, Func<IQueryable<AlmacenArchivo>, IIncludableQueryable<AlmacenArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            AlmacenArchivo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<AlmacenArchivo>();

            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }

            Query = GetDefaultQuery(Query);
            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id.Trim()));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public Task<AlmacenArchivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
    }
}

