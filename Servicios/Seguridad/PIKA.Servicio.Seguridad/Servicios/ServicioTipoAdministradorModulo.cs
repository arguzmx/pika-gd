﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Servicios
{
       public class ServicioTipoAdministradorModulo : ContextoServicioSeguridad
      , IServicioInyectable, IServicioTipoAdministradorModulo

    {
        private const string DEFAULT_SORT_COL = "ModuloId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoAdministradorModulo> repo;

        public ServicioTipoAdministradorModulo(
              IAppCache cache,
         IRegistroAuditoria registroAuditoria,
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
                  ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_AUDITORIA)
        {
            this.repo = UDT.ObtenerRepositoryAsync<TipoAdministradorModulo>(
                new QueryComposer<TipoAdministradorModulo>());
        }

        public async Task<bool> Existe(Expression<Func<TipoAdministradorModulo, bool>> predicado)
        {
            List<TipoAdministradorModulo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoAdministradorModulo> CrearAsync(TipoAdministradorModulo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.AplicacionId.Equals(entity.AplicacionId, StringComparison.InvariantCultureIgnoreCase)
            && x.ModuloId.Equals(entity.ModuloId,StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.AplicacionId);
            }
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();


            return entity ?? entity.Copia();
        }

        public async Task ActualizarAsync(TipoAdministradorModulo entity)
        {

            TipoAdministradorModulo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }


            if (await Existe(x =>
            x.Id != entity.Id
            && x.AplicacionId.Equals(entity.AplicacionId, StringComparison.InvariantCultureIgnoreCase) 
            && x.ModuloId.Equals(entity.ModuloId,StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.AplicacionId);
            }

            o.AplicacionId = entity.AplicacionId;
            o.ModuloId = entity.ModuloId;

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
        public async Task<IPaginado<TipoAdministradorModulo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoAdministradorModulo>, IIncludableQueryable<TipoAdministradorModulo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

      

        public Task<List<TipoAdministradorModulo>> ObtenerAsync(Expression<Func<TipoAdministradorModulo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<TipoAdministradorModulo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

       


       

        public async Task<TipoAdministradorModulo> UnicoAsync(Expression<Func<TipoAdministradorModulo, bool>> predicado = null, Func<IQueryable<TipoAdministradorModulo>, IOrderedQueryable<TipoAdministradorModulo>> ordenarPor = null, Func<IQueryable<TipoAdministradorModulo>, IIncludableQueryable<TipoAdministradorModulo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            TipoAdministradorModulo d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }
        #region sin implementar 
        public Task<IEnumerable<TipoAdministradorModulo>> CrearAsync(params TipoAdministradorModulo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAdministradorModulo>> CrearAsync(IEnumerable<TipoAdministradorModulo> entities, CancellationToken cancellationToken = default)
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
            TipoAdministradorModulo o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            await this.repo.Eliminar(o);
                        }
                        this.UDT.SaveChanges();
                        listaEliminados.Add(o.Id);
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
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        public Task<IPaginado<TipoAdministradorModulo>> ObtenerPaginadoAsync(Expression<Func<TipoAdministradorModulo, bool>> predicate = null, Func<IQueryable<TipoAdministradorModulo>, IOrderedQueryable<TipoAdministradorModulo>> orderBy = null, Func<IQueryable<TipoAdministradorModulo>, IIncludableQueryable<TipoAdministradorModulo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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
