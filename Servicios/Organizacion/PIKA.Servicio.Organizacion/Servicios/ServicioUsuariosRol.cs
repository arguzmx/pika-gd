﻿using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioUsuariosRol : ContextoServicioOrganizacion,
        IServicioInyectable, IServicioUsuariosRol
    {
        private const string DEFAULT_SORT_COL = "ApplicationUserId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<UsuariosRol> repo;
        
        public ServicioUsuariosRol(
         IAppCache cache,
         IRegistroAuditoria registroAuditoria,
         IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
                  ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_ROL)
        {
            this.repo = UDT.ObtenerRepositoryAsync<UsuariosRol>(
                new QueryComposer<UsuariosRol>());
        }


        public async Task<bool> Existe(Expression<Func<UsuariosRol, bool>> predicado)
        {
            seguridad.EstableceDatosProceso<UsuariosRol>();
            List<UsuariosRol> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<UsuariosRol> CrearAsync(UsuariosRol entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<UsuariosRol>();
            Rol r = UDT.Context.Roles.FirstOrDefault(x => x.Id == entity.RolId);
            if(r==null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.IdEnDominio(r.OrigenId);

            if (!(await Existe(x => x.ApplicationUserId == entity.ApplicationUserId && x.RolId == entity.RolId)))
            {
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            }

            await seguridad.RegistraEventoCrear(entity.ApplicationUserId, r.Nombre);

            return entity;
        }

  
        /// <summary>
        ///  Añade una lista de ids de usuarios al rol
        /// </summary>
        /// <param name="rolid"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<int> PostIds(string rolid, string[] ids)
        {
            seguridad.EstableceDatosProceso<UsuariosRol>();
            Rol r = UDT.Context.Roles.FirstOrDefault(x => x.Id == rolid);
            if (r == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.IdEnDominio(r.OrigenId);

            int total= 0;
            foreach (string id in ids)
            {
                if (!(await Existe(x => x.ApplicationUserId == id && x.RolId == rolid)))
                {
                    await this.repo.CrearAsync(new UsuariosRol() { ApplicationUserId = id, RolId = rolid });
                    await seguridad.RegistraEventoCrear(id, r.Nombre);
                }
            }
            UDT.SaveChanges();
            return total;
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
        public async Task<IPaginado<UsuariosRol>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<UsuariosRol>, IIncludableQueryable<UsuariosRol, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<UsuariosRol>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

        public async Task<ICollection<string>> DeleteIds(string rolId, string[] ids)
        {
            seguridad.EstableceDatosProceso<UsuariosRol>();
            Rol r = UDT.Context.Roles.FirstOrDefault(x => x.Id == rolId);
            if (r == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.IdEnDominio(r.OrigenId);


            List<string> l = new List<string>();
           foreach(string id in ids)
            {
                UsuariosRol ur = await repo.UnicoAsync(x => x.RolId == rolId && x.ApplicationUserId == id);
                if (ur!=null)
                {
                    this.UDT.Context.Entry(ur).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(id, r.Nombre);
                    l.Add(id);
                }
            }
            this.UDT.SaveChanges();

            return l;
        }


        public async Task<List<string>> IdentificadoresRolesUsuario(string uid)
        {
            var roles = await repo.ObtenerAsync(x =>
            x.ApplicationUserId.Equals(uid, StringComparison.InvariantCultureIgnoreCase));
            return roles.ToList().Select(x=> x.RolId ).ToList();
        }



        #region sin implemetar

        public Task<List<UsuariosRol>> ObtenerAsync(Expression<Func<UsuariosRol, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task ActualizarAsync(UsuariosRol entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<UsuariosRol>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<UsuariosRol>> ObtenerPaginadoAsync(Expression<Func<UsuariosRol, bool>> predicate = null, Func<IQueryable<UsuariosRol>, IOrderedQueryable<UsuariosRol>> orderBy = null, Func<IQueryable<UsuariosRol>, IIncludableQueryable<UsuariosRol, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<UsuariosRol> UnicoAsync(Expression<Func<UsuariosRol, bool>> predicado = null, Func<IQueryable<UsuariosRol>, IOrderedQueryable<UsuariosRol>> ordenarPor = null, Func<IQueryable<UsuariosRol>, IIncludableQueryable<UsuariosRol, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UsuariosRol>> CrearAsync(params UsuariosRol[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UsuariosRol>> CrearAsync(IEnumerable<UsuariosRol> entities, CancellationToken cancellationToken = default)
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

        public Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<UsuariosRol> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
