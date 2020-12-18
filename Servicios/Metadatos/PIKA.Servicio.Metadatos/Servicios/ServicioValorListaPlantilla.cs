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
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Servicios
{
  public  class ServicioValorListaPlantilla : ContextoServicioMetadatos,
        IServicioInyectable, IServicioValorListaPlantilla
    {
        private const string DEFAULT_SORT_COL = "Texto";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ValorListaPlantilla> repo;

        private UnidadDeTrabajo<DbContextMetadatos> UDT;

        public ServicioValorListaPlantilla(IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
           ILogger<ServicioValorListaPlantilla> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ValorListaPlantilla>(new QueryComposer<ValorListaPlantilla>());
        }
        public async Task<bool> Existe(Expression<Func<ValorListaPlantilla, bool>> predicado)
        {
            List<ValorListaPlantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<ValorListaPlantilla> CrearAsync(ValorListaPlantilla entity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entity.Texto))
            {
                throw new ExDatosNoValidos();
            }

            if (await Existe(x => x.PropiedadId.Equals(entity.PropiedadId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Texto.Equals(entity.Texto.Trim(), StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                throw new ExElementoExistente(entity.PropiedadId);
            }


            entity.PropiedadId = entity.PropiedadId.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(ValorListaPlantilla entity)
        {
            if (string.IsNullOrEmpty(entity.Texto))
            {
                throw new ExDatosNoValidos();
            }

            ValorListaPlantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }


            if (await Existe(x => x.PropiedadId.Equals(entity.PropiedadId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Texto.Equals(entity.Texto.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Id != entity.Id))
            {
                throw new ExElementoExistente(entity.PropiedadId);
            }

            o.Texto = entity.Texto.Trim();
            o.Indice = entity.Indice;
            o.PropiedadId = entity.PropiedadId.Trim();
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
        public async Task<IPaginado<ValorListaPlantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ValorListaPlantilla>, IIncludableQueryable<ValorListaPlantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ValorListaPlantilla o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    try
                    {
                        await this.repo.Eliminar(o);
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
        public Task<List<ValorListaPlantilla>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<ValorListaPlantilla>> ObtenerAsync(Expression<Func<ValorListaPlantilla, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<ValorListaPlantilla> UnicoAsync(Expression<Func<ValorListaPlantilla, bool>> predicado = null, Func<IQueryable<ValorListaPlantilla>, IOrderedQueryable<ValorListaPlantilla>> ordenarPor = null, Func<IQueryable<ValorListaPlantilla>, IIncludableQueryable<ValorListaPlantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ValorListaPlantilla a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            ValorListaPlantilla c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
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
                Texto = x.PropiedadId
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
                Texto = x.PropiedadId
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        #region Sin Implementación
        public Task<IPaginado<ValorListaPlantilla>> ObtenerPaginadoAsync(Expression<Func<ValorListaPlantilla, bool>> predicate = null, Func<IQueryable<ValorListaPlantilla>, IOrderedQueryable<ValorListaPlantilla>> orderBy = null, Func<IQueryable<ValorListaPlantilla>, IIncludableQueryable<ValorListaPlantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<ValorListaPlantilla>> CrearAsync(params ValorListaPlantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValorListaPlantilla>> CrearAsync(IEnumerable<ValorListaPlantilla> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        


        #endregion

    }
}
