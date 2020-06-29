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

    public class ServicioHorarioMedioContacto : ContextoServicioOContacto
        , IServicioInyectable, IServicioHorarioMedioContacto

    {
        private const string DEFAULT_SORT_COL = "DiaSemana";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<HorarioMedioContacto> repo;
        private UnidadDeTrabajo<DbContextContacto> UDT;

        public ServicioHorarioMedioContacto(
         IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones,
         ILogger<ServicioHorarioMedioContacto> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContacto>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<HorarioMedioContacto>(new QueryComposer<HorarioMedioContacto>());

        }

        public async Task<bool> Existe(Expression<Func<HorarioMedioContacto, bool>> predicado)
        {
            List<HorarioMedioContacto> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<HorarioMedioContacto> CrearAsync(HorarioMedioContacto entity, CancellationToken cancellationToken = default)
        {
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }


        public async Task ActualizarAsync(HorarioMedioContacto entity)
        {
            HorarioMedioContacto tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            tmp.DiaSemana = entity.DiaSemana;
            tmp.Fin = entity.Fin;
            tmp.Inicio = entity.Inicio;
            tmp.SinHorario = tmp.SinHorario;
            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
        }
        
        


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            HorarioMedioContacto o;
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


            return listaEliminados;

        }


        public Task<List<HorarioMedioContacto>> ObtenerAsync(Expression<Func<HorarioMedioContacto, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<HorarioMedioContacto> UnicoAsync(Expression<Func<HorarioMedioContacto, bool>> predicado = null, Func<IQueryable<HorarioMedioContacto>, IOrderedQueryable<HorarioMedioContacto>> ordenarPor = null, Func<IQueryable<HorarioMedioContacto>, IIncludableQueryable<HorarioMedioContacto, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            HorarioMedioContacto d = await this.repo.UnicoAsync(predicado);
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

        public async Task<IPaginado<HorarioMedioContacto>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<HorarioMedioContacto>, IIncludableQueryable<HorarioMedioContacto, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

        #region sin implementar

        public Task<IEnumerable<HorarioMedioContacto>> CrearAsync(params HorarioMedioContacto[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<HorarioMedioContacto>> CrearAsync(IEnumerable<HorarioMedioContacto> entities, CancellationToken cancellationToken = default)
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


        public Task<List<HorarioMedioContacto>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<HorarioMedioContacto>> ObtenerPaginadoAsync(Expression<Func<HorarioMedioContacto, bool>> predicate = null, Func<IQueryable<HorarioMedioContacto>, IOrderedQueryable<HorarioMedioContacto>> orderBy = null, Func<IQueryable<HorarioMedioContacto>, IIncludableQueryable<HorarioMedioContacto, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
