﻿using System;
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
using PIKA.Servicio.Contenido.ElasticSearch.modelos;
using PIKA.Servicio.Contenido.Gestores;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioVolumen : ContextoServicioContenido,
        IServicioInyectable, IServicioVolumen
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Volumen> repo;
        private static TimeSpan volCacheExpiry = new TimeSpan(0, 1, 0);
        private IConfiguration configuration;
        IOptions<ConfiguracionServidor> opciones;
        public ServicioVolumen(
            IOptions<ConfiguracionServidor> opciones,
            IConfiguration configuration,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION)
        {
            this.opciones = opciones;
            this.configuration= configuration;
            this.repo = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
        }

        private string GetCacheVolKey(string VolumenId)
        {
            return $"cache-vol-{VolumenId}";
        }

        private string GetCacheGestorESKey(string VolumenId)
        {
            return $"cache-gestor-{VolumenId}";
        }

        public async Task<IGestorES> ObtienInstanciaGestor(string VolumenId)
        {
            seguridad.EstableceDatosProceso<Volumen>();
            if (!await seguridad.AccesoCacheVolumen(VolumenId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(VolumenId);
            }

            Volumen vol = null;
            IGestorES gestor = null;
            if (!string.IsNullOrEmpty(VolumenId))
            {
                string key = GetCacheVolKey(VolumenId);
                vol = cache.Get<Volumen>(key);
                if (vol == null)
                {
                    vol = await this.UDT.Context.Volumen.FirstOrDefaultAsync(x => x.Id.Equals(VolumenId.Trim(), StringComparison.InvariantCultureIgnoreCase));
                    if (vol != null)
                    {
                        this.cache.Add<Volumen>(key, vol, volCacheExpiry);
                    }
                }
            }

            if( vol!=null)
            {
                string key = GetCacheGestorESKey(vol.Id);
                switch (vol.TipoGestorESId)
                {
                    case TipoGestorES.LOCAL_FOLDER:
                        GestorLocalConfig config = cache.Get<GestorLocalConfig>(key);
                        if (config == null)
                        {
                            IRepositorioAsync<GestorLocalConfig> repoconfig = UDT.ObtenerRepositoryAsync<GestorLocalConfig>(new QueryComposer<GestorLocalConfig>()); ;
                            config = await this.UDT.Context.GestorLocalConfig.FirstOrDefaultAsync(x => x.VolumenId.Equals(VolumenId.Trim(), StringComparison.InvariantCultureIgnoreCase));
                            if (config != null)
                            {
                                this.cache.Add<GestorLocalConfig>(key, config, volCacheExpiry);
                            }
                        }

                        if (config != null)
                        {
                            gestor = new GestorLocal(logger, config, configuration, opciones);
                        }
                        break;

                    case TipoGestorES.LaserFiche:
                        GestorLaserficheConfig configlf = cache.Get<GestorLaserficheConfig>(key);
                        if (configlf == null)
                        {
                            IRepositorioAsync<GestorLaserficheConfig> repoconfig = UDT.ObtenerRepositoryAsync<GestorLaserficheConfig>(new QueryComposer<GestorLaserficheConfig>()); ;
                            configlf = await this.UDT.Context.GestorLaserficheConfig.FirstOrDefaultAsync(x => x.VolumenId.Equals(VolumenId.Trim(), StringComparison.InvariantCultureIgnoreCase));
                            if (configlf != null)
                            {
                                this.cache.Add<GestorLaserficheConfig>(key, configlf, volCacheExpiry);
                            }
                        }

                        if (configlf != null)
                        {
                            gestor = new GestorLaserfiche(logger, configlf, opciones);
                        }
                        break;


                    case TipoGestorES.SMB:
                        break;

                    case TipoGestorES.AzureBlob:
                        break;

                }
            }
            

            return gestor;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<Volumen>();

            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = true,
                    Operador = "eq",
                    Valor = "true"
                });
            }
            Query = GetDefaultQuery(Query);
            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {

            this.UDT.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }



        public async Task<bool> Existe(Expression<Func<Volumen, bool>> predicado)
        {
            List<Volumen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Volumen> CrearAsync(Volumen entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Volumen>();
            await seguridad.IdEnDominio(entity.OrigenId);


            if (await Existe(x => x.Nombre == entity.Nombre
            && x.OrigenId != entity.OrigenId
            && x.TipoOrigenId == entity.TipoOrigenId))
            {
                throw new ExElementoExistente(entity.Nombre);
            }


            entity.Id = System.Guid.NewGuid().ToString();
            // Se actualizará a activo cuando se configure la conexión del tipo de gestor

            entity.Activo = true;
            entity.EscrituraHabilitada = true;
            entity.Eliminada = false;
            entity.ConsecutivoVolumen = 0;
            entity.CanidadPartes = 0;
            entity.CanidadElementos = 0;
            await this.repo.CrearAsync(entity);

            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);
            return entity.Copia();
        }


        public async Task ActualizarAsync(Volumen entity)
        {
            seguridad.EstableceDatosProceso<Volumen>();

            Volumen o = await this.UDT.Context.Volumen.FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (!await seguridad.AccesoCacheVolumen(o.Id))
            {
                await seguridad.EmiteDatosSesionIncorrectos(o.Id);
            }

            if (await Existe(x => x.Nombre == entity.Nombre && x.OrigenId == o.OrigenId
           && x.TipoOrigenId == o.TipoOrigenId && x.Id != entity.Id))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            if ((entity.TamanoMaximo > 0) && (o.Tamano > entity.TamanoMaximo))
            {
                throw new ExDatosNoValidos("El tamaño máximo es menor al actual");
            }

            string original = o.Flat();

            /// Solo se permiten cambios en el estao una vez que l aconfiguración es válida
            if (o.ConfiguracionValida)
            {
                o.Activo = entity.Activo;
            }

            if (o.TipoGestorESId != entity.TipoGestorESId)
            {
                o.TipoGestorESId = entity.TipoGestorESId;
                o.ConfiguracionValida = false;
            }

            o.Nombre = entity.Nombre;
            o.EscrituraHabilitada = entity.EscrituraHabilitada;
            o.TamanoMaximo = entity.TamanoMaximo;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(o.Flat()));
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

            // Añade un filtro permanente para la unidad organizacional
            query.Filtros.RemoveAll(x => x.Propiedad == "OrigenId");
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "OrigenId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.DominioId });


            return query;
        }
        public async Task<IPaginado<Volumen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }

      

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Volumen>();
            List<Volumen> listaEliminados = new List<Volumen>();
            foreach (var Id in ids.LimpiaIds())
            {
                Volumen d = await this.UDT.Context.Volumen.FirstOrDefaultAsync(x => x.Id == Id);
                if (d != null)
                {
                    if(!await seguridad.AccesoCacheVolumen(d.Id))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(d.Id);
                    }

                    listaEliminados.Add(d);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var d in listaEliminados)
                {
                    d.Eliminada = true;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    await seguridad.RegistraEventoEliminar(d.Id, d.Nombre);
                }
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x=>x.Id).ToList();
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Volumen>();
            List<Volumen> listaEliminados = new List<Volumen>();
            foreach (var Id in ids.LimpiaIds())
            {
                Volumen d = await this.UDT.Context.Volumen.FirstOrDefaultAsync(x => x.Id == Id);
                if (d != null)
                {
                    if (!await seguridad.AccesoCacheVolumen(d.Id))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(d.Id);
                    }

                    listaEliminados.Add(d);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach (var d in listaEliminados)
                {
                    string original = d.Flat();

                    d.Eliminada = false;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    await seguridad.RegistraEventoActualizar(d.Id, d.Nombre, original.JsonDiff(d.Flat()));
                }
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x => x.Id).ToList();
        }



        public async Task<Volumen> UnicoAsync(Expression<Func<Volumen, bool>> predicado = null, Func<IQueryable<Volumen>, IOrderedQueryable<Volumen>> ordenarPor = null, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<Volumen>();
            Volumen d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);
            if (!await seguridad.AccesoCacheVolumen(d.Id))
            {
                await seguridad.EmiteDatosSesionIncorrectos(d.Id);
            }
            return d.Copia();
        }
        public async Task<List<string>> Purgar()
        {
    
            throw new NotImplementedException();
        }


        #region No Implemenatdaos

        public Task<IEnumerable<Volumen>> CrearAsync(params Volumen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Volumen>> CrearAsync(IEnumerable<Volumen> entities, CancellationToken cancellationToken = default)
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

        public Task<List<Volumen>> ObtenerAsync(Expression<Func<Volumen, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Volumen>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Volumen>> ObtenerPaginadoAsync(Expression<Func<Volumen, bool>> predicate = null, Func<IQueryable<Volumen>, IOrderedQueryable<Volumen>> orderBy = null, Func<IQueryable<Volumen>, IIncludableQueryable<Volumen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task ActualizaEstadisticas(EstadisticaVolumen s, string Id)
        {
            seguridad.EstableceDatosProceso<Volumen>();
            string x = $"update {DbContextContenido.TablaVolumen} set CanidadPartes={s.ConteoPartes}, CanidadElementos={s.ConteoElementos}, Tamano={s.TamanoBytes} where Id='{Id}'";
            await this.UDT.Context.Database.ExecuteSqlRawAsync(x);

        }

        public Task<Volumen> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }




        #endregion
    }



}