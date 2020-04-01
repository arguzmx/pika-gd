﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioTipoArchivo: IServicioInyectable, IServicioTipoArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<TipoArchivo> repo;
        private ICompositorConsulta<TipoArchivo> compositor;
        private ILogger<ServicioTipoArchivo> logger;
        private DBContextGestionDocumental contexto;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioTipoArchivo(DBContextGestionDocumental contexto,
           ICompositorConsulta<TipoArchivo> compositorConsulta,
           ILogger<ServicioTipoArchivo> Logger,
           IServicioCache servicioCache)
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<TipoArchivo>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<TipoArchivo, bool>> predicado)
        {
            List<TipoArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<TipoArchivo> CrearAsync(TipoArchivo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(TipoArchivo entity)
        {

            TipoArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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
            o.FaseCicloVitalId = o.FaseCicloVitalId;

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
        public async Task<IPaginado<TipoArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<TipoArchivo>> CrearAsync(params TipoArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoArchivo>> CrearAsync(IEnumerable<TipoArchivo> entities, CancellationToken cancellationToken = default)
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

        public Task Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<TipoArchivo>> ObtenerAsync(Expression<Func<TipoArchivo, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoArchivo>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<TipoArchivo>> ObtenerPaginadoAsync(Expression<Func<TipoArchivo, bool>> predicate = null, Func<IQueryable<TipoArchivo>, IOrderedQueryable<TipoArchivo>> orderBy = null, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<TipoArchivo> UnicoAsync(Expression<Func<TipoArchivo, bool>> predicado = null, Func<IQueryable<TipoArchivo>, IOrderedQueryable<TipoArchivo>> ordenarPor = null, Func<IQueryable<TipoArchivo>, IIncludableQueryable<TipoArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}