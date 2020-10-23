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
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioTipoGestorES : ContextoServicioContenido,
        IServicioInyectable, IServicioTipoGestorES
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoGestorES> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioTipoGestorES(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TipoGestorES>(new QueryComposer<TipoGestorES>());
        }




        public async Task<bool> Existe(Expression<Func<TipoGestorES, bool>> predicado)
        {
            List<TipoGestorES> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoGestorES> CrearAsync(TipoGestorES entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();

        }


        public async Task ActualizarAsync(TipoGestorES entity)
        {

            TipoGestorES o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
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
        public async Task<IPaginado<TipoGestorES>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoGestorES>, IIncludableQueryable<TipoGestorES, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

      

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            TipoGestorES d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    await this.repo.Eliminar(d);
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }



        public async Task<TipoGestorES> UnicoAsync(Expression<Func<TipoGestorES, bool>> predicado = null, Func<IQueryable<TipoGestorES>, IOrderedQueryable<TipoGestorES>> ordenarPor = null, Func<IQueryable<TipoGestorES>, IIncludableQueryable<TipoGestorES, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            TipoGestorES d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);


            return d.Copia();
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }




        #region No Implemenatdaos

        public Task<IEnumerable<TipoGestorES>> CrearAsync(params TipoGestorES[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoGestorES>> CrearAsync(IEnumerable<TipoGestorES> entities, CancellationToken cancellationToken = default)
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

        public Task<List<TipoGestorES>> ObtenerAsync(Expression<Func<TipoGestorES, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<TipoGestorES>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<TipoGestorES>> ObtenerPaginadoAsync(Expression<Func<TipoGestorES, bool>> predicate = null, Func<IQueryable<TipoGestorES>, IOrderedQueryable<TipoGestorES>> orderBy = null, Func<IQueryable<TipoGestorES>, IIncludableQueryable<TipoGestorES, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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