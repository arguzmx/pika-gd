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
  public  class ServicioVolumen : ContextoServicioContenido,
        IServicioInyectable, IServicioVolumen
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Volumen> repo;
        private ICompositorConsulta<Volumen> compositor;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioVolumen(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ICompositorConsulta<Volumen> compositorConsulta,
        ILogger<ServicioVolumen> Logger,
        IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Volumen>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<Volumen, bool>> predicado)
        {
            List<Volumen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Volumen> CrearAsync(Volumen entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(Volumen entity)
        {

            Volumen o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
        public async Task<IPaginado<Volumen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Volumen>> CrearAsync(params Volumen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Volumen>> CrearAsync(IEnumerable<Volumen> entities, CancellationToken cancellationToken = default)
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
            Volumen d;
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

        public Task<List<Volumen>> ObtenerAsync(Expression<Func<Volumen, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Volumen>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Volumen>> ObtenerPaginadoAsync(Expression<Func<Volumen, bool>> predicate = null, Func<IQueryable<Volumen>, IOrderedQueryable<Volumen>> orderBy = null, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Volumen> UnicoAsync(Expression<Func<Volumen, bool>> predicado = null, Func<IQueryable<Volumen>, IOrderedQueryable<Volumen>> ordenarPor = null, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Volumen d = await this.repo.UnicoAsync(predicado);

            return d.CopiaVolumen();
        }

    }



}
