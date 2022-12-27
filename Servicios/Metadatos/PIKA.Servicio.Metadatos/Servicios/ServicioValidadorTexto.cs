using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public class ServicioValidadorTexto : ContextoServicioMetadatos,IServicioInyectable, IServicioValidadorTexto
    {
        private const string DEFAULT_SORT_COL = "propiedadId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ValidadorTexto> repo;
        private ICompositorConsulta<ValidadorTexto> compositor;

        public ServicioValidadorTexto(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS)
        {
            this.repo = UDT.ObtenerRepositoryAsync<ValidadorTexto>(new QueryComposer<ValidadorTexto>());
        }

        public async Task<bool> Existe(Expression<Func<ValidadorTexto, bool>> predicado)
        {
            List<ValidadorTexto> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<ValidadorTexto> CrearAsync(ValidadorTexto entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ValidadorTexto>();
            
            var propiedad = UDT.Context.PropiedadPlantilla.FirstOrDefault(x => x.Id == entity.PropiedadId);
            if (propiedad == null)
            {
                throw new ExElementoExistente(entity.PropiedadId);
            };

            if (!await seguridad.AccesoCachePlantillas(propiedad.PlantillaId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(propiedad.PlantillaId);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.PropiedadId);

            return entity.Copia();
        }

        public async Task ActualizarAsync(ValidadorTexto entity)
        {
            seguridad.EstableceDatosProceso<ValidadorTexto>();
            ValidadorTexto o = await this.repo.UnicoAsync(x => x.PropiedadId == entity.PropiedadId);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            var propiedad = UDT.Context.PropiedadPlantilla.FirstOrDefault(x => x.Id == o.PropiedadId);
            if (propiedad == null)
            {
                throw new ExElementoExistente(entity.PropiedadId);
            };

            if (!await seguridad.AccesoCachePlantillas(propiedad.PlantillaId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(propiedad.PlantillaId);
            }

            string original = o.Flat();

            o.PropiedadId = entity.PropiedadId;
            o.longmax = entity.longmax;
            o.longmin = entity.longmin;
            o.Propiedad = entity.Propiedad;
            o.regexp = entity.regexp;
            o.valordefault = entity.valordefault;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            await seguridad.RegistraEventoActualizar(o.Id, o.PropiedadId, original.JsonDiff(o.Flat()));
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
        public async Task<IPaginado<ValidadorTexto>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ValidadorTexto>, IIncludableQueryable<ValidadorTexto, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ValidadorTexto>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ValidadorTexto>> CrearAsync(params ValidadorTexto[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ValidadorTexto>> CrearAsync(IEnumerable<ValidadorTexto> entities, CancellationToken cancellationToken = default)
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
            seguridad.EstableceDatosProceso<ValidadorTexto>();
            List<ValidadorTexto> listaEliminados = new List<ValidadorTexto>();
            foreach (var Id in ids)
            {
                ValidadorTexto o = await this.UDT.Context.ValidadorTexto.FirstOrDefaultAsync(x => x.Id == Id);
                if (o != null)
                {
                    var propiedad = UDT.Context.PropiedadPlantilla.FirstOrDefault(x => x.Id == o.PropiedadId);
                    if (propiedad == null)
                    {
                        throw new ExElementoExistente(o.PropiedadId);
                    };

                    if (!await seguridad.AccesoCachePlantillas(propiedad.PlantillaId))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(propiedad.PlantillaId);
                    }
                    listaEliminados.Add(o);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach (var o in listaEliminados)
                {
                    UDT.Context.Entry(o).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(o.Id, o.PropiedadId);
                }
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x => x.Id).ToList();
        }

        public Task<List<ValidadorTexto>> ObtenerAsync(Expression<Func<ValidadorTexto, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<ValidadorTexto>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ValidadorTexto>> ObtenerPaginadoAsync(Expression<Func<ValidadorTexto, bool>> predicate = null, Func<IQueryable<ValidadorTexto>, IOrderedQueryable<ValidadorTexto>> orderBy = null, Func<IQueryable<ValidadorTexto>, IIncludableQueryable<ValidadorTexto, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidadorTexto> UnicoAsync(Expression<Func<ValidadorTexto, bool>> predicado = null, Func<IQueryable<ValidadorTexto>, IOrderedQueryable<ValidadorTexto>> ordenarPor = null, Func<IQueryable<ValidadorTexto>, IIncludableQueryable<ValidadorTexto, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            ValidadorTexto d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }

        public Task<ValidadorTexto> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
    }
}
