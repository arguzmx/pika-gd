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
   public class ServicioPermiso : ContextoServicioContenido,
        IServicioInyectable, IServicioPermiso
    {
        private const string DEFAULT_SORT_COL = "Id";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Permiso> repo;
        private IRepositorioAsync<DestinatarioPermiso> repoDestinatariosPermiso;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioPermiso(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Permiso>(new QueryComposer<Permiso>());
            this.repoDestinatariosPermiso = UDT.ObtenerRepositoryAsync<DestinatarioPermiso>(new QueryComposer<DestinatarioPermiso>());
        }




        public async Task<bool> Existe(Expression<Func<Permiso, bool>> predicado)
        {
            List<Permiso> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Permiso> CrearAsync(Permiso entity, CancellationToken cancellationToken = default)
        {

          

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);

            if(entity.Destinatarios!=null)
            {
                foreach(var d in entity.Destinatarios)
                {
                    d.PermisoId = entity.Id;
                    await this.repoDestinatariosPermiso.CrearAsync(d);

                }
            }

            UDT.SaveChanges();
            return entity.Copia();

        }


        public async Task ActualizarAsync(Permiso entity)
        {

            Permiso o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            o.Leer = entity.Leer;
            o.Escribir = entity.Escribir;
            o.Eliminar = entity.Eliminar;
            o.Crear = entity.Crear;

            if (entity.Destinatarios != null)
            {

                List<DestinatarioPermiso> destinatarios = await this.repoDestinatariosPermiso.ObtenerAsync(
                    x => x.PermisoId == o.Id);

                if(destinatarios.Count>0)
                {
                    await this.repoDestinatariosPermiso.EliminarRango(destinatarios);
                }

                foreach (var d in entity.Destinatarios)
                {
                    d.PermisoId = entity.Id;
                    await this.repoDestinatariosPermiso.CrearAsync(d);

                }
            }


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
        public async Task<IPaginado<Permiso>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Permiso>, IIncludableQueryable<Permiso, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

      

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Permiso d;
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



        public async Task<Permiso> UnicoAsync(Expression<Func<Permiso, bool>> predicado = null, Func<IQueryable<Permiso>, IOrderedQueryable<Permiso>> ordenarPor = null, Func<IQueryable<Permiso>, IIncludableQueryable<Permiso, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Permiso d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);


            return d.Copia();
        }



        #region No Implemenatdaos

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

        public Task<List<Permiso>> ObtenerAsync(Expression<Func<Permiso, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Permiso>> ObtenerAsync(string SqlCommand)
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
 
        #endregion
    }



}