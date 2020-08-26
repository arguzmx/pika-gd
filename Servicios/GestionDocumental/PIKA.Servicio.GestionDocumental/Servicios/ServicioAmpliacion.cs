using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    public class ServicioAmpliacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioAmpliacion
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Ampliacion> repo;
        private IRepositorioAsync<TipoAmpliacion> repoTipo;
        private IRepositorioAsync<Activo> repoActivo;

        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioAmpliacion(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioAmpliacion> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Ampliacion>(new QueryComposer<Ampliacion>());
            this.repoTipo = UDT.ObtenerRepositoryAsync<TipoAmpliacion>(new QueryComposer<TipoAmpliacion>());
            this.repoActivo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
        }

        public async Task<bool> Existe(Expression<Func<Ampliacion, bool>> predicado)
        {
            List<Ampliacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteTipoAmpliacion(Expression<Func<TipoAmpliacion, bool>> predicado)
        {
            List<TipoAmpliacion> l = await this.repoTipo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteActivo(Expression<Func<Activo, bool>> predicado)
        {
            List<Activo> l = await this.repoActivo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        private async Task<Ampliacion> ValidaAmpliacion(Ampliacion entity , bool isUpdate)
        {
            if (isUpdate)
            {
        
            }

            if (!await ExisteTipoAmpliacion(x => x.Id.Equals(entity.TipoAmpliacionId.Trim(), StringComparison.InvariantCulture)))
                throw new ExDatosNoValidos(entity.TipoAmpliacionId.Trim());

            if (!await ExisteActivo(x => x.Id.Equals(entity.ActivoId.Trim(), StringComparison.InvariantCulture)))
                throw new ExDatosNoValidos(entity.ActivoId);
            
            if (!entity.Inicio.HasValue) throw new ExDatosNoValidos("Inicio");

            if (entity.FechaFija)
            {
 
                if (!entity.Fin.HasValue) throw new ExDatosNoValidos("Fin");
                entity.Dias = null;
                entity.Anos = null;
                entity.Meses = null;
                entity.Vigente = (DateTime.Now.Ticks < entity.Fin.Value.Ticks);
            }
            else
            {
                if (entity.Dias.HasValue || entity.Anos.HasValue || entity.Meses.HasValue)
                {
                    DateTime f = entity.Inicio.Value;

                    if (entity.Dias.HasValue) f.AddDays(entity.Dias.Value);
                    if (entity.Anos.HasValue) f.AddDays(entity.Anos.Value);
                    if (entity.Meses.HasValue) f.AddDays(entity.Meses.Value);

                    entity.Vigente = (DateTime.Now.Ticks < f.Ticks);

                } else
                {
                    if (!entity.Fin.HasValue) throw new ExDatosNoValidos("Periodo requerido");
                }
            }

            return entity;
        }

        public async Task<Ampliacion> CrearAsync(Ampliacion entity, CancellationToken cancellationToken = default)
        {

            entity = await ValidaAmpliacion(entity, false);

            entity.Id = System.Guid.NewGuid().ToString();
            entity.ActivoId = entity.ActivoId.Trim();
            entity.TipoAmpliacionId = entity.TipoAmpliacionId.Trim();
            entity.FundamentoLegal = entity.FundamentoLegal.Trim();

         

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }
        public async Task ActualizarAsync(Ampliacion entity)
        {

            entity = await ValidaAmpliacion(entity, true);

            Ampliacion o = await this.repo.UnicoAsync(x=>x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            

            o.Vigente = entity.Vigente;
            o.TipoAmpliacionId = entity.TipoAmpliacionId.Trim();
            o.FechaFija = entity.FechaFija;
            o.FechaFija = entity.FechaFija;
            o.FundamentoLegal = entity.FundamentoLegal.Trim();
            o.Inicio = entity.Inicio;
            o.Fin = entity.Fin;
            o.Anos = entity.Anos;
            o.Meses = entity.Meses;
            o.Dias = entity.Dias;

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
        public async Task<IPaginado<Ampliacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Ampliacion>, IIncludableQueryable<Ampliacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Ampliacion o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            await this.repo.Eliminar(o);
                            listaEliminados.Add(o.ActivoId);
                        }
                        this.UDT.SaveChanges();
                        
                    }
                    catch (DbUpdateException)
                    {
                        throw new ExErrorRelacional(Id);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            UDT.SaveChanges();

            return listaEliminados;

        }
        public Task<List<Ampliacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<Ampliacion>> ObtenerAsync(Expression<Func<Ampliacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        public async Task<Ampliacion> UnicoAsync(Expression<Func<Ampliacion, bool>> predicado = null, Func<IQueryable<Ampliacion>, IOrderedQueryable<Ampliacion>> ordenarPor = null, Func<IQueryable<Ampliacion>, IIncludableQueryable<Ampliacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Ampliacion a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        #region Sin implementar
        public Task<IEnumerable<Ampliacion>> CrearAsync(params Ampliacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ampliacion>> CrearAsync(IEnumerable<Ampliacion> entities, CancellationToken cancellationToken = default)
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
        public Task<IPaginado<Ampliacion>> ObtenerPaginadoAsync(Expression<Func<Ampliacion, bool>> predicate = null, Func<IQueryable<Ampliacion>, IOrderedQueryable<Ampliacion>> orderBy = null, Func<IQueryable<Ampliacion>, IIncludableQueryable<Ampliacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
