using DocumentFormat.OpenXml.Vml.Office;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioTipoAmpliacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTipoAmpliacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IRepositorioAsync<TipoAmpliacion> repo;

        public ServicioTipoAmpliacion(
            IOptions<ConfiguracionServidor> config,
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, 
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger, cache,
               ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ACTIVOS)
        {
            this.ConfiguracionServidor = config.Value;
            this.repo = UDT.ObtenerRepositoryAsync<TipoAmpliacion>(new QueryComposer<TipoAmpliacion>());
        }

        public async Task<bool> Existe(Expression<Func<TipoAmpliacion, bool>> predicado)
        {
            List<TipoAmpliacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        private bool VerificarDominio()
        {
            return ConfiguracionServidor.dominio_en_catalogos.HasValue && ConfiguracionServidor.dominio_en_catalogos.Value == true;
        }

        public async Task<TipoAmpliacion> CrearAsync(TipoAmpliacion entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<TipoAmpliacion>();
            await seguridad.IdEnDominio(entity.DominioId);

            if (VerificarDominio())
            {
                if (await Existe(x => 
                    x.DominioId == entity.DominioId &&
                    x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }
            else
            {
                if (await Existe(x => 
                x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }

            entity.Id = entity.Id.Trim();
            entity.Nombre = entity.Nombre.Trim();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }
        public async Task ActualizarAsync(TipoAmpliacion entity)
        {
            seguridad.EstableceDatosProceso<TipoAmpliacion>();
            await seguridad.IdEnDominio(entity.DominioId);

            if (VerificarDominio())
            {
                if (await Existe(x =>
                    x.Id != entity.Id && x.DominioId == entity.DominioId &&
                    x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }
            else
            {
                if (await Existe(x =>
                x.Id != entity.Id &&
                x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }

            TipoAmpliacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            string original = JsonConvert.SerializeObject(o);
            
            o.Nombre = entity.Nombre.Trim();
            o.Id = entity.Id.Trim();
            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id,  o.Nombre, original.JsonDiff(JsonConvert.SerializeObject(o)));

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
        public async Task<IPaginado<TipoAmpliacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoAmpliacion>, IIncludableQueryable<TipoAmpliacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<TipoAmpliacion>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<TipoAmpliacion>();
            List<TipoAmpliacion> listaEliminados = new List<TipoAmpliacion>();

            foreach (var Id in ids)
            {
                TipoAmpliacion o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id);
                    if (o != null)
                    {
                        if (await seguridad.IdEnDominio(o.DominioId))
                        {
                            if(UDT.Context.Ampliaciones.Any(x=>x.TipoAmpliacionId == o.Id))
                            {
                                throw new ExElementoExistente();
                            }
                            listaEliminados.Add(o);
                        } else
                        {
                            await seguridad.EmiteDatosSesionIncorrectos( "*", "*");
                        }
                    }
                }
            }

            foreach(var c in listaEliminados)
            {
                UDT.Context.Entry(c).State = EntityState.Deleted;
                await seguridad.RegistraEventoEliminar( c.Id, c.Nombre);
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x=>x.Id).ToList();

        }

        public async Task<TipoAmpliacion> UnicoAsync(Expression<Func<TipoAmpliacion, bool>> predicado = null, Func<IQueryable<TipoAmpliacion>, IOrderedQueryable<TipoAmpliacion>> ordenarPor = null, Func<IQueryable<TipoAmpliacion>, IIncludableQueryable<TipoAmpliacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<TipoAmpliacion>();
            TipoAmpliacion t = await this.repo.UnicoAsync(predicado);
            await seguridad.IdEnDominio(t.DominioId);
            return t.Copia();
        }

        public Task<List<TipoAmpliacion>> ObtenerAsync(Expression<Func<TipoAmpliacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<TipoAmpliacion>();
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id.Trim()));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public Task<List<TipoAmpliacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);

        }

        #region  Sin Implementar 


        public Task<IPaginado<TipoAmpliacion>> ObtenerPaginadoAsync(Expression<Func<TipoAmpliacion, bool>> predicate = null, Func<IQueryable<TipoAmpliacion>, IOrderedQueryable<TipoAmpliacion>> orderBy = null, Func<IQueryable<TipoAmpliacion>, IIncludableQueryable<TipoAmpliacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<TipoAmpliacion>> CrearAsync(params TipoAmpliacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAmpliacion>> CrearAsync(IEnumerable<TipoAmpliacion> entities, CancellationToken cancellationToken = default)
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

        public Task<TipoAmpliacion> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
