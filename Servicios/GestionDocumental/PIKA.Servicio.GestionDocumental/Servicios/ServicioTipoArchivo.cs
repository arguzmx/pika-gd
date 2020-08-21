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
    public class ServicioTipoArchivo: ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTipoArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoArchivo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<Archivo> repoA;
        public ServicioTipoArchivo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioTipoArchivo> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
            this.repoA=UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
        }
        public async Task<bool> Existe(Expression<Func<TipoArchivo, bool>> predicado)
        {
            List<TipoArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<TipoArchivo> CrearAsync(TipoArchivo entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Id.Equals(entity.Id.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ExErrorRelacional(entity.Id);

            if (await Existe(x => x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            if (!await Existe(x => x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)
             && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            )) {
                entity.Id = entity.Id.Trim();
                entity.Nombre = entity.Nombre.Trim();
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            }
            
            return entity.Copia();
        }
        public async Task ActualizarAsync(TipoArchivo entity)
        {
            TipoArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            if (o == null)
                throw new EXNoEncontrado(entity.Id);

            if (await Existe(x => x.Id!=entity.Id.Trim() && x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            if (!await Existe(x => x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)
             && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                o.Nombre = entity.Nombre.Trim();
                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }
        public Task<List<TipoArchivo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
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
        public async Task<IPaginado<TipoArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            TipoArchivo o;
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
        public async Task<TipoArchivo> UnicoAsync(Expression<Func<TipoArchivo, bool>> predicado = null, Func<IQueryable<TipoArchivo>, IOrderedQueryable<TipoArchivo>> ordenarPor = null, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            TipoArchivo t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }

        public Task<List<TipoArchivo>> ObtenerAsync(Expression<Func<TipoArchivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
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
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = true,
                    Operador = "eq",
                    Valor = "true"
                });
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

        #region Sin Implementar

        public Task<IEnumerable<TipoArchivo>> CrearAsync(params TipoArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoArchivo>> CrearAsync(IEnumerable<TipoArchivo> entities, CancellationToken cancellationToken = default)
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


        public Task<IPaginado<TipoArchivo>> ObtenerPaginadoAsync(Expression<Func<TipoArchivo, bool>> predicate = null, Func<IQueryable<TipoArchivo>, IOrderedQueryable<TipoArchivo>> orderBy = null, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
