using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public class ServicioAtributoTabla : ContextoServicioMetadatos, IServicioInyectable, IServicioAtributoTabla
    {
        private const string DEFAULT_SORT_COL = "propiedadId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<AtributoTabla> repo;
        private ICompositorConsulta<AtributoTabla> compositor;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioAtributoTabla(
           IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
           ICompositorConsulta<AtributoTabla> compositorConsulta,
           ILogger<ServicioAtributoTabla> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<AtributoTabla>(compositor);
        }






        public async Task<bool> Existe(Expression<Func<AtributoTabla, bool>> predicado)
        {
            List<AtributoTabla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<AtributoTabla> CrearAsync(AtributoTabla entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(AtributoTabla entity)
        {

            AtributoTabla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Id.Equals(entity.Id, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Id);
            }

            o.PropiedadId = entity.PropiedadId;
            o.Alternable = entity.Alternable;
            o.Incluir = entity.Incluir;
            o.IndiceOrdebnamiento = entity.IndiceOrdebnamiento;
            o.Propiedad = entity.Propiedad;
            o.Visible = entity.Visible;

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
        public async Task<IPaginado<AtributoTabla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AtributoTabla>, IIncludableQueryable<AtributoTabla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<AtributoTabla>> CrearAsync(params AtributoTabla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AtributoTabla>> CrearAsync(IEnumerable<AtributoTabla> entities, CancellationToken cancellationToken = default)
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
            AtributoTabla at;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                at = await this.repo.UnicoAsync(x => x.Id == Id);
                if (at != null)
                {
                    UDT.Context.Entry(at).State = EntityState.Deleted;
                    listaEliminados.Add(at.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<AtributoTabla>> ObtenerAsync(Expression<Func<AtributoTabla, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<AtributoTabla>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<AtributoTabla>> ObtenerPaginadoAsync(Expression<Func<AtributoTabla, bool>> predicate = null, Func<IQueryable<AtributoTabla>, IOrderedQueryable<AtributoTabla>> orderBy = null, Func<IQueryable<AtributoTabla>, IIncludableQueryable<AtributoTabla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<AtributoTabla> UnicoAsync(Expression<Func<AtributoTabla, bool>> predicado = null, Func<IQueryable<AtributoTabla>, IOrderedQueryable<AtributoTabla>> ordenarPor = null, Func<IQueryable<AtributoTabla>, IIncludableQueryable<AtributoTabla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            AtributoTabla d = await this.repo.UnicoAsync(predicado);

            return d.CopiaAtributoTabla();
        }
    }
}
