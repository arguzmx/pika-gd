﻿
using Microsoft.EntityFrameworkCore;
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
    public class ServicioEventoTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioEventoTransferencia
    {
        private const string DEFAULT_SORT_COL = "Comentario";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EventoTransferencia> repo;
        private ICompositorConsulta<EventoTransferencia> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioEventoTransferencia(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ICompositorConsulta<EventoTransferencia> compositorConsulta,
           ILogger<ServicioEventoTransferencia> Logger,
           IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<EventoTransferencia>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<EventoTransferencia, bool>> predicado)
        {
            List<EventoTransferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<EventoTransferencia> CrearAsync(EventoTransferencia entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Comentario.Equals(entity.Comentario, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Comentario);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(EventoTransferencia entity)
        {

            EventoTransferencia o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Comentario.Equals(entity.Comentario, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Comentario);
            }

            o.Comentario = entity.Comentario;
            o.Fecha = entity.Fecha;
            o.EstadoTransferenciaId = entity.EstadoTransferenciaId;

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
        public async Task<IPaginado<EventoTransferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EventoTransferencia>, IIncludableQueryable<EventoTransferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<EventoTransferencia>> CrearAsync(params EventoTransferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventoTransferencia>> CrearAsync(IEnumerable<EventoTransferencia> entities, CancellationToken cancellationToken = default)
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
            EventoTransferencia a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public Task<List<EventoTransferencia>> ObtenerAsync(Expression<Func<EventoTransferencia, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventoTransferencia>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<EventoTransferencia>> ObtenerPaginadoAsync(Expression<Func<EventoTransferencia, bool>> predicate = null, Func<IQueryable<EventoTransferencia>, IOrderedQueryable<EventoTransferencia>> orderBy = null, Func<IQueryable<EventoTransferencia>, IIncludableQueryable<EventoTransferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<EventoTransferencia> UnicoAsync(Expression<Func<EventoTransferencia, bool>> predicado = null, Func<IQueryable<EventoTransferencia>, IOrderedQueryable<EventoTransferencia>> ordenarPor = null, Func<IQueryable<EventoTransferencia>, IIncludableQueryable<EventoTransferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EventoTransferencia t = await this.repo.UnicoAsync(predicado);
            return t.CopiaEventoTransferencia();
        }
    }
}
