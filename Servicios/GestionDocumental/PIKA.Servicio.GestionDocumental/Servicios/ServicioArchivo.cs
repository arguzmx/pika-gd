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
    public class ServicioArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Archivo> repo;
        private ICompositorConsulta<Archivo> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioArchivo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ICompositorConsulta<Archivo> compositorConsulta,
           ILogger<ServicioArchivo> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Archivo>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Archivo> CrearAsync(Archivo entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(Archivo entity)
        {

            Archivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.Eliminada = entity.Eliminada;
            o.TipoArchivoId = o.TipoArchivoId;

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
        public async Task<IPaginado<Archivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Archivo>> CrearAsync(params Archivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Archivo>> CrearAsync(IEnumerable<Archivo> entities, CancellationToken cancellationToken = default)
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
            Archivo a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    a.Eliminada = true;
                    UDT.Context.Entry(a).State = EntityState.Modified;
                    listaEliminados.Add(a.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<Archivo>> ObtenerAsync(Expression<Func<Archivo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Archivo>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Archivo>> ObtenerPaginadoAsync(Expression<Func<Archivo, bool>> predicate = null, Func<IQueryable<Archivo>, IOrderedQueryable<Archivo>> orderBy = null, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<Archivo> UnicoAsync(Expression<Func<Archivo, bool>> predicado = null, Func<IQueryable<Archivo>, IOrderedQueryable<Archivo>> ordenarPor = null, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}
