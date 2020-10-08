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
    public class ServicioPrestamo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioPrestamo
    {
        private const string DEFAULT_SORT_COL = "FechaCreacion";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Prestamo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private ILogger<ServicioActivoPrestamo> lap;
        private ILogger<ServicioComentarioPrestamo> lp;
        
        public ServicioPrestamo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioPrestamo> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Prestamo>(new QueryComposer<Prestamo>());
        }

        public async Task<bool> Existe(Expression<Func<Prestamo, bool>> predicado)
        {
            List<Prestamo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Prestamo> CrearAsync(Prestamo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Folio.Equals(entity.Folio, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Folio);
            }

            entity.Id = Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(Prestamo entity)
        {

            Prestamo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.Folio == entity.Folio))
            {
                throw new ExElementoExistente(entity.Folio);
            }

            o.Folio = entity.Folio;
            o.Eliminada = entity.Eliminada;
            o.FechaProgramadaDevolucion = entity.FechaProgramadaDevolucion;
            o.FechaDevolucion = entity.FechaDevolucion;
            o.TieneDevolucionesParciales = entity.TieneDevolucionesParciales;

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
        public async Task<IPaginado<Prestamo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Prestamo>, IIncludableQueryable<Prestamo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Prestamo>> CrearAsync(params Prestamo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Prestamo>> CrearAsync(IEnumerable<Prestamo> entities, CancellationToken cancellationToken = default)
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
            Prestamo p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {
                    p.Eliminada = true;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                    listaEliminados.Add(p.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public async Task<ICollection<string>> EliminarPrestamo(string[] ids)
        {
            Prestamo p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {
                    p.Eliminada = true;
                    UDT.Context.Entry(p).State = EntityState.Deleted;
                    listaEliminados.Add(p.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public Task<List<Prestamo>> ObtenerAsync(Expression<Func<Prestamo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Prestamo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Prestamo>> ObtenerPaginadoAsync(Expression<Func<Prestamo, bool>> predicate = null, Func<IQueryable<Prestamo>, IOrderedQueryable<Prestamo>> orderBy = null, Func<IQueryable<Prestamo>, IIncludableQueryable<Prestamo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Prestamo> UnicoAsync(Expression<Func<Prestamo, bool>> predicado = null, Func<IQueryable<Prestamo>, IOrderedQueryable<Prestamo>> ordenarPor = null, Func<IQueryable<Prestamo>, IIncludableQueryable<Prestamo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Prestamo p = await this.repo.UnicoAsync(predicado);
            return p.Copia();
        }

        public async Task<List<string>> Purgar()
        {
            List<Prestamo> ListaPrestamo = await this.repo.ObtenerAsync(x=>x.Eliminada==true);
            if (ListaPrestamo.Count > 0)
            {
                ServicioActivoPrestamo sap = new ServicioActivoPrestamo(this.proveedorOpciones, lap);
                ServicioComentarioPrestamo scp = new ServicioComentarioPrestamo(this.proveedorOpciones, lp);
                string[] IdPrestamoeliminados = ListaPrestamo.Select(x => x.Id).ToArray();
                await sap.EliminarActivosPrestamos(1, IdPrestamoeliminados);
                List<ComentarioPrestamo> cp = await scp.ObtenerAsync(x => x.PrestamoId.Contains(ListaPrestamo.Select(x => x.Id).FirstOrDefault()));
                string[] ComentariosPrestamosId = cp.Select(x => x.Id).ToArray();
                scp = new ServicioComentarioPrestamo(this.proveedorOpciones,lp);
                await scp.Eliminar(ComentariosPrestamosId);
            }
            return ListaPrestamo.Select(x=>x.Id).ToList();
        }
    }
}
