﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
  public  class ServicioParte : ContextoServicioContenido,
        IServicioInyectable, IServicioParte
    {
        private const string DEFAULT_SORT_COL = "ElementoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Parte> repo;
        private ICompositorConsulta<Parte> compositor;
        private UnidadDeTrabajo<DbContextContenido> UDT;

        public ServicioParte(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ICompositorConsulta<Parte> compositorConsulta,
        ILogger<ServicioParte> Logger,
        IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Parte>(compositor);
        }


        public async Task<bool> Existe(Expression<Func<Parte, bool>> predicado)
        {
            List<Parte> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Parte> CrearAsync(Parte entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.ElementoId.Equals(entity.ElementoId, StringComparison.InvariantCultureIgnoreCase)&& 
            x.VersionId.Equals(entity.VersionId,StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                throw new ExElementoExistente(entity.ElementoId);
            }

            //entity.ElementoId = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(Parte entity)
        {

            Parte o = await this.repo.UnicoAsync(x => x.ElementoId == entity.ElementoId && x.VersionId==entity.VersionId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ElementoId +", "+ entity.VersionId);
            }

            if (await Existe(x =>
            x.ElementoId != entity.ElementoId & x.VersionId == entity.VersionId ))
            {
                throw new ExElementoExistente(entity.ElementoId + ", " + entity.VersionId);
            }

            o.Indice = entity.Indice;
            o.ConsecutivoVolumen = entity.ConsecutivoVolumen;
            o.TipoMime = entity.TipoMime;
            o.LongitudBytes = entity.LongitudBytes;
            o.NombreOriginal = entity.NombreOriginal;
            o.Eliminada = entity.Eliminada;
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
        public async Task<IPaginado<Parte>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Parte>, IIncludableQueryable<Parte, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Parte>> CrearAsync(params Parte[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Parte>> CrearAsync(IEnumerable<Parte> entities, CancellationToken cancellationToken = default)
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Parte d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.VersionId == Id );
                if (d != null)
                {

                    d.Eliminada = true;
                    UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.VersionId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<Parte>> ObtenerAsync(Expression<Func<Parte, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Parte>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Parte>> ObtenerPaginadoAsync(Expression<Func<Parte, bool>> predicate = null, Func<IQueryable<Parte>, IOrderedQueryable<Parte>> orderBy = null, Func<IQueryable<Parte>, IIncludableQueryable<Parte, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Parte> UnicoAsync(Expression<Func<Parte, bool>> predicado = null, Func<IQueryable<Parte>, IOrderedQueryable<Parte>> ordenarPor = null, Func<IQueryable<Parte>, IIncludableQueryable<Parte, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Parte d = await this.repo.UnicoAsync(predicado);

            return d.CopiaParte();
        }

    }
}