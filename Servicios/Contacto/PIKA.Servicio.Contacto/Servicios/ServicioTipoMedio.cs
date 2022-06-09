using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;


namespace PIKA.Servicio.Contacto
{

    public class ServicioTipoMedio : ContextoServicioOContacto
        , IServicioInyectable, IServicioTipoMedio

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoMedio> repo;
        private UnidadDeTrabajo<DbContextContacto> UDT;

        public ServicioTipoMedio(
         IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones,
         ILogger<ServicioTipoMedio> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContacto>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TipoMedio>(new QueryComposer<TipoMedio>());

        }

        public async Task<bool> Existe(Expression<Func<TipoMedio, bool>> predicado)
        {
            List<TipoMedio> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoMedio> CrearAsync(TipoMedio entity, CancellationToken cancellationToken = default)
        {
            TipoMedio tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }


        public async Task ActualizarAsync(TipoMedio entity)
        {
            TipoMedio tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            tmp.Nombre = entity.Nombre;
            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
        }
        
        


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            TipoMedio o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    try
                    {
                        await this.repo.Eliminar(o);
                        listaEliminados.Add(o.Id);
                    }
                    catch (Exception)
                    {}
                }
            }

            this.UDT.SaveChanges();

            return listaEliminados;

        }


        public Task<List<TipoMedio>> ObtenerAsync(Expression<Func<TipoMedio, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<TipoMedio> UnicoAsync(Expression<Func<TipoMedio, bool>> predicado = null, Func<IQueryable<TipoMedio>, IOrderedQueryable<TipoMedio>> ordenarPor = null, Func<IQueryable<TipoMedio>, IIncludableQueryable<TipoMedio, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            TipoMedio d = await this.repo.UnicoAsync(predicado);
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

        public async Task<IPaginado<TipoMedio>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoMedio>, IIncludableQueryable<TipoMedio, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = true,
                    Operador = FiltroConsulta.OP_CONTAINS,
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id) );
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        #region sin implementar

        public Task<IEnumerable<TipoMedio>> CrearAsync(params TipoMedio[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoMedio>> CrearAsync(IEnumerable<TipoMedio> entities, CancellationToken cancellationToken = default)
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


        public Task<List<TipoMedio>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<TipoMedio>> ObtenerPaginadoAsync(Expression<Func<TipoMedio, bool>> predicate = null, Func<IQueryable<TipoMedio>, IOrderedQueryable<TipoMedio>> orderBy = null, Func<IQueryable<TipoMedio>, IIncludableQueryable<TipoMedio, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
