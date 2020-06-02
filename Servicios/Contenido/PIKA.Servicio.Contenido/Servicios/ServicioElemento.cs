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
    public class ServicioElemento : ContextoServicioContenido,
        IServicioInyectable, IServicioElemento
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Elemento> repo;
        private ICompositorConsulta<Elemento> compositor;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioElemento(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ICompositorConsulta<Elemento> compositorConsulta,
        ILogger<ServicioElemento> Logger,
        IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Elemento>(compositor);
        }


        public async Task<bool> Existe(Expression<Func<Elemento, bool>> predicado)
        {
            List<Elemento> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Elemento> CrearAsync(Elemento entity, CancellationToken cancellationToken = default)
        {

            //if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            //{
            //    throw new ExElementoExistente(entity.Nombre);
            //}

            entity.Id = System.Guid.NewGuid().ToString();
           
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(Elemento entity)
        {

            Elemento o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.TipoOrigenId == entity.TipoOrigenId && x.OrigenId == entity.OrigenId
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
        public async Task<IPaginado<Elemento>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Elemento>, IIncludableQueryable<Elemento, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            
            return respuesta;
        }

        public Task<IEnumerable<Elemento>> CrearAsync(params Elemento[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Elemento>> CrearAsync(IEnumerable<Elemento> entities, CancellationToken cancellationToken = default)
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
            Elemento d;
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

        public Task<List<Elemento>> ObtenerAsync(Expression<Func<Elemento, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Elemento>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Elemento>> ObtenerPaginadoAsync(Expression<Func<Elemento, bool>> predicate = null, Func<IQueryable<Elemento>, IOrderedQueryable<Elemento>> orderBy = null, Func<IQueryable<Elemento>, IIncludableQueryable<Elemento, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Elemento> UnicoAsync(Expression<Func<Elemento, bool>> predicado = null, Func<IQueryable<Elemento>, IOrderedQueryable<Elemento>> ordenarPor = null, Func<IQueryable<Elemento>, IIncludableQueryable<Elemento, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Elemento d = await this.repo.UnicoAsync(predicado);

            return d.CopiaElemento();
        }

    }
}
