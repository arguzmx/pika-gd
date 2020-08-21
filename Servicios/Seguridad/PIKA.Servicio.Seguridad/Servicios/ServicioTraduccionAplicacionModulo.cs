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
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{
 
    public class ServicioTraduccionAplicacionModulo : ContextoServicioSeguridad
      , IServicioInyectable, IServicioTraduccionAplicacionModulo

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TraduccionAplicacionModulo> repo;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;

        public ServicioTraduccionAplicacionModulo(
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioTraduccionAplicacionModulo> Logger) : 
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TraduccionAplicacionModulo>(
                new QueryComposer<TraduccionAplicacionModulo>());
        }

        public async Task<bool> Existe(Expression<Func<TraduccionAplicacionModulo, bool>> predicado)
        {
            List<TraduccionAplicacionModulo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TraduccionAplicacionModulo> CrearAsync(TraduccionAplicacionModulo entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(TraduccionAplicacionModulo entity)
        {

            TraduccionAplicacionModulo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Descripcion = entity.Descripcion;
            o.UICulture = entity.UICulture;
            o.ModuloId = entity.ModuloId;
            o.AplicacionId = entity.AplicacionId;

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
        public async Task<IPaginado<TraduccionAplicacionModulo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TraduccionAplicacionModulo>, IIncludableQueryable<TraduccionAplicacionModulo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

      

        public async Task<TraduccionAplicacionModulo> UnicoAsync(Expression<Func<TraduccionAplicacionModulo, bool>> predicado = null, Func<IQueryable<TraduccionAplicacionModulo>, IOrderedQueryable<TraduccionAplicacionModulo>> ordenarPor = null, Func<IQueryable<TraduccionAplicacionModulo>, IIncludableQueryable<TraduccionAplicacionModulo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            TraduccionAplicacionModulo d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }
        #region sin implementar 
        public Task<IEnumerable<TraduccionAplicacionModulo>> CrearAsync(params TraduccionAplicacionModulo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TraduccionAplicacionModulo>> CrearAsync(IEnumerable<TraduccionAplicacionModulo> entities, CancellationToken cancellationToken = default)
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
            TraduccionAplicacionModulo o;
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


        public Task<List<TraduccionAplicacionModulo>> ObtenerAsync(Expression<Func<TraduccionAplicacionModulo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<TraduccionAplicacionModulo>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TraduccionAplicacionModulo>> ObtenerPaginadoAsync(Expression<Func<TraduccionAplicacionModulo, bool>> predicate = null, Func<IQueryable<TraduccionAplicacionModulo>, IOrderedQueryable<TraduccionAplicacionModulo>> orderBy = null, Func<IQueryable<TraduccionAplicacionModulo>, IIncludableQueryable<TraduccionAplicacionModulo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
