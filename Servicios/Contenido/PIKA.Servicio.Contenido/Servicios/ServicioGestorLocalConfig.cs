using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Gestores;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using GestorLocalConfig = PIKA.Modelo.Contenido.GestorLocalConfig;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioGestorLocalConfig : ContextoServicioContenido,
        IServicioInyectable, IServicioGestorLocalConfig
    {
        private const string DEFAULT_SORT_COL = "VolumenId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<GestorLocalConfig> repo;
        private IRepositorioAsync<Volumen> repoVol;
        private IOptions<ConfiguracionServidor> configServer;
        private IConfiguration configuration;
        public ServicioGestorLocalConfig(
            IConfiguration configuration,
            IOptions<ConfiguracionServidor> opciones,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION)
        {
            this.configuration = configuration;
            this.configServer = opciones;
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<GestorLocalConfig>( new QueryComposer<GestorLocalConfig>());
            this.repoVol = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
        }

        public async Task<bool> Existe(Expression<Func<GestorLocalConfig, bool>> predicado)
        {
            List<GestorLocalConfig> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        private async Task ValidaEntidad(GestorLocalConfig entity)
        {
            Volumen v = await repoVol.UnicoAsync(x => x.Id == entity.VolumenId);
            if (v == null)
            {
                throw new ExDatosNoValidos($"VolumenId: {entity.VolumenId}");
            }

            if (string.IsNullOrEmpty(entity.Ruta))
            {
                throw new ExDatosNoValidos($"Ruta: {entity.Ruta}");
            }

            GestorLocal g = new GestorLocal(this.logger,  entity, configuration, configServer);
            if (!g.ConexionValida() )
            {
                throw new ExDatosNoValidos($"Sin acceso a ruta: {entity.Ruta}");
            }

        }

        public async Task<GestorLocalConfig> CrearAsync(GestorLocalConfig entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<GestorLocalConfig>();

            GestorLocalConfig o = await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);
            if (o == null)
            {
            
                if(!await seguridad.AccesoCacheVolumen(o.VolumenId))
                {
                    await seguridad.EmiteDatosSesionIncorrectos(entity.VolumenId);
                }

                await ValidaEntidad(entity);
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
                await ActualizaEstadoVolumen(entity.VolumenId, true);
            }
            else
            {
                await ActualizarAsync(entity);
            }


            return entity.Copia();
        }

        private async Task ActualizaEstadoVolumen(string Id, bool configuracionValida)
        {
            var v = await this.repoVol.UnicoAsync(x => x.Id == Id);
            if (v != null)
            {
                v.ConfiguracionValida = configuracionValida;
                UDT.Context.Entry(v).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }

        public async Task ActualizarAsync(GestorLocalConfig entity)
        {
            seguridad.EstableceDatosProceso<GestorLocalConfig>();
            GestorLocalConfig o =  await this.repo.UnicoAsync(x => x.VolumenId == entity.VolumenId);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.VolumenId);
                }


            if (!await seguridad.AccesoCacheVolumen(entity.VolumenId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(entity.VolumenId);
            }

            await ValidaEntidad(entity);
            string original = o.Flat();

            o.Ruta = entity.Ruta;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            await ActualizaEstadoVolumen(entity.VolumenId, true);

            await seguridad.RegistraEventoActualizar(o.VolumenId, o.Ruta, original.JsonDiff(o.Flat()));
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
        public async Task<IPaginado<GestorLocalConfig>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<GestorLocalConfig>, IIncludableQueryable<GestorLocalConfig, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<GestorLocalConfig>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<GestorLocalConfig>();
            //GestorLocalConfig d;
            //ICollection<string> lista = new HashSet<string>();
            //foreach (var Id in ids.LimpiaIds())
            //{
            //    d = await this.repo.UnicoAsync(x => x.VolumenId == Id);
            //    if (d != null)
            //    {
            //        lista.Add(Id);
            //        await repo.Eliminar(d);
            //     }
            //}

            //UDT.SaveChanges();
            //return lista;
            throw new NotImplementedException();
        }


        public async Task<GestorLocalConfig> ObtieneConfiguracionVolumen(string VolumenId)
        {
            seguridad.EstableceDatosProceso<GestorLocalConfig>();
            var v = await this.UDT.Context.Volumen.FirstOrDefaultAsync(x => x.Id == VolumenId);
            if (v != null)
            {
                if(!await seguridad.AccesoCacheVolumen(VolumenId))
                {
                    await seguridad.EmiteDatosSesionIncorrectos(VolumenId);
                }

                if(v.TipoGestorESId != TipoGestorES.LOCAL_FOLDER)
                {
                    throw new ExDatosNoValidos();
                }

                var c = this.UDT.Context.GestorLocalConfig.FirstOrDefault(x => x.VolumenId == VolumenId);
                if (c == null)
                {
                    c = new GestorLocalConfig() { Ruta = "", VolumenId = VolumenId };
                    this.UDT.Context.GestorLocalConfig.Add(c);
                    this.UDT.Context.SaveChanges();
                }
                return c;
            }
            throw new ExDatosNoValidos();
        }


        public async Task<GestorLocalConfig> UnicoAsync(Expression<Func<GestorLocalConfig, bool>> predicado = null, Func<IQueryable<GestorLocalConfig>, IOrderedQueryable<GestorLocalConfig>> ordenarPor = null, Func<IQueryable<GestorLocalConfig>, IIncludableQueryable<GestorLocalConfig, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<GestorLocalConfig>();
            var o = await this.repo.UnicoAsync(predicado);
            if (!await seguridad.AccesoCacheVolumen(o.VolumenId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(o.VolumenId);
            }
            return o.Copia();
        }

        #region No Implementado


        public Task<List<GestorLocalConfig>> ObtenerAsync(Expression<Func<GestorLocalConfig, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task<List<GestorLocalConfig>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<GestorLocalConfig>> CrearAsync(params GestorLocalConfig[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GestorLocalConfig>> CrearAsync(IEnumerable<GestorLocalConfig> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<GestorLocalConfig>> ObtenerPaginadoAsync(Expression<Func<GestorLocalConfig, bool>> predicate = null, Func<IQueryable<GestorLocalConfig>, IOrderedQueryable<GestorLocalConfig>> orderBy = null, Func<IQueryable<GestorLocalConfig>, IIncludableQueryable<GestorLocalConfig, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<GestorLocalConfig> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }



        #endregion





    }



}