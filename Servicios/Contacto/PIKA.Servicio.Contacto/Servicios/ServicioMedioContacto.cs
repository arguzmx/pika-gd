using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;


namespace PIKA.Servicio.Contacto
{

    public class ServicioMedioContacto : ContextoServicioOContacto
        , IServicioInyectable, IServicioMedioContacto

    {
        private const string DEFAULT_SORT_COL = "TipoMedioId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<MedioContacto> repo;
        private IRepositorioAsync<HorarioMedioContacto> repoHorario;
        private UnidadDeTrabajo<DbContextContacto> UDT;

        public ServicioMedioContacto(
         IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones,
         ILogger<ServicioMedioContacto> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContacto>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<MedioContacto>(new QueryComposer<MedioContacto>());
        }

        public async Task<bool> Existe(Expression<Func<MedioContacto, bool>> predicado)
        {
            List<MedioContacto> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<MedioContacto> CrearAsync(MedioContacto entity, CancellationToken cancellationToken = default)
        {

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity ?? entity.Copia();
        }


        public async Task ActualizarAsync(MedioContacto entity)
        {
            MedioContacto tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            tmp.Activo = entity.Activo;
            tmp.Notas  = entity.Notas;
            tmp.Prefijo  = entity.Prefijo;
            tmp.Principal  = entity.Principal;
            tmp.Sufijo = entity.Sufijo;
            tmp.TipoFuenteContactoId = entity.TipoFuenteContactoId;
            tmp.TipoMedioId = entity.TipoMedioId;

            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
        }
        
        


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            MedioContacto o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    try
                    {
                        await this.repo.Eliminar(o);
                        listaEliminados.Add(o.Id);
                    }
                    catch (Exception)
                    {}
                }
            }
            
            this.UDT.SaveChanges();

            return listaEliminados;

        }


        public Task<List<MedioContacto>> ObtenerAsync(Expression<Func<MedioContacto, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<MedioContacto> UnicoAsync(Expression<Func<MedioContacto, bool>> predicado = null, Func<IQueryable<MedioContacto>, IOrderedQueryable<MedioContacto>> ordenarPor = null, Func<IQueryable<MedioContacto>, IIncludableQueryable<MedioContacto, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            MedioContacto d = await this.repo.UnicoAsync(predicado);
            return d.Copia();
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

        public async Task<IPaginado<MedioContacto>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<MedioContacto>, IIncludableQueryable<MedioContacto, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }


        #region sin implementar

        public Task<IEnumerable<MedioContacto>> CrearAsync(params MedioContacto[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MedioContacto>> CrearAsync(IEnumerable<MedioContacto> entities, CancellationToken cancellationToken = default)
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


        public Task<List<MedioContacto>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<MedioContacto>> ObtenerPaginadoAsync(Expression<Func<MedioContacto, bool>> predicate = null, Func<IQueryable<MedioContacto>, IOrderedQueryable<MedioContacto>> orderBy = null, Func<IQueryable<MedioContacto>, IIncludableQueryable<MedioContacto, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
