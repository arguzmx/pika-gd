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
using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Servicios
{
 public   class ServicioTipoAlmacenMetadatos : ContextoServicioMetadatos, IServicioInyectable, IServicioTipoAlmacenMetadatos
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<TipoAlmacenMetadatos> repo;
        private ICompositorConsulta<TipoAlmacenMetadatos> compositor;

        public ServicioTipoAlmacenMetadatos(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS)
        {
            this.repo = UDT.ObtenerRepositoryAsync(compositor);
        }

        public async Task<bool> Existe(Expression<Func<TipoAlmacenMetadatos, bool>> predicado)
        {
            List<TipoAlmacenMetadatos> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<TipoAlmacenMetadatos> CrearAsync(TipoAlmacenMetadatos entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Nombre.Equals(entity.Nombre.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre.Trim());
            }
            entity.Id = entity.Id.Trim();
            entity.Nombre = entity.Nombre.Trim();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(TipoAlmacenMetadatos entity)
        {

            TipoAlmacenMetadatos o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;


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
        public async Task<IPaginado<TipoAlmacenMetadatos>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoAlmacenMetadatos>, IIncludableQueryable<TipoAlmacenMetadatos, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public Task<List<TipoAlmacenMetadatos>> ObtenerAsync(Expression<Func<TipoAlmacenMetadatos, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
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
       

        public async Task<TipoAlmacenMetadatos> UnicoAsync(Expression<Func<TipoAlmacenMetadatos, bool>> predicado = null, Func<IQueryable<TipoAlmacenMetadatos>, IOrderedQueryable<TipoAlmacenMetadatos>> ordenarPor = null, Func<IQueryable<TipoAlmacenMetadatos>, IIncludableQueryable<TipoAlmacenMetadatos, object>> incluir = null, bool inhabilitarSegumiento = true)
        {


            TipoAlmacenMetadatos d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            TipoAlmacenMetadatos o;
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
        
        public Task<List<TipoAlmacenMetadatos>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        #region Sin implementar
        public Task<IEnumerable<TipoAlmacenMetadatos>> CrearAsync(params TipoAlmacenMetadatos[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoAlmacenMetadatos>> CrearAsync(IEnumerable<TipoAlmacenMetadatos> entities, CancellationToken cancellationToken = default)
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

      

        

        public Task<IPaginado<TipoAlmacenMetadatos>> ObtenerPaginadoAsync(Expression<Func<TipoAlmacenMetadatos, bool>> predicate = null, Func<IQueryable<TipoAlmacenMetadatos>, IOrderedQueryable<TipoAlmacenMetadatos>> orderBy = null, Func<IQueryable<TipoAlmacenMetadatos>, IIncludableQueryable<TipoAlmacenMetadatos, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TipoAlmacenMetadatos> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
