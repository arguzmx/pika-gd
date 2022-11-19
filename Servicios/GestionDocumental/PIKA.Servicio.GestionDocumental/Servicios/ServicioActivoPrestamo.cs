﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
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
    public class ServicioActivoPrestamo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivoPrestamo
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ActivoPrestamo> repo;
        private ICompositorConsulta<ActivoPrestamo> compositor;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioActivoPrestamo(IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger)
        {

            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ActivoPrestamo>(new QueryComposer<ActivoPrestamo>());
        }

        public async Task<bool> Existe(Expression<Func<ActivoPrestamo, bool>> predicado)
        {
            List<ActivoPrestamo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ActivoPrestamo> CrearAsync(ActivoPrestamo entity, CancellationToken cancellationToken = default)
        {
            var prestamo = await UDT.Context.Prestamos.FindAsync(entity.PrestamoId);
            if (prestamo.Devuelto || prestamo.Entregado) throw
                       new ExDatosNoValidos("No pueden añadirse activos a un prestamo entragado o cerrado");
            
            var activo = await UDT.Context.Activos.FindAsync(entity.ActivoId);
            if (activo.EnPrestamo)
            {
                new ExDatosNoValidos("El elemento se encuentra en préstamo");
            }

            if (prestamo != null)
            {
                if (!await Existe(x => x.PrestamoId == entity.PrestamoId && x.ActivoId == entity.ActivoId))
                {
                    entity.Id = Guid.NewGuid().ToString();
                    entity.Devuelto = false;
                    await this.repo.CrearAsync(entity);
                    UDT.SaveChanges();

                    prestamo.CantidadActivos++;
                    UDT.SaveChanges();

                    EstableceEnPrestamo(new List<string>() { entity.ActivoId }, true);
                }
                return entity.Copia();
            }
            throw new EXNoEncontrado(entity.PrestamoId);
            
        }

        public async Task ActualizarAsync(ActivoPrestamo entity)
        {

            ActivoPrestamo o = await this.repo.UnicoAsync(x => x.PrestamoId == entity.PrestamoId && x.ActivoId == entity.ActivoId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ActivoId +"|"+entity.PrestamoId);
            }

            o.Devuelto = entity.Devuelto;
            o.FechaDevolucion = entity.FechaDevolucion;

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
        public async Task<IPaginado<ActivoPrestamo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ActivoPrestamo>, IIncludableQueryable<ActivoPrestamo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ActivoPrestamo>> CrearAsync(params ActivoPrestamo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivoPrestamo>> CrearAsync(IEnumerable<ActivoPrestamo> entities, CancellationToken cancellationToken = default)
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            
            ActivoPrestamo ap;
            bool primero = true;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                ap = await this.repo.UnicoAsync(x => x.Id == Id);
                if (ap!=null && primero)
                {
                    Prestamo p = UDT.Context.Prestamos.Find(ap.PrestamoId);
                    if(p.Devuelto || p.Entregado)
                    {
                        throw new ExDatosNoValidos("No pueden eliminarse elementos de un préstamo entregado o devuelto");
                    }
                    primero = false;
                }

                
                if (ap != null)
                {
                    UDT.Context.Entry(ap).State = EntityState.Deleted;
                    listaEliminados.Add(ap.ActivoId);
                }
            }
            UDT.SaveChanges();

            EstableceEnPrestamo(listaEliminados.ToList(), false);

            return listaEliminados;
        }

        private void EstableceEnPrestamo(List<string> lista, bool EnPrestamo)
        {
            string l = "";
            lista.ToList().ForEach(id =>
            {
                l += $"'{id}',";
            });

            this.UDT.Context.Database.GetDbConnection().Open();
            var cmd = this.UDT.Context.Database.GetDbConnection().CreateCommand();
            string p = EnPrestamo ? "1": "0";
            string sql = @$"update  {DBContextGestionDocumental.TablaActivos} set EnPrestamo={p} where Id In ({l.TrimEnd(',')});";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }


        public Task<List<ActivoPrestamo>> ObtenerAsync(Expression<Func<ActivoPrestamo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ActivoPrestamo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<ActivoPrestamo>> ObtenerPaginadoAsync(Expression<Func<ActivoPrestamo, bool>> predicate = null, Func<IQueryable<ActivoPrestamo>, IOrderedQueryable<ActivoPrestamo>> orderBy = null, Func<IQueryable<ActivoPrestamo>, IIncludableQueryable<ActivoPrestamo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ActivoPrestamo> UnicoAsync(Expression<Func<ActivoPrestamo, bool>> predicado = null, Func<IQueryable<ActivoPrestamo>, IOrderedQueryable<ActivoPrestamo>> ordenarPor = null, Func<IQueryable<ActivoPrestamo>, IIncludableQueryable<ActivoPrestamo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ActivoPrestamo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }
        public Task<List<string>> EliminarActivosPrestamos(int entidad, string[] ids) 
        {
            if (entidad == 1)
                return EliminarPrestamos(ids);
            else
                return EliminarActivos(ids);
        }
        private async Task<List<string>> EliminarPrestamos(string[] ids)
        {
            ActivoPrestamo ap;
            List<string> listaEliminados = new List<string>();
            foreach (var Id in ids)
            {
                ap = await this.repo.UnicoAsync(x => x.PrestamoId == Id);
                if (ap != null)
                {
                    await this.repo.Eliminar(ap);
                    listaEliminados.Add(ap.ActivoId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        private async Task<List<string>> EliminarActivos(string[] ids)
        {
            ActivoPrestamo ap;
            List<string> listaEliminados = new List<string>();
            foreach (var Id in ids)
            {
                ap = await this.repo.UnicoAsync(x => x.ActivoId == Id);
                if (ap != null)
                {
                    UDT.Context.Entry(ap).State = EntityState.Deleted;
                    listaEliminados.Add(ap.ActivoId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        
    }
}
