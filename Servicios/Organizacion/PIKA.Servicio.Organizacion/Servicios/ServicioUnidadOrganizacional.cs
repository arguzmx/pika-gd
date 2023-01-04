using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioUnidadOrganizacional : 
        ContextoServicioOrganizacion, 
        IServicioInyectable, 
        IServicioUnidadOrganizacional
    {

        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<UnidadOrganizacional> repo;
        
        public ServicioUnidadOrganizacional(
         IAppCache cache,
         IRegistroAuditoria registroAuditoria,
         IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
                  ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_UNIDADORGANIZACIONAL)
        {
            this.repo = UDT.ObtenerRepositoryAsync<UnidadOrganizacional>(
                new QueryComposer<UnidadOrganizacional>());
        }

        public async Task<bool> Existe(Expression<Func<UnidadOrganizacional, bool>> predicado)
        {
            List<UnidadOrganizacional> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<UnidadOrganizacional> CrearAsync(UnidadOrganizacional entity, CancellationToken cancellationToken = default)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();

            }
            seguridad.EstableceDatosProceso<UnidadOrganizacional>();

            await seguridad.IdEnDominio(entity.DominioId);

            // los nombres de unidad organizacional pueden repetirse en los diferetes dominios
            if (await Existe(x =>
            x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase) &&
            x.DominioId.Equals(entity.DominioId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity;

        }


        public async Task ActualizarAsync(UnidadOrganizacional entity)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            seguridad.EstableceDatosProceso<UnidadOrganizacional>();
            UnidadOrganizacional uo = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (uo == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            await seguridad.IdEnDominio(uo.DominioId);
            string original = uo.Flat();

            if (await Existe(x =>
            x.Id != entity.Id && x.Id == entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            uo.Eliminada = entity.Eliminada;
            uo.Nombre = entity.Nombre;
            UDT.Context.Entry(uo).State = EntityState.Modified;

            await seguridad.RegistraEventoActualizar(uo.Id, uo.Nombre, original.JsonDiff(uo.Flat()));
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
                query = new Consulta() { indice = 0, tamano = 20, ord_columna  = DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }

            query.Filtros.RemoveAll(x => x.Propiedad == "DominioId");
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "DominioId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.DominioId });

            return query;
        }
        public async Task<IPaginado<UnidadOrganizacional>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            return respuesta;
        }

        

        public async Task EjecutarSql(string sqlCommand)
        {
            await this.contexto.Database.ExecuteSqlRawAsync(sqlCommand);
        }

        public async Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            foreach (string s in sqlCommand)
            {
                await this.contexto.Database.ExecuteSqlRawAsync(s);
            }
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();

            }
            UnidadOrganizacional uo;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                uo = await this.repo.UnicoAsync(x => x.Id == Id);
                if (uo != null)
                {
                    uo.Eliminada = true;
                    UDT.Context.Entry(uo).State = EntityState.Modified;
                    listaEliminados.Add(uo.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<List<UnidadOrganizacional>> ObtenerAsync(Expression<Func<UnidadOrganizacional, bool>> predicado)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }
            return await this.repo.ObtenerAsync(predicado);
        }

        public async Task<List<UnidadOrganizacional>> ObtenerAsync(string SqlCommand)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }
            return await this.repo.ObtenerAsync(SqlCommand);
        }



        public async Task<UnidadOrganizacional> UnicoAsync(Expression<Func<UnidadOrganizacional, bool>> predicado = null, Func<IQueryable<UnidadOrganizacional>, IOrderedQueryable<UnidadOrganizacional>> ordenarPor = null, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            UnidadOrganizacional uo = await this.repo.UnicoAsync(predicado);
            return uo.CopiaUO();
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }
            UnidadOrganizacional uo;
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids)
            {
                uo = await this.repo.UnicoAsync(x => x.Id == Id);
                if (uo != null)
                {
                    uo.Eliminada = false;
                    UDT.Context.Entry(uo).State = EntityState.Modified;
                    lista.Add(uo.Id);
                }
            }
            UDT.SaveChanges();
            return lista;
        }
        public Task<string[]> Purgar()
        {
            throw new NotImplementedException();
        }

        #region Sin implemntar
        public Task<IEnumerable<UnidadOrganizacional>> CrearAsync(params UnidadOrganizacional[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadOrganizacional>> CrearAsync(IEnumerable<UnidadOrganizacional> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<UnidadOrganizacional>> ObtenerPaginadoAsync(Expression<Func<UnidadOrganizacional, bool>> predicate = null, Func<IQueryable<UnidadOrganizacional>, IOrderedQueryable<UnidadOrganizacional>> orderBy = null, Func<IQueryable<UnidadOrganizacional>, IIncludableQueryable<UnidadOrganizacional, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UnidadOrganizacional> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
