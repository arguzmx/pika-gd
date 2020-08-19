
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
    public class ServicioTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTransferencia
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Transferencia> repo;
        private IRepositorioAsync<EstadoTransferencia> repoET;
        private IRepositorioAsync<Archivo> repoA;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioTransferencia(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, ILogger<ServicioCuadroClasificacion> Logger) : base(proveedorOpciones,Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.repoET = UDT.ObtenerRepositoryAsync<EstadoTransferencia>(new QueryComposer<EstadoTransferencia>());
            this.repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
        }
        public async Task<bool> Existe(Expression<Func<Transferencia, bool>> predicado)
        {
            List<Transferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteET(Expression<Func<EstadoTransferencia, bool>> predicado)
        {
            List<EstadoTransferencia> l = await this.repoET.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteA(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repoA.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<Transferencia> CrearAsync(Transferencia entity, CancellationToken cancellationToken = default)
        {
            if (!await ExisteET(x => x.Id.Equals(entity.EstadoTransferenciaId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional( entity.EstadoTransferenciaId); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.ArchivoOrigenId); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional( entity.ArchivoDestinoId); }
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExElementoExistente(entity.Nombre); }
            
            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }
        public async Task ActualizarAsync(Transferencia entity)
        {
            Transferencia o = await this.repo.UnicoAsync(x => x.Id.Trim() == entity.Id.Trim());

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoOrigenId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional( entity.ArchivoOrigenId.Trim()); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoDestinoId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.ArchivoDestinoId.Trim()); }
            if (!await ExisteET(x => x.Id.Equals(entity.EstadoTransferenciaId.Trim(), StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.EstadoTransferenciaId.Trim()); }
            if (await Existe(x =>
            x.Id.Trim() != entity.Id.Trim()
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            
            entity.Id = entity.Id.Trim();
            o.Nombre = entity.Nombre.Trim();
            o.FechaCreacion = DateTime.UtcNow;

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
        public async Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Transferencia a;
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
        public Task<List<Transferencia>> ObtenerAsync(Expression<Func<Transferencia, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Transferencia>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public async Task<Transferencia> UnicoAsync(Expression<Func<Transferencia, bool>> predicado = null, Func<IQueryable<Transferencia>, IOrderedQueryable<Transferencia>> ordenarPor = null, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Transferencia t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }

        #region Sin Implementar
       

        public Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(Expression<Func<Transferencia, bool>> predicate = null, Func<IQueryable<Transferencia>, IOrderedQueryable<Transferencia>> orderBy = null, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Transferencia>> CrearAsync(params Transferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transferencia>> CrearAsync(IEnumerable<Transferencia> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        #endregion



    }
}
