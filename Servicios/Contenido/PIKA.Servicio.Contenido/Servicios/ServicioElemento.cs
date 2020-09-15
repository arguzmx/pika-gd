using System;
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
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioElemento : ContextoServicioContenido,
        IServicioInyectable, IServicioElemento
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Elemento> repo;
        private IRepositorioAsync<Volumen> repoVol;
        private IRepositorioAsync<PuntoMontaje> repoPM;
        private IRepositorioAsync<VolumenPuntoMontaje> repoVPM;
        private IRepositorioAsync<PIKA.Modelo.Contenido.Version> repoVer;
        private IRepositorioAsync<Permiso> repoPerm;
        private IRepositorioAsync<Carpeta> repoCarpetas;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private HelperCarpetas helperCarpetas;

        public ServicioElemento(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioElemento> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Elemento>(new QueryComposer<Elemento>());
            this.repoVer = UDT.ObtenerRepositoryAsync<PIKA.Modelo.Contenido.Version>(new QueryComposer<PIKA.Modelo.Contenido.Version>());
            this.repoVol = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
            this.repoVPM = UDT.ObtenerRepositoryAsync<VolumenPuntoMontaje>(new QueryComposer<VolumenPuntoMontaje>());
            this.repoPerm = UDT.ObtenerRepositoryAsync<Permiso>(new QueryComposer<Permiso>());
            this.repoPM = UDT.ObtenerRepositoryAsync<PuntoMontaje>(new QueryComposer<PuntoMontaje>());
            this.repoCarpetas = UDT.ObtenerRepositoryAsync<Carpeta>(new QueryComposer<Carpeta>());
            this.helperCarpetas = new HelperCarpetas(this.repoCarpetas);
        }

        public async Task<bool> Existe(Expression<Func<Elemento, bool>> predicado)
        {
            List<Elemento> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        private async Task ValidaEntidad(Elemento entity, Elemento instancia,  bool esActualizacion)
        {
            if (!string.IsNullOrEmpty(entity.Nombre)) entity.Nombre = entity.Nombre.Trim();

            if (esActualizacion) {

                if (instancia == null)
                {
                    throw new EXNoEncontrado(entity.Id);
                }

                if (entity.VolumenId != instancia.VolumenId)
                {
                    throw new ExDatosNoValidos($"No es posible modificar el volumen de un elemento");
                }
            }
      
            if((await repoPM.UnicoAsync(x=>x.Id == entity.PuntoMontajeId)) == null)
            {
                throw new ExDatosNoValidos($"Punto de montaje no válido");
            }

            var vol = await repoVol.UnicoAsync(x => x.Id == entity.VolumenId);
            if (vol == null)
            {
                throw new ExDatosNoValidos($"Volumen {entity.VolumenId} inexistente");
            }
            else
            {
                if (vol.Eliminada || (!vol.ConfiguracionValida))
                {
                    throw new ExDatosNoValidos($"Volumen {entity.VolumenId} eliminado o sin configuración válida");
                }
            }

            if (await Existe(x => x.Nombre == entity.Nombre
                  && x.PuntoMontajeId == entity.PuntoMontajeId
                 && x.CarpetaId == entity.CarpetaId && x.Eliminada == false && x.Id != entity.Id))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            if (!string.IsNullOrEmpty(entity.CarpetaId))
            {
                var carpeta = await repoCarpetas.UnicoAsync(x => x.Id == entity.CarpetaId);
                if (carpeta == null)
                {
                    throw new ExDatosNoValidos($"Carpeta {entity.CarpetaId} inexistente");
                }
                else
                {
                    var lista = await this.repoVPM.ObtenerAsync(x => x.PuntoMontajeId == carpeta.PuntoMontajeId
                    && x.VolumenId == entity.VolumenId);
                    if(lista.Count == 0)
                    {
                        throw new ExDatosNoValidos($"Carpeta {entity.CarpetaId} no pertenece al volumen {entity.VolumenId}");
                    }

                    if (await this.helperCarpetas.EsCarpetaEliminada(entity.CarpetaId))
                    {
                        throw new ExDatosNoValidos($"Carpeta {entity.CarpetaId} eliminada");
                    }  

                    
                }
            }

            if (!string.IsNullOrEmpty(entity.PermisoId))
            {
                var perm = await repoPerm.UnicoAsync(x => x.Id == entity.PermisoId);
                if (perm == null)
                {
                    throw new ExDatosNoValidos($"Permiso {entity.PermisoId} inexistente");
                }
            }

        }

        public async Task<Elemento> CrearAsync(Elemento entity, CancellationToken cancellationToken = default)
        {
            await this.ValidaEntidad(entity, null, false);
            try
            {

                entity.Id = System.Guid.NewGuid().ToString();
                entity.FechaCreacion = DateTime.Now;
                entity.Eliminada = false;
                entity.Versionado = false;

                await this.repo.CrearAsync(entity);

                PIKA.Modelo.Contenido.Version v = new Version()
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Activa = true,
                    CreadorId = entity.CreadorId,
                    ElementoId = entity.Id,
                    Eliminada = false,
                    FechaCreacion = entity.FechaCreacion
                };

                await this.repoVer.CrearAsync(v);


                UDT.SaveChanges();

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de relació no es válido");
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear Unidad Organizacional {0}", ex.Message);
                throw ex;
            }
           

        }

        public async Task ActualizarAsync(Elemento entity)
        {

            Elemento o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

       

            await this.ValidaEntidad(entity, o, true);
        

            try
            {
                o.Nombre = entity.Nombre;
                o.CarpetaId = entity.CarpetaId;
                o.Eliminada = entity.Eliminada;
                o.PermisoId = entity.PermisoId;
                o.Versionado = entity.Versionado;

                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de la Elemento padre no es válido");
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear Unidad Organizacional {0}", ex.Message);
                throw ex;
            }

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
        public async Task<IPaginado<Elemento>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Elemento>, IIncludableQueryable<Elemento, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Elemento o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    o.Eliminada = true;
                    UDT.Context.Entry(o).State = EntityState.Modified;
                    listaEliminados.Add(o.Id);

                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Elemento o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    o.Eliminada = false;
                    UDT.Context.Entry(o).State = EntityState.Modified;
                    listaEliminados.Add(o.Id);
                }

            }
            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<Elemento> UnicoAsync(Expression<Func<Elemento, bool>> predicado = null, Func<IQueryable<Elemento>, IOrderedQueryable<Elemento>> ordenarPor = null, Func<IQueryable<Elemento>, IIncludableQueryable<Elemento, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Elemento d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);


            return d.Copia();
        }

        #region No Implemenatdaos

        public Task<IEnumerable<Elemento>> CrearAsync(params Elemento[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Elemento>> CrearAsync(IEnumerable<Elemento> entities, CancellationToken cancellationToken = default)
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

        public Task<List<Elemento>> ObtenerAsync(Expression<Func<Elemento, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Elemento>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Elemento>> ObtenerPaginadoAsync(Expression<Func<Elemento, bool>> predicate = null, Func<IQueryable<Elemento>, IOrderedQueryable<Elemento>> orderBy = null, Func<IQueryable<Elemento>, IIncludableQueryable<Elemento, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }




        #endregion
    }

}