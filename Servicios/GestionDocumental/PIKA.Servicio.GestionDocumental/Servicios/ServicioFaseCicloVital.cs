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
    public class ServicioFaseCicloVital : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioFaseCicloVital
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<FaseCicloVital> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioFaseCicloVital(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioFaseCicloVital> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<FaseCicloVital>(new QueryComposer<FaseCicloVital>());
        }

        public async Task<bool> Existe(Expression<Func<FaseCicloVital, bool>> predicado)
        {
            List<FaseCicloVital> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<FaseCicloVital> CrearAsync(FaseCicloVital entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(FaseCicloVital entity)
        {

            FaseCicloVital o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
        public async Task<IPaginado<FaseCicloVital>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<FaseCicloVital>, IIncludableQueryable<FaseCicloVital, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<FaseCicloVital>> CrearAsync(params FaseCicloVital[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FaseCicloVital>> CrearAsync(IEnumerable<FaseCicloVital> entities, CancellationToken cancellationToken = default)
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
            FaseCicloVital f;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                f = await this.repo.UnicoAsync(x => x.Id == Id);
                if (f != null)
                {
                    UDT.Context.Entry(f).State = EntityState.Deleted;
                    listaEliminados.Add(f.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<FaseCicloVital>> ObtenerAsync(Expression<Func<FaseCicloVital, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<FaseCicloVital>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<FaseCicloVital>> ObtenerPaginadoAsync(Expression<Func<FaseCicloVital, bool>> predicate = null, Func<IQueryable<FaseCicloVital>, IOrderedQueryable<FaseCicloVital>> orderBy = null, Func<IQueryable<FaseCicloVital>, IIncludableQueryable<FaseCicloVital, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<FaseCicloVital> UnicoAsync(Expression<Func<FaseCicloVital, bool>> predicado = null, Func<IQueryable<FaseCicloVital>, IOrderedQueryable<FaseCicloVital>> ordenarPor = null, Func<IQueryable<FaseCicloVital>, IIncludableQueryable<FaseCicloVital, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            FaseCicloVital a = await this.repo.UnicoAsync(predicado);
            return a.CopiaFase();
        }
    }
}
