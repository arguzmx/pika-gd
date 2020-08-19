using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
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
    public class ServicioEstadoCuadroClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioEstadoCuadroClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EstadoCuadroClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<CuadroClasificacion> repoCC;

        public ServicioEstadoCuadroClasificacion(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioEstadoCuadroClasificacion> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EstadoCuadroClasificacion>(new QueryComposer<EstadoCuadroClasificacion>());
            this.repoCC= UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
        }

        public async Task<bool> Existe(Expression<Func<EstadoCuadroClasificacion, bool>> predicado)
        {
            List<EstadoCuadroClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<EstadoCuadroClasificacion> CrearAsync(EstadoCuadroClasificacion entity, CancellationToken cancellationToken = default)
        {
            entity.Id = entity.Id.Trim();
            if (await Existe(x=>x.Id==entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            EstadoCuadroClasificacion tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }

            entity.Nombre = entity.Nombre.Trim();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }
        
        public async Task ActualizarAsync(EstadoCuadroClasificacion entity)
        {
            entity.Id = entity.Id.Trim();
            entity.Nombre= entity.Nombre.Trim();
            if (await Existe(x => x.Id != entity.Id &&
           string.Equals(x.Nombre, entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre.Trim());
            }
            EstadoCuadroClasificacion tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            tmp.Id = entity.Id;
            tmp.Nombre = entity.Nombre;
            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
        }
    
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            EstadoCuadroClasificacion o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null )
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
        public Task<List<EstadoCuadroClasificacion>> ObtenerAsync(Expression<Func<EstadoCuadroClasificacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<EstadoCuadroClasificacion> UnicoAsync(Expression<Func<EstadoCuadroClasificacion, bool>> predicado = null, Func<IQueryable<EstadoCuadroClasificacion>, IOrderedQueryable<EstadoCuadroClasificacion>> ordenarPor = null, Func<IQueryable<EstadoCuadroClasificacion>, IIncludableQueryable<EstadoCuadroClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EstadoCuadroClasificacion d = await this.repo.UnicoAsync(predicado);
            return d.Copia();
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

        public async Task<IPaginado<EstadoCuadroClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EstadoCuadroClasificacion>, IIncludableQueryable<EstadoCuadroClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
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


        public Task<List<EstadoCuadroClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);

        }
        #region sin implementar

        public Task<IEnumerable<EstadoCuadroClasificacion>> CrearAsync(params EstadoCuadroClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EstadoCuadroClasificacion>> CrearAsync(IEnumerable<EstadoCuadroClasificacion> entities, CancellationToken cancellationToken = default)
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

       


        public async Task<IPaginado<EstadoCuadroClasificacion>> ObtenerPaginadoAsync(Expression<Func<EstadoCuadroClasificacion, bool>> predicate = null, Func<IQueryable<EstadoCuadroClasificacion>, IOrderedQueryable<EstadoCuadroClasificacion>> orderBy = null, Func<IQueryable<EstadoCuadroClasificacion>, IIncludableQueryable<EstadoCuadroClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
