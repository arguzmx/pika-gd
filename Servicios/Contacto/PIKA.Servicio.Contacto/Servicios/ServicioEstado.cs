using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;


namespace PIKA.Servicio.Contacto
{

    public class ServicioEstado : ContextoServicioOContacto
        , IServicioInyectable, IServicioEstado

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Estado> repo;
        private IRepositorioAsync<Pais> repoPais;
        private UnidadDeTrabajo<DbContextContacto> UDT;

        public ServicioEstado(
         IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones,
         ILogger<ServicioEstado> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContacto>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Estado>(new QueryComposer<Estado>());
            this.repoPais = UDT.ObtenerRepositoryAsync<Pais>(new QueryComposer<Pais>());

        }

        public async Task<bool> Existe(Expression<Func<Estado, bool>> predicado)
        {
            List<Estado> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Estado> CrearAsync(Estado entity, CancellationToken cancellationToken = default)
        {
            Pais p = await this.repoPais.UnicoAsync(x => x.Id == entity.PaisId);
            if(p==null) throw new EXNoEncontrado(entity.PaisId);

            Estado tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }


        public async Task ActualizarAsync(Estado entity)
        {
            Estado tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            tmp.Nombre = entity.Nombre;
            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
        }


        public async Task<List<Estado>> ObtienePorPadre(string padreId) {
            List<Estado> l = await this.repo.ObtenerAsync(x => x.PaisId == padreId);

            return l.Select(x => x.Copia()).ToList(); ;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Estado o;
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


        public Task<List<Estado>> ObtenerAsync(Expression<Func<Estado, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<Estado> UnicoAsync(Expression<Func<Estado, bool>> predicado = null, Func<IQueryable<Estado>, IOrderedQueryable<Estado>> ordenarPor = null, Func<IQueryable<Estado>, IIncludableQueryable<Estado, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Estado d = await this.repo.UnicoAsync(predicado);
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

        public async Task<IPaginado<Estado>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Estado>, IIncludableQueryable<Estado, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query )
        {
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

        #region sin implementar

        public Task<IEnumerable<Estado>> CrearAsync(params Estado[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Estado>> CrearAsync(IEnumerable<Estado> entities, CancellationToken cancellationToken = default)
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


        public Task<List<Estado>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<Estado>> ObtenerPaginadoAsync(Expression<Func<Estado, bool>> predicate = null, Func<IQueryable<Estado>, IOrderedQueryable<Estado>> orderBy = null, Func<IQueryable<Estado>, IIncludableQueryable<Estado, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
