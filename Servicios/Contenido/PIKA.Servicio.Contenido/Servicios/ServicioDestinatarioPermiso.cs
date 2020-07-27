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
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioDestinatarioPermiso : ContextoServicioContenido,
        IServicioInyectable, IServicioDestinatarioPermiso
    {
        private const string DEFAULT_SORT_COL = "PermisoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<DestinatarioPermiso> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioDestinatarioPermiso(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioPermiso> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<DestinatarioPermiso>(new QueryComposer<DestinatarioPermiso>());
        }




        public async Task<bool> Existe(Expression<Func<DestinatarioPermiso, bool>> predicado)
        {
            List<DestinatarioPermiso> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<DestinatarioPermiso> CrearAsync(DestinatarioPermiso entity, CancellationToken cancellationToken = default)
        {
            try
            {
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
                return entity.Copia();

            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("Identificador de permiso no válido");
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear Unidad Organizacional {0}", ex.Message);
                throw ex;
            }
            
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
        public async Task<IPaginado<DestinatarioPermiso>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<DestinatarioPermiso>, 
            IIncludableQueryable<DestinatarioPermiso, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

      

        public async Task<ICollection<string>> Eliminar(string permisoId , string[] ids)
        {
            DestinatarioPermiso d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.PermisoId == permisoId && x.DestinatarioId == Id);
                if (d != null)
                {
                    await this.repo.Eliminar(d);
                    listaEliminados.Add(Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }



        public async Task<DestinatarioPermiso> UnicoAsync(Expression<Func<DestinatarioPermiso, bool>> predicado = null,
            Func<IQueryable<DestinatarioPermiso>, IOrderedQueryable<DestinatarioPermiso>> ordenarPor = null, 
            Func<IQueryable<DestinatarioPermiso>, IIncludableQueryable<DestinatarioPermiso, object>> incluir = null, 
            bool inhabilitarSegumiento = true)
        {

            DestinatarioPermiso d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);


            return d.Copia();
        }



        #region No Implemenatdaos

        public Task ActualizarAsync(DestinatarioPermiso entity)
        {
            // Los destinatarios de los permisos no se actualizan
            throw new NotImplementedException();

        }
        public Task<IEnumerable<Permiso>> CrearAsync(params Permiso[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Permiso>> CrearAsync(IEnumerable<Permiso> entities, CancellationToken cancellationToken = default)
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

        public Task<List<DestinatarioPermiso>> ObtenerAsync(Expression<Func<DestinatarioPermiso, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<DestinatarioPermiso>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Permiso>> ObtenerPaginadoAsync(Expression<Func<Permiso, bool>> predicate = null, Func<IQueryable<Permiso>, IOrderedQueryable<Permiso>> orderBy = null, Func<IQueryable<Permiso>, IIncludableQueryable<Permiso, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<DestinatarioPermiso>> ObtenerPaginadoAsync(Expression<Func<DestinatarioPermiso, bool>> predicate = null, Func<IQueryable<DestinatarioPermiso>, IOrderedQueryable<DestinatarioPermiso>> orderBy = null, Func<IQueryable<DestinatarioPermiso>, IIncludableQueryable<DestinatarioPermiso, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DestinatarioPermiso>> CrearAsync(params DestinatarioPermiso[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DestinatarioPermiso>> CrearAsync(IEnumerable<DestinatarioPermiso> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        #endregion
    }



}