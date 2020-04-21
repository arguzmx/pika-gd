using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion
{
    public class ServicioUnidadOrganizacional : 
        ContextoServicioOrganizacion, 
        IServicioInyectable, 
        IServicioUnidadOrganizacional
    {

        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<UnidadOrganizacional> repo;
        private ICompositorConsulta<UnidadOrganizacional> compositor;
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;
        
        public ServicioUnidadOrganizacional(
            IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
            ICompositorConsulta<UnidadOrganizacional> compositorConsulta,
            ILogger<ServicioUnidadOrganizacional> Logger,
            IServicioCache servicioCache): base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<UnidadOrganizacional>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<UnidadOrganizacional, bool>> predicado)
        {
            List<UnidadOrganizacional> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<UnidadOrganizacional> CrearAsync(UnidadOrganizacional entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }


        public async Task ActualizarAsync(UnidadOrganizacional entity)
        {
            UnidadOrganizacional uo = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (uo == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.Id == entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            uo.Nombre = entity.Nombre;
            uo.Eliminada = entity.Eliminada;
            UDT.Context.Entry(uo).State = EntityState.Modified;
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
                query = new Consulta() { indice = 0, tamano = 20, ord_columna  = DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<UnidadOrganizacional>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            //Query.Filtros.Add(new FiltroConsulta() { Operador =  operado, Property = COL_OWNERID, Value = OwnerId });
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<UnidadOrganizacional>> CrearAsync(params UnidadOrganizacional[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadOrganizacional>> CrearAsync(IEnumerable<UnidadOrganizacional> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            UnidadOrganizacional uo;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                uo = await this.repo.UnicoAsync(x => x.Id == Id);
                if (uo != null)
                {
                    uo.Eliminada = true;
                    UDT.Context.Entry(uo).State = EntityState.Modified;
                    listaEliminados.Add(uo.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<UnidadOrganizacional>> ObtenerAsync(Expression<Func<UnidadOrganizacional, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadOrganizacional>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<UnidadOrganizacional>> ObtenerPaginadoAsync(Expression<Func<UnidadOrganizacional, bool>> predicate = null, Func<IQueryable<UnidadOrganizacional>, IOrderedQueryable<UnidadOrganizacional>> orderBy = null, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UnidadOrganizacional> UnicoAsync(Expression<Func<UnidadOrganizacional, bool>> predicado = null, Func<IQueryable<UnidadOrganizacional>, IOrderedQueryable<UnidadOrganizacional>> ordenarPor = null, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }


    }
}
