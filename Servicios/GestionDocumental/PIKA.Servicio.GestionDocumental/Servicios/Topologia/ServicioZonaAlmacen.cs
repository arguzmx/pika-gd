using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private ILogger<ServicioCuadroClasificacion> LoggerCC;
        public ServicioZonaAlmacen(IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
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

            var almacen = UDT.Context.AlmacenesArchivo.SingleOrDefault(x => x.Id == entity.AlmacenArchivoId);

            if (almacen == null)
            {
                throw new ExErrorRelacional("El almacén no existe");
            }

            if (await Existe(x => x.AlmacenArchivoId == entity.AlmacenArchivoId && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.AlmacenArchivoId = almacen.Id;
            entity.ArchivoId = almacen.ArchivoId;
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(ZonaAlmacen entity)
        {

            ZonaAlmacen o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.AlmacenArchivoId == o.AlmacenArchivoId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;

            UDT.Context.Entry(o).State = EntityState.Modified;
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
        public async Task<IPaginado<ZonaAlmacen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ZonaAlmacen>, IIncludableQueryable<ZonaAlmacen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
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
            ZonaAlmacen o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            await this.repo.Eliminar(o);
                        }
                        this.UDT.SaveChanges();
                        listaEliminados.Add(o.Id);
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
            }
            UDT.SaveChanges();

            return listaEliminados;
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

        public async Task EliminarRelaciones(List<ZonaAlmacen> ids)
        {
            if (ids.Count > 0)
            {
         
            }
        }
        private string[] ListaIdEliminar(string[] ids) 
        {
            return ids;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
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
    }
}

