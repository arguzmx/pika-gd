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
    public class ServicioTipoArchivo: ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTipoArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoArchivo> repo;
        private ConfiguracionServidor ConfiguracionServidor;
        public ServicioTipoArchivo(
            IAppCache cache,
            IOptions<ConfiguracionServidor> ConfiguracionServidor,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CATALOGOSCC)
        {
            this.ConfiguracionServidor = ConfiguracionServidor.Value;
            this.repo = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
        }
        public async Task<bool> Existe(Expression<Func<TipoArchivo, bool>> predicado)
        {
            List<TipoArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        private bool VerificarDominio()
        {
            return ConfiguracionServidor.dominio_en_catalogos.HasValue && ConfiguracionServidor.dominio_en_catalogos.Value == true;
        }

        public async Task<TipoArchivo> CrearAsync(TipoArchivo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
            if (VerificarDominio())
            {
                await seguridad.IdEnDominio(entity.DominioId);


                if (await Existe(x => x.DominioId == this.RegistroActividad.DominioId &&
                                    x.Id == entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
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
                if (await Existe(x => x.Id == entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }

            TipoArchivo tmp = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (tmp != null)
            {
                throw new ExElementoExistente(entity.Id);
            }

            entity.Id = Guid.NewGuid().ToString();
            entity.Nombre = entity.Nombre.Trim();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }

        public async Task ActualizarAsync(TipoArchivo entity)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
            TipoArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id.Trim());
            if (o == null)
                throw new EXNoEncontrado(entity.Id);
            
            string original = o.Flat();

            if (VerificarDominio())
            {
                await seguridad.IdEnDominio(entity.DominioId);

                if (await Existe(x => x.DominioId == this.RegistroActividad.DominioId &&
                                    x.Id != entity.Id && string.Equals(x.Nombre.Trim(), entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
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

            o.Nombre = entity.Nombre.Trim();
            o.Tipo = entity.Tipo;
            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(o.Flat()));
        }


        public Task<List<TipoArchivo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
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
        public async Task<IPaginado<TipoArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
            TipoArchivo o;
            List<TipoArchivo> listaEliminados = new List<TipoArchivo>();

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
                    if (UDT.Context.Archivos.Any(x => x.TipoArchivoId == o.Id))
                    {
                        throw new ExElementoExistente();

                    }
                    else
                    {
                        listaEliminados.Add(o);
                    }

                }
            }


            if (listaEliminados.Count > 0)
            {
                foreach (var c in listaEliminados)
                {
                    UDT.Context.Entry(c).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar( c.Id, c.Nombre);
                }
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x => x.Id).ToList();

        }


        public async Task<TipoArchivo> UnicoAsync(Expression<Func<TipoArchivo, bool>> predicado = null, Func<IQueryable<TipoArchivo>, IOrderedQueryable<TipoArchivo>> ordenarPor = null, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
            TipoArchivo o = await this.repo.UnicoAsync(predicado);
            await seguridad.IdEnDominio(o.DominioId);
            return o.Copia();
        }

        public Task<List<TipoArchivo>> ObtenerAsync(Expression<Func<TipoArchivo, bool>> predicado)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<TipoArchivo>();
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

        #region Sin Implementar

        public Task<IEnumerable<TipoArchivo>> CrearAsync(params TipoArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoArchivo>> CrearAsync(IEnumerable<TipoArchivo> entities, CancellationToken cancellationToken = default)
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


        public Task<IPaginado<TipoArchivo>> ObtenerPaginadoAsync(Expression<Func<TipoArchivo, bool>> predicate = null, Func<IQueryable<TipoArchivo>, IOrderedQueryable<TipoArchivo>> orderBy = null, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TipoArchivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
        #endregion



    }
}
