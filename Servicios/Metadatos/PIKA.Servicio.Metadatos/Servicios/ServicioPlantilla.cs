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
    public class ServicioPlantilla : ContextoServicioMetadatos,IServicioInyectable, IServicioPlantilla
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Plantilla> repo;
        private ICompositorConsulta<Plantilla> compositor;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        public ServicioPlantilla(
          IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
          ICompositorConsulta<Plantilla> compositorConsulta,
          ILogger<ServicioPlantilla> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Plantilla>(compositor);
        }
        public async Task<bool> Existe(Expression<Func<Plantilla, bool>> predicado)
        {
            List<Plantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Plantilla> CrearAsync(Plantilla entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada!=true ))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            try
            {
                entity.Id = System.Guid.NewGuid().ToString();
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional(entity.AlmacenDatosId);
            }
            catch (Exception)
            {
                throw;
            }
        
            return entity.Copia();
        }

        public async Task ActualizarAsync(Plantilla entity)
        {

            Plantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada!=true))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            try
            {
                o.Nombre = entity.Nombre.Trim();
                o.AlmacenDatosId = entity.AlmacenDatosId.Trim();

                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional(entity.AlmacenDatosId);
            }
            catch (Exception)
            {
                throw;
            }
           

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
        public async Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Plantilla p;
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

        public async Task<Plantilla> UnicoAsync(Expression<Func<Plantilla, bool>> predicado = null, 
            Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> ordenarPor = null, 
            Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> incluir = null, 
            bool inhabilitarSegumiento = true)
        {

            Plantilla d = await this.repo.UnicoAsync(predicado,ordenarPor, incluir);

            return d.Copia();
        }
        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Plantilla c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    if(await Existe(x => x.Id != c.Id && x.Nombre.Equals(c.Nombre,StringComparison.InvariantCultureIgnoreCase)&&x.Eliminada==false))
                    c.Nombre = $"{c.Nombre} Restaurado {DateTime.Now.Ticks}";

                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }

            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public Task<List<Plantilla>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public  Task<List<Plantilla>> ObtenerAsync(Expression<Func<Plantilla, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        #region Sin Implementar
        public Task<IEnumerable<Plantilla>> CrearAsync(params Plantilla[] entities)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Plantilla>> CrearAsync(IEnumerable<Plantilla> entities, CancellationToken cancellationToken = default)
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
       
        
        public Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Expression<Func<Plantilla, bool>> predicate = null, Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> orderBy = null, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        
        #endregion


    }
}
