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
    public class ServicioAlmacenArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioAlmacenArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<AlmacenArchivo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioAlmacenArchivo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioAlmacenArchivo> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<AlmacenArchivo>(new QueryComposer<AlmacenArchivo>());
        }

        public async Task<bool> Existe(Expression<Func<AlmacenArchivo, bool>> predicado)
        {
            List<AlmacenArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<AlmacenArchivo> CrearAsync(AlmacenArchivo entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(AlmacenArchivo entity)
        {

            AlmacenArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.ArchivoId == entity.ArchivoId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Clave= entity.Clave;

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
        public async Task<IPaginado<AlmacenArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AlmacenArchivo>, IIncludableQueryable<AlmacenArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<AlmacenArchivo>> CrearAsync(params AlmacenArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AlmacenArchivo>> CrearAsync(IEnumerable<AlmacenArchivo> entities, CancellationToken cancellationToken = default)
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
            AlmacenArchivo a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<AlmacenArchivo>> ObtenerAsync(Expression<Func<AlmacenArchivo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<AlmacenArchivo>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<AlmacenArchivo>> ObtenerPaginadoAsync(Expression<Func<AlmacenArchivo, bool>> predicate = null, Func<IQueryable<AlmacenArchivo>, IOrderedQueryable<AlmacenArchivo>> orderBy = null, Func<IQueryable<AlmacenArchivo>, IIncludableQueryable<AlmacenArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<AlmacenArchivo> UnicoAsync(Expression<Func<AlmacenArchivo, bool>> predicado = null, Func<IQueryable<AlmacenArchivo>, IOrderedQueryable<AlmacenArchivo>> ordenarPor = null, Func<IQueryable<AlmacenArchivo>, IIncludableQueryable<AlmacenArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            AlmacenArchivo a = await this.repo.UnicoAsync(predicado);
            return a.CopiaAlamacen();
        }
    }
}

