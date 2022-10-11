
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Nest;
using Nest.Specification.TransformApi;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
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

        public ServicioActivoTransferencia(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            ILogger<ServicioLog> Logger) 
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
 
      

        public async Task<ActivoTransferencia> CrearAsync(ActivoTransferencia entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.ActivoId == entity.ActivoId && x.TransferenciaId ==entity.TransferenciaId))
            {
                throw new ExElementoExistente(entity.ActivoId);
            }

            var t = await this.UDT.Context.Transferencias.Where(x => x.Id == entity.TransferenciaId).FirstOrDefaultAsync();
            if(t==null)
            {
                throw new EXNoEncontrado();
            }


            List<string> validos = await this.UDT.Context.ActivosValidosTransferencia( new List<string>() { entity.ActivoId }, t.ArchivoOrigenId);
            if(validos.Count>0)
            {
                entity.Id = Guid.NewGuid().ToString();
                entity.Declinado = false;
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();

                await this.UDT.Context.ActualizaConteoActivosTrasnferencia(1, entity.TransferenciaId);
                await this.UDT.Context.ActualizaActivosEnTrasnferencia(new List<string>() {  entity.ActivoId }, false);
                return entity.Copia();
            } else
            {
                throw new ExErrorRelacional("APICODE-ACTIVOTRANSFERENCIA-DATOSINCORRECTOS");
            }


        }

        public async Task ActualizarAsync(ActivoTransferencia entity)
        {
            throw new NotImplementedException();
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
                    await this.repo.Eliminar(a);
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

        public async Task<ICollection<string>> EliminarActivoTransferencia(string[] ids)
        {
            ActivoTransferencia a;
            int conteo = 0;
            string txId = "";
            List<string> listaEliminados = new List<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    txId = a.TransferenciaId;
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.ActivoId);
                    conteo--;
                }
            }
            UDT.SaveChanges();
            await this.UDT.Context.ActualizaActivosEnTrasnferencia(listaEliminados, false);
            await this.UDT.Context.ActualizaConteoActivosTrasnferencia(conteo, txId);

            return listaEliminados;
        }
        
        
        public async Task EliminarActivosTransferencia( string TransferenciaId)
        {
            ActivoTransferencia a;
            var listaEliminados = await this.UDT.Context.ActivosTransferencia.Where(x => x.TransferenciaId == TransferenciaId).ToListAsync();
            if(listaEliminados.Count>0)
            {
                this.UDT.Context.RemoveRange(listaEliminados);
                UDT.SaveChanges();
            }
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
