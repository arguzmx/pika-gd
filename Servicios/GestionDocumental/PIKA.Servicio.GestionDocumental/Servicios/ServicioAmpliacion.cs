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
    public class ServicioAmpliacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioAmpliacion
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Ampliacion> repo;
        private ICompositorConsulta<Ampliacion> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioAmpliacion(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ICompositorConsulta<Ampliacion> compositorConsulta,
           ILogger<ServicioAmpliacion> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Ampliacion>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<Ampliacion, bool>> predicado)
        {
            List<Ampliacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Ampliacion> CrearAsync(Ampliacion entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.ActivoId == entity.ActivoId && x.Vigente == entity.Vigente))
            {
                throw new ExElementoExistente(entity.ActivoId +"|"+entity.Vigente);
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }


        public async Task ActualizarAsync(Ampliacion entity)
        {

            Ampliacion o = await this.repo.UnicoAsync(x => x.ActivoId == entity.ActivoId && x.Vigente == entity.Vigente);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ActivoId + "|" + entity.Vigente);
            }

            o.Vigente = entity.Vigente;
            o.TipoAmpliacionId = entity.TipoAmpliacionId;
            o.FechaFija = entity.FechaFija;
            o.FechaFija = entity.FechaFija;
            o.FundamentoLegal = entity.FundamentoLegal;
            o.Inicio = entity.Inicio;
            o.Fin = entity.Fin;
            o.Anos = entity.Anos;
            o.Meses = entity.Meses;
            o.Dias = entity.Dias;


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
        public async Task<IPaginado<Ampliacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Ampliacion>, IIncludableQueryable<Ampliacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<Ampliacion>> CrearAsync(params Ampliacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ampliacion>> CrearAsync(IEnumerable<Ampliacion> entities, CancellationToken cancellationToken = default)
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
            Ampliacion a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.ActivoId == Id);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.ActivoId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<Ampliacion>> ObtenerAsync(Expression<Func<Ampliacion, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Ampliacion>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Ampliacion>> ObtenerPaginadoAsync(Expression<Func<Ampliacion, bool>> predicate = null, Func<IQueryable<Ampliacion>, IOrderedQueryable<Ampliacion>> orderBy = null, Func<IQueryable<Ampliacion>, IIncludableQueryable<Ampliacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Ampliacion> UnicoAsync(Expression<Func<Ampliacion, bool>> predicado = null, Func<IQueryable<Ampliacion>, IOrderedQueryable<Ampliacion>> ordenarPor = null, Func<IQueryable<Ampliacion>, IIncludableQueryable<Ampliacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Ampliacion a = await this.repo.UnicoAsync(predicado);
            return a.CopiaAmpliacion();
        }
    }
}
