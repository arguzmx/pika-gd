using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using Parte = PIKA.Modelo.Contenido.Parte;
using Version = PIKA.Modelo.Contenido.Version;
namespace PIKA.Servicio.Contenido.Servicios
{
    public class ServicioParte : ContextoServicioContenido,
        IServicioInyectable, IServicioParte
    {
        private const string DEFAULT_SORT_COL = "Indice";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Parte> repo;
        private IRepositorioAsync<Version> repoVer;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private HelperVolumen helperVolumen;
        private HelperVersion helperVersion;
        public ServicioParte(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
        ILogger Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Parte>( new QueryComposer<Parte>());
            this.repoVer = UDT.ObtenerRepositoryAsync<Version>(new QueryComposer<Version>());
            helperVolumen = new HelperVolumen(UDT);
            helperVersion = new HelperVersion(UDT);
        }

        public async Task<bool> Existe(Expression<Func<Parte, bool>> predicado)
        {
            List<Parte> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        private async Task<string> VolmenIdDeVersion(string IdVersion)
        {
            string id = "";
            var p = await this.repoVer.UnicoAsync(x => x.Id == IdVersion, null,
                y => y.Include(z => z.Elemento));

            if (p != null)
            {
                return p.Elemento.VolumenId;
            }

            return id;
        }


        public async Task<Parte> CrearAsync(Parte entity, CancellationToken cancellationToken = default)
        {


            try
            {

                var v = await repoVer.UnicoAsync(x => x.Id == entity.VersionId);
                if (v == null) throw new EXNoEncontrado("Version:" + entity.VersionId);

                string volid = await this.VolmenIdDeVersion(v.Id);


                entity.Id = System.Guid.NewGuid().ToString();
                entity.Indice = v.MaxIndicePartes + 1;
                entity.ConsecutivoVolumen = await helperVolumen.GetConsecutivoVolumen(volid, entity.LongitudBytes)  ;
                logger.LogWarning(entity.ConsecutivoVolumen.ToString());
                entity.Eliminada = false;

                await helperVersion.CreaParte(entity);

                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();

                return entity.Copia();
            }
            catch (DbUpdateException ee)
            {
                logger.LogError(ee.ToString());
                throw new ExErrorRelacional("Alguno de los identificadores no es válido");
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear Unidad Organizacional {0}", ex.Message);
                throw ex;
            }
        }

   

        public async Task ActualizarAsync(Parte entity)
        {
                Parte o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.Id);
                }


            if (string.IsNullOrEmpty(entity.TipoMime))
                throw new ExDatosNoValidos($"El tipo mime no es valido {entity.TipoMime}");

                o.Indice = entity.Indice;
                o.TipoMime = entity.TipoMime;
                o.LongitudBytes = entity.LongitudBytes;

                if (o.LongitudBytes != entity.LongitudBytes)
                {
                    string volid = await this.VolmenIdDeVersion(o.VersionId);
                    if (!string.IsNullOrEmpty(volid))
                    {
                        await this.helperVolumen.ActualizaTamanoVolumen(volid, o.LongitudBytes, entity.LongitudBytes);
                    }

                await helperVersion.ActualizaParte(o, entity);
            }

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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Parte d;
            List<Parte> eliminadas = new List<Parte>();
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    eliminadas.Add(d.Copia());
                    d.Eliminada = true;
                    UDT.Context.Entry(d).State = EntityState.Modified;
                    lista.Add(d.Id);
                }
            }

            if (eliminadas.Count > 0)
            {
                await helperVersion.EliminaPartes(eliminadas);
            }

            UDT.SaveChanges();
            return lista;
        }


        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Parte d;
            List<Parte> restauradas = new List<Parte>();
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids.LimpiaIds())
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    restauradas.Add(d.Copia());
                    d.Eliminada = false;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    lista.Add(d.Id);
                }
            }

            if (restauradas.Count > 0)
            {
                await helperVersion.RestauraPartes(restauradas);
            }

            UDT.SaveChanges();
            return lista;
        }



        public async Task<Parte> UnicoAsync(Expression<Func<Parte, bool>> predicado = null, Func<IQueryable<Parte>, IOrderedQueryable<Parte>> ordenarPor = null, Func<IQueryable<Parte>, IIncludableQueryable<Parte, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            var o = await this.repo.UnicoAsync(predicado);

            return o.Copia();
        }

        #region No Implementado


        public Task<List<Parte>> ObtenerAsync(Expression<Func<Parte, bool>> predicado)
        {
            throw new NotImplementedException();
        }


        public Task<List<Parte>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<Parte>> CrearAsync(params Parte[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Parte>> CrearAsync(IEnumerable<Parte> entities, CancellationToken cancellationToken = default)
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

        public Task<IPaginado<Parte>> ObtenerPaginadoAsync(Expression<Func<Parte, bool>> predicate = null, Func<IQueryable<Parte>, IOrderedQueryable<Parte>> orderBy = null, Func<IQueryable<Parte>, IIncludableQueryable<Parte, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        #endregion





    }



}