using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioTipoDisposicionDocumental : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTipoDisposicionDocumental
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        
        private IRepositorioAsync<TipoDisposicionDocumental> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<EntradaClasificacion> repoEC;

        public ServicioTipoDisposicionDocumental(
         IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
         ILogger<ServicioTipoDisposicionDocumental> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoEC= UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
        }

        public async Task<bool> Existe(Expression<Func<TipoDisposicionDocumental, bool>> predicado)
        {
            List<TipoDisposicionDocumental> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoDisposicionDocumental> CrearAsync(TipoDisposicionDocumental entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Id == entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            TipoDisposicionDocumental tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }
            entity.Nombre = entity.Nombre.Trim();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(TipoDisposicionDocumental entity)
        {
            if (await Existe(x => x.Id == entity.Id &&
           string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            TipoDisposicionDocumental tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            if (tmp == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            tmp.Nombre = entity.Nombre.Trim();
            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
        }


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            TipoDisposicionDocumental o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        Console.WriteLine($"\n Delete *** {Id}");
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            Console.WriteLine($"\n Delete entidad != nuu *** {Id}");

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


        public Task<List<TipoDisposicionDocumental>> ObtenerAsync(Expression<Func<TipoDisposicionDocumental, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<TipoDisposicionDocumental> UnicoAsync(Expression<Func<TipoDisposicionDocumental, bool>> predicado = null, Func<IQueryable<TipoDisposicionDocumental>, IOrderedQueryable<TipoDisposicionDocumental>> ordenarPor = null, Func<IQueryable<TipoDisposicionDocumental>, IIncludableQueryable<TipoDisposicionDocumental, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            TipoDisposicionDocumental d = await this.repo.UnicoAsync(predicado);
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

        public async Task<IPaginado<TipoDisposicionDocumental>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoDisposicionDocumental>, IIncludableQueryable<TipoDisposicionDocumental, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public Task<List<TipoDisposicionDocumental>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        #region sin implementar

        public Task<IEnumerable<TipoDisposicionDocumental>> CrearAsync(params TipoDisposicionDocumental[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoDisposicionDocumental>> CrearAsync(IEnumerable<TipoDisposicionDocumental> entities, CancellationToken cancellationToken = default)
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


       


        public async Task<IPaginado<TipoDisposicionDocumental>> ObtenerPaginadoAsync(Expression<Func<TipoDisposicionDocumental, bool>> predicate = null, Func<IQueryable<TipoDisposicionDocumental>, IOrderedQueryable<TipoDisposicionDocumental>> orderBy = null, Func<IQueryable<TipoDisposicionDocumental>, IIncludableQueryable<TipoDisposicionDocumental, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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