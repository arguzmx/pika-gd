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
using Serilog.Data;

namespace PIKA.Servicio.Metadatos.Servicios
{
  public  class ServicioAlmacenDatos : ContextoServicioMetadatos,
        IServicioInyectable, IServicioAlmacenDatos
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<AlmacenDatos> repo;

        private UnidadDeTrabajo<DbContextMetadatos> UDT;

        public ServicioAlmacenDatos(IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
           ILogger<ServicioAlmacenDatos> Logger) : base(proveedorOpciones, Logger)
        {
           
                this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
                this.repo = UDT.ObtenerRepositoryAsync<AlmacenDatos>(new QueryComposer<AlmacenDatos>());
            
        }
        public async Task<bool> Existe(Expression<Func<AlmacenDatos, bool>> predicado)
        {
            List<AlmacenDatos> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        
        public async Task<AlmacenDatos> CrearAsync(AlmacenDatos entity, CancellationToken cancellationToken = default)
        {
           
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Id != entity.Id ))
            {
                throw new ExElementoExistente(entity.Nombre);
            }


            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(AlmacenDatos entity)
        {
            AlmacenDatos o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            
            if (await Existe(x => x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Id != entity.Id   ))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre.Trim();
            o.Usuario = entity.Usuario.Trim();
            o.Contrasena = entity.Contrasena.Trim();
            o.Direccion = entity.Direccion.Trim();
            o.Protocolo = entity.Protocolo.Trim();
            o.TipoAlmacenMetadatosId = entity.TipoAlmacenMetadatosId.Trim();
            o.Puerto = entity.Puerto.Trim();
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
        public async Task<IPaginado<AlmacenDatos>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AlmacenDatos>, IIncludableQueryable<AlmacenDatos, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            AlmacenDatos o;
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
        public Task<List<AlmacenDatos>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<AlmacenDatos>> ObtenerAsync(Expression<Func<AlmacenDatos, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<AlmacenDatos> UnicoAsync(Expression<Func<AlmacenDatos, bool>> predicado = null, Func<IQueryable<AlmacenDatos>, IOrderedQueryable<AlmacenDatos>> ordenarPor = null, Func<IQueryable<AlmacenDatos>, IIncludableQueryable<AlmacenDatos, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            AlmacenDatos a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

     
        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            AlmacenDatos c;
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

        #region Sin Implementación
        public Task<IPaginado<AlmacenDatos>> ObtenerPaginadoAsync(Expression<Func<AlmacenDatos, bool>> predicate = null, Func<IQueryable<AlmacenDatos>, IOrderedQueryable<AlmacenDatos>> orderBy = null, Func<IQueryable<AlmacenDatos>, IIncludableQueryable<AlmacenDatos, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<AlmacenDatos>> CrearAsync(params AlmacenDatos[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AlmacenDatos>> CrearAsync(IEnumerable<AlmacenDatos> entities, CancellationToken cancellationToken = default)
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
