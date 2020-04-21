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


namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioDominio : ContextoServicioOrganizacion,
        IServicioInyectable, IServicioDominio
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Dominio> repo;
        private ICompositorConsulta<Dominio> compositor;
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;

        public ServicioDominio(
            IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
        ICompositorConsulta<Dominio> compositorConsulta,
        ILogger<ServicioDominio> Logger,
        IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Dominio>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<Dominio, bool>> predicado)
        {
            List<Dominio> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Dominio> CrearAsync(Dominio entity, CancellationToken cancellationToken = default)
        {

            if( await  Existe(x=>x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(Dominio entity)
        {
            
            Dominio o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            
            if (await Existe(x => 
            x.Id!=entity.Id & x.TipoOrigenId ==entity.TipoOrigenId && x.OrigenId==entity.OrigenId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
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
        public async Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Dominio>> CrearAsync(params Dominio[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dominio>> CrearAsync(IEnumerable<Dominio> entities, CancellationToken cancellationToken = default)
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

        public async Task Eliminar(string[] ids)
        {
            Dominio d;
            IEnumerable<Dominio> domError = new HashSet<Dominio>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d == null)
                {
                    domError.Append(d);
                    throw new EXNoEncontrado();
                }

                UDT.Context.Entry(d).State = EntityState.Deleted;
            }
            UDT.SaveChanges();
        }   

        public Task<List<Dominio>> ObtenerAsync(Expression<Func<Dominio, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dominio>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Expression<Func<Dominio, bool>> predicate = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> orderBy = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

  

        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Dominio> UnicoAsync(Expression<Func<Dominio, bool>> predicado = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> ordenarPor = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Dominio d = await this.repo.UnicoAsync(predicado);

            //Cuando llamas a serializar objetos con propiedades de navegación pueden crearse referencias circulares
            //por eso devolvemos una instancia filtrada

            //Hayq que implmenbatrlo como un método de extensión en ExtensionesDominio
            //estudiar https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

            return d.CopiaDominio();
        }



    }

  

}
