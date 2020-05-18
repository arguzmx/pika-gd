
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioEstadoTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioEstadoTransferencia
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EstadoTransferencia> repo;
        private ICompositorConsulta<EstadoTransferencia> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioEstadoTransferencia(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ICompositorConsulta<EstadoTransferencia> compositorConsulta,
           ILogger<ServicioEstadoTransferencia> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<EstadoTransferencia>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<EstadoTransferencia, bool>> predicado)
        {
            List<EstadoTransferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<EstadoTransferencia> CrearAsync(EstadoTransferencia entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(EstadoTransferencia entity)
        {

            EstadoTransferencia o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
        public async Task<IPaginado<EstadoTransferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EstadoTransferencia>, IIncludableQueryable<EstadoTransferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<EstadoTransferencia>> CrearAsync(params EstadoTransferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EstadoTransferencia>> CrearAsync(IEnumerable<EstadoTransferencia> entities, CancellationToken cancellationToken = default)
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
            EstadoTransferencia a;
            ICollection<string> listaEliminados = new HashSet<string>();
            
            foreach (var Id in ids)
            {
                Console.WriteLine("1");
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    Console.WriteLine("existe" + a.Nombre);
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.Id);
                }
            }
            UDT.SaveChanges();
            Console.WriteLine("elimina");
            return listaEliminados;
        }

        public Task<List<EstadoTransferencia>> ObtenerAsync(Expression<Func<EstadoTransferencia, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EstadoTransferencia>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<EstadoTransferencia>> ObtenerPaginadoAsync(Expression<Func<EstadoTransferencia, bool>> predicate = null, Func<IQueryable<EstadoTransferencia>, IOrderedQueryable<EstadoTransferencia>> orderBy = null, Func<IQueryable<EstadoTransferencia>, IIncludableQueryable<EstadoTransferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<EstadoTransferencia> UnicoAsync(Expression<Func<EstadoTransferencia, bool>> predicado = null, Func<IQueryable<EstadoTransferencia>, IOrderedQueryable<EstadoTransferencia>> ordenarPor = null, Func<IQueryable<EstadoTransferencia>, IIncludableQueryable<EstadoTransferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EstadoTransferencia t = await this.repo.UnicoAsync(predicado);
            return t.CopiaEstadoTransferencia();
        }
    }
}
