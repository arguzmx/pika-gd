﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
   public class ServicioTipoValoracionDocumental: ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTipoValoracionDocumental
    {
       
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
         
        private IRepositorioAsync<TipoValoracionDocumental> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoVC;
        private readonly ConfiguracionServidor ConfiguracionServidor;
        

        public ServicioTipoValoracionDocumental(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
         IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
         ILogger<ServicioLog> Logger,
         IOptions<ConfiguracionServidor> Config) :
            base(registroAuditoria, proveedorOpciones, Logger, cache,
                ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC)
        {
            this.ConfiguracionServidor = Config.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
            this.repoVC= UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
        }

        private bool VerificarDominio()
        {
            return ConfiguracionServidor.dominio_en_catalogos.HasValue && ConfiguracionServidor.dominio_en_catalogos.Value == true;
        }

        public async Task<bool> Existe(Expression<Func<TipoValoracionDocumental, bool>> predicado)
        {
            List<TipoValoracionDocumental> l = await this.repo.ObtenerAsync(predicado);

            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<TipoValoracionDocumental> CrearAsync(TipoValoracionDocumental entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
            await seguridad.IdEnDominio(entity.DominioId);

            if (VerificarDominio())
            {
                if (await Existe(x => x.DominioId == RegistroActividad.DominioId 
                    && x.Id != entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
                entity.DominioId = RegistroActividad.DominioId;
                entity.UOId = null;

            } else
            {
                entity.DominioId = null;
                entity.UOId = null;
                if (await Existe(x => x.Id != entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }

            
            TipoValoracionDocumental tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }
            entity.Nombre = entity.Nombre.Trim();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }


        public async Task ActualizarAsync(TipoValoracionDocumental entity)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
            await seguridad.IdEnDominio(entity.DominioId);

            if (VerificarDominio())
            {
                if (await Existe(x => x.DominioId == RegistroActividad.DominioId
                    && x.Id != entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
                entity.DominioId = RegistroActividad.DominioId;
                entity.UOId = null;

            }
            else
            {
                entity.DominioId = null;
                entity.UOId = null;
                if (await Existe(x => x.Id != entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }

            var tmp = UDT.Context.TipoValoracionDocumental.FirstOrDefault(x => x.Id == entity.Id);
            if(tmp==null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            string original = tmp.Flat();
            tmp.Nombre = entity.Nombre;
            UDT.Context.Entry(tmp).State = EntityState.Modified;
            UDT.SaveChanges();
            await seguridad.RegistraEventoActualizar( entity.Id, entity.Nombre, original.JsonDiff(tmp.Flat()));
        }


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
            TipoValoracionDocumental o;
            List<TipoValoracionDocumental> listaEliminados = new List<TipoValoracionDocumental>();

            foreach (var Id in ids)
            {
                if (VerificarDominio())
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id.Trim() && x.DominioId == RegistroActividad.DominioId);
                }
                else
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                }


                if (o != null)
                {
                    if (VerificarDominio())
                    {
                        await seguridad.IdEnDominio(o.DominioId);
                    }

                    // verifica que no exista un elemento asigando al catálogo
                    if (UDT.Context.ValoracionEntradaClasificacion.Any(x => x.TipoValoracionDocumentalId == o.Id))
                    {
                        throw new ExElementoExistente();
                    } else
                    {
                        listaEliminados.Add(o); 
                    }

                }
            }

            foreach (var c in listaEliminados)
            {
                UDT.Context.Entry(c).State = EntityState.Deleted;
                await seguridad.RegistraEventoEliminar( c.Id, c.Nombre);
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x => x.Id).ToList();

        }


        public Task<List<TipoValoracionDocumental>> ObtenerAsync(Expression<Func<TipoValoracionDocumental, bool>> predicado)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<TipoValoracionDocumental> UnicoAsync(Expression<Func<TipoValoracionDocumental, bool>> predicado = null, Func<IQueryable<TipoValoracionDocumental>, IOrderedQueryable<TipoValoracionDocumental>> ordenarPor = null, Func<IQueryable<TipoValoracionDocumental>, IIncludableQueryable<TipoValoracionDocumental, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
            TipoValoracionDocumental d = await this.repo.UnicoAsync(predicado);
            await seguridad.IdEnDominio(d.DominioId);
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

            if (VerificarDominio())
            {
                query.Filtros.RemoveAll(x => x.Propiedad == "DominioId");
                query.Filtros.Add(new FiltroConsulta() { Propiedad = "DominioId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.DominioId });
            }

            return query;
        }

        public async Task<IPaginado<TipoValoracionDocumental>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoValoracionDocumental>, IIncludableQueryable<TipoValoracionDocumental, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<TipoValoracionDocumental>();
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }
        public Task<List<TipoValoracionDocumental>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);

        }


        #region sin implementar

        public Task<IEnumerable<TipoValoracionDocumental>> CrearAsync(params TipoValoracionDocumental[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoValoracionDocumental>> CrearAsync(IEnumerable<TipoValoracionDocumental> entities, CancellationToken cancellationToken = default)
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


     


        public async Task<IPaginado<TipoValoracionDocumental>> ObtenerPaginadoAsync(Expression<Func<TipoValoracionDocumental, bool>> predicate = null, Func<IQueryable<TipoValoracionDocumental>, IOrderedQueryable<TipoValoracionDocumental>> orderBy = null, Func<IQueryable<TipoValoracionDocumental>, IIncludableQueryable<TipoValoracionDocumental, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TipoValoracionDocumental> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }




        #endregion
    }
}
