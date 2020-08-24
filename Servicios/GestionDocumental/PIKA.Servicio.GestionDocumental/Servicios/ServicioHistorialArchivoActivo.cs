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
  public  class ServicioHistorialArchivoActivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioHistorialArchivoActivo
    {
        private const string DEFAULT_SORT_COL = "ArchivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<HistorialArchivoActivo> repo;
        private IRepositorioAsync<Activo> repoActivo;
        private IRepositorioAsync<Archivo> repoArchivo;

        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioHistorialArchivoActivo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioHistorialArchivoActivo> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<HistorialArchivoActivo>(new QueryComposer<HistorialArchivoActivo>());
            this.repoActivo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.repoArchivo = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
        }

        public async Task<bool> Existe(Expression<Func<HistorialArchivoActivo, bool>> predicado)
        {
            List<HistorialArchivoActivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteActivo(Expression<Func<Activo, bool>> predicado)
        {
            List<Activo> l = await this.repoActivo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteArchivo(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repoArchivo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<HistorialArchivoActivo> CrearAsync(HistorialArchivoActivo entity, CancellationToken cancellationToken = default)
        {
            if (!await ExisteArchivo(x => x.Id.Equals(entity.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada!=true))
                throw new ExErrorRelacional(entity.ArchivoId);
            if (!await ExisteActivo(x => x.Id.Equals(entity.ActivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada!=true))
                throw new ExErrorRelacional(entity.ActivoId);
                        
            entity.ArchivoId = entity.ArchivoId.Trim();
            entity.ActivoId = entity.ActivoId.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
           
            UDT.SaveChanges();
            return entity.Copia();
        }
        public async Task ActualizarAsync(HistorialArchivoActivo entity)
        {

            HistorialArchivoActivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (!await ExisteArchivo(x => x.Id.Equals(entity.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ExErrorRelacional(entity.ArchivoId);

            if (!await ExisteActivo(x => x.Id.Equals(entity.ActivoId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                throw new ExErrorRelacional(entity.ActivoId);
            

            o.ActivoId = entity.ActivoId.Trim();
            o.ArchivoId = entity.ArchivoId.Trim();
            o.FechaEgreso = entity.FechaEgreso;
            o.FechaIngreso = entity.FechaIngreso;
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
        public async Task<IPaginado<HistorialArchivoActivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<HistorialArchivoActivo>, IIncludableQueryable<HistorialArchivoActivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            HistorialArchivoActivo o;
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
        public Task<List<HistorialArchivoActivo>> ObtenerAsync(Expression<Func<HistorialArchivoActivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        public async Task<HistorialArchivoActivo> UnicoAsync(Expression<Func<HistorialArchivoActivo, bool>> predicado = null, Func<IQueryable<HistorialArchivoActivo>, IOrderedQueryable<HistorialArchivoActivo>> ordenarPor = null, Func<IQueryable<HistorialArchivoActivo>, IIncludableQueryable<HistorialArchivoActivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            HistorialArchivoActivo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }
        public Task<List<HistorialArchivoActivo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        #region Sin Implementación
        public Task<IPaginado<HistorialArchivoActivo>> ObtenerPaginadoAsync(Expression<Func<HistorialArchivoActivo, bool>> predicate = null, Func<IQueryable<HistorialArchivoActivo>, IOrderedQueryable<HistorialArchivoActivo>> orderBy = null, Func<IQueryable<HistorialArchivoActivo>, IIncludableQueryable<HistorialArchivoActivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<HistorialArchivoActivo>> CrearAsync(params HistorialArchivoActivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<HistorialArchivoActivo>> CrearAsync(IEnumerable<HistorialArchivoActivo> entities, CancellationToken cancellationToken = default)
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

