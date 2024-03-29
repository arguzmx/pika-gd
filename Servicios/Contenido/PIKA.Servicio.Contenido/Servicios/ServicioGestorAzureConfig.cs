﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using GestorAzureConfig = PIKA.Modelo.Contenido.GestorAzureConfig;
namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioGestorAzureConfig : ContextoServicioContenido,
        IServicioInyectable, IServicioGestorAzureConfig
    {
        private const string DEFAULT_SORT_COL = "VolumenId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<GestorAzureConfig> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        public ServicioGestorAzureConfig(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION)
        {
            this.repo = UDT.ObtenerRepositoryAsync<GestorAzureConfig>( new QueryComposer<GestorAzureConfig>());
        }

        public async Task<bool> Existe(Expression<Func<GestorAzureConfig, bool>> predicado)
        {
            List<GestorAzureConfig> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<GestorAzureConfig> CrearAsync(GestorAzureConfig entity, CancellationToken cancellationToken = default)
        {
            try
            {
                GestorAzureConfig o = await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);
                if (o == null)
                {

                    await this.repo.CrearAsync(entity);
                    UDT.SaveChanges();

                } else
                {
                    await ActualizarAsync(entity);
                }
                

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("Alguno de los identificadores no es válido");
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public async Task ActualizarAsync(GestorAzureConfig entity)
        {
            GestorAzureConfig o =  await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.VolumenId);
                }
            o.Endpoint = entity.Endpoint;
            o.Usuario = entity.Usuario;
            o.VolumenId = entity.VolumenId;
            o.Contrasena = entity.Contrasena;

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
        public async Task<IPaginado<GestorAzureConfig>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<GestorAzureConfig>, IIncludableQueryable<GestorAzureConfig, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            GestorAzureConfig d;
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.VolumenId == Id);
                if (d != null)
                {
                    lista.Add(Id);
                    await repo.Eliminar(d);
                 }
            }

            UDT.SaveChanges();
            return lista;
        }

       public async Task<GestorAzureConfig> UnicoAsync(Expression<Func<GestorAzureConfig, bool>> predicado = null, Func<IQueryable<GestorAzureConfig>, IOrderedQueryable<GestorAzureConfig>> ordenarPor = null, Func<IQueryable<GestorAzureConfig>, IIncludableQueryable<GestorAzureConfig, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            var o = await this.repo.UnicoAsync(predicado);

            return o.Copia();
        }

        #region No Implementado


        public Task<List<GestorAzureConfig>> ObtenerAsync(Expression<Func<GestorAzureConfig, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task<List<GestorAzureConfig>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<GestorAzureConfig>> CrearAsync(params GestorAzureConfig[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GestorAzureConfig>> CrearAsync(IEnumerable<GestorAzureConfig> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<GestorAzureConfig>> ObtenerPaginadoAsync(Expression<Func<GestorAzureConfig, bool>> predicate = null, Func<IQueryable<GestorAzureConfig>, IOrderedQueryable<GestorAzureConfig>> orderBy = null, Func<IQueryable<GestorAzureConfig>, IIncludableQueryable<GestorAzureConfig, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<GestorAzureConfig> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion





    }



}