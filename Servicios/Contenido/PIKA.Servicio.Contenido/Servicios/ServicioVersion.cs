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
using Version = PIKA.Modelo.Contenido.Version;
namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioVersion : ContextoServicioContenido,
        IServicioInyectable, IServicioVersion
    {
        private const string DEFAULT_SORT_COL = "ElementoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Version> repo;
        private ICompositorConsulta<Version> compositor;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioVersion(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ICompositorConsulta<Version> compositorConsulta,
        ILogger<ServicioVersion> Logger,
        IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Version>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<Version, bool>> predicado)
        {
            List<Version> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Version> CrearAsync(Version entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.ElementoId.Equals(entity.ElementoId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.ElementoId);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            entity.Partes.FirstOrDefault().VersionId = entity.Id;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            try
            {


                return ClonaVersion(entity);
            }
            catch (Exception ex)
            {
                await this.repo.Eliminar(entity);
                UDT.SaveChanges();
                throw (ex);
            }
        }

        private Version ClonaVersion(Version entidad)
        {
            Version resuldtado = new Version()
            {
                Id = entidad.Id,
                ElementoId=entidad.ElementoId,
                FechaActualizacion=entidad.FechaActualizacion,
                FechaCreacion=entidad.FechaCreacion
            };

            return resuldtado;
        }

        public async Task ActualizarAsync(Version entity)
        {

            Version o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.ElementoId == entity.ElementoId ))
            {
                throw new ExElementoExistente(entity.ElementoId);
            }

            o.Activa = entity.Activa;
            o.FechaActualizacion = entity.FechaActualizacion;
            o.FechaCreacion = entity.FechaCreacion;
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
        public async Task<IPaginado<Version>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Version>, IIncludableQueryable<Version, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Version>> CrearAsync(params Version[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Version>> CrearAsync(IEnumerable<Version> entities, CancellationToken cancellationToken = default)
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
            Version d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    d.Eliminada = true;
                    UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<Version>> ObtenerAsync(Expression<Func<Version, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Version>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Version>> ObtenerPaginadoAsync(Expression<Func<Version, bool>> predicate = null, Func<IQueryable<Version>, IOrderedQueryable<Version>> orderBy = null, Func<IQueryable<Version>, IIncludableQueryable<Version, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Version> UnicoAsync(Expression<Func<Version, bool>> predicado = null, Func<IQueryable<Version>, IOrderedQueryable<Version>> ordenarPor = null, Func<IQueryable<Version>, IIncludableQueryable<Version, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Version d = await this.repo.UnicoAsync(predicado);

           
            return d.CopiaVersion();
        }

    }



}