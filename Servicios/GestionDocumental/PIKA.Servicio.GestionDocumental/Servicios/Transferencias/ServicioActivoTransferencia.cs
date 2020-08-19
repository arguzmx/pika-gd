
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
    public class ServicioActivoTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivoTransferencia
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ActivoTransferencia> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<Transferencia> repoT;
        private IRepositorioAsync<Archivo> repoAr;
        private IRepositorioAsync<Activo> repoAct;

        public ServicioActivoTransferencia(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, ILogger<ServicioCuadroClasificacion> Logger) 
            : base(proveedorOpciones,Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ActivoTransferencia>(new QueryComposer<ActivoTransferencia>());
            this.repoT = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.repoAr = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoAct = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
        }

        public async Task<bool> Existe(Expression<Func<ActivoTransferencia, bool>> predicado)
        {
            List<ActivoTransferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        private async Task<bool> ValidarReglas(ActivoTransferencia activoT)
        {
            Transferencia t = await this.repoT.UnicoAsync(x => x.Id.Equals(activoT.TransferenciaId, StringComparison.InvariantCultureIgnoreCase)
            );
            if (t != null)
            {
                Activo a = await this.repoAct.UnicoAsync(x => x.ArchivoId.Equals(t.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)
                && x.EnPrestamo != false
                && x.Ampliado != false
                && x.Id.Equals(activoT.ActivoId, StringComparison.InvariantCultureIgnoreCase));
                if (a != null)
                    return true;
                a = await this.repoAct.UnicoAsync(x => x.Id.Equals(activoT.ActivoId, StringComparison.InvariantCultureIgnoreCase));
                if (a != null)
                {
                    t = await this.repoT.UnicoAsync(x => x.ArchivoOrigenId.Equals(a.ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                    && x.EstadoTransferenciaId != EstadoTransferencia.ESTADO_RECIBIDA
                    && x.EstadoTransferenciaId != EstadoTransferencia.ESTADO_RECIBIDA_PARCIAL
                    && x.EstadoTransferenciaId != EstadoTransferencia.ESTADO_CANCELADA
                    && x.EstadoTransferenciaId != EstadoTransferencia.ESTADO_DECLINADA
                    );
                    if (t != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<ActivoTransferencia> CrearAsync(ActivoTransferencia entity, CancellationToken cancellationToken = default)
        {
            if (await ValidarReglas(entity))
            {
                throw new ExDatosNoValidos(entity.ActivoId);
            }
            if (await Existe(x => x.ActivoId == entity.ActivoId && x.TransferenciaId ==entity.TransferenciaId))
            {
                throw new ExElementoExistente(entity.ActivoId);
            }

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }

        public async Task ActualizarAsync(ActivoTransferencia entity)
        {

            ActivoTransferencia o = await this.repo.UnicoAsync(x => x.ActivoId == entity.ActivoId && x.TransferenciaId == entity.TransferenciaId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.ActivoId);
            }

            if (await Existe(x => x.ActivoId == entity.ActivoId && x.TransferenciaId == entity.TransferenciaId))
            {
                throw new ExElementoExistente(entity.ActivoId);
            }

            o.ActivoId = entity.ActivoId;
            o.TransferenciaId = entity.TransferenciaId;

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
        public async Task<IPaginado<ActivoTransferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ActivoTransferencia a;
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

      
        public Task<List<ActivoTransferencia>> ObtenerAsync(Expression<Func<ActivoTransferencia, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ActivoTransferencia>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
    
        public async Task<ActivoTransferencia> UnicoAsync(Expression<Func<ActivoTransferencia, bool>> predicado = null, Func<IQueryable<ActivoTransferencia>, IOrderedQueryable<ActivoTransferencia>> ordenarPor = null, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ActivoTransferencia t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }

        public async Task<ICollection<string>> EliminarActivoTransferencia(string TransferenciaId, string[] ids)
        {
            ActivoTransferencia a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.ActivoId == Id && x.TransferenciaId == TransferenciaId);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.ActivoId);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        #region Sin Implementar
        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public Task<IPaginado<ActivoTransferencia>> ObtenerPaginadoAsync(Expression<Func<ActivoTransferencia, bool>> predicate = null, Func<IQueryable<ActivoTransferencia>, IOrderedQueryable<ActivoTransferencia>> orderBy = null, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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


        public Task<IEnumerable<ActivoTransferencia>> CrearAsync(params ActivoTransferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivoTransferencia>> CrearAsync(IEnumerable<ActivoTransferencia> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
