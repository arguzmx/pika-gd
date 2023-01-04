using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Metadatos;
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public class ServicioPropiedadPlantilla : ContextoServicioMetadatos, IServicioInyectable, IServicioPropiedadPlantilla
    {

        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PropiedadPlantilla> repo;
        private IRepositorioMetadatos repositorioMetadatos;

        public ServicioPropiedadPlantilla(
           IRepositorioMetadatos repositorioMetadatos,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS)
        {
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<PropiedadPlantilla>());
            this.repositorioMetadatos = repositorioMetadatos;
        }

        public async Task<bool> Existe(Expression<Func<PropiedadPlantilla, bool>> predicado)
        {
            List<PropiedadPlantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        private PropiedadPlantilla ValidaPropiedadPlantilla(PropiedadPlantilla p , bool esActualizar)
        {

            
            if (!this.contexto.Plantilla.Where(x => x.Id.Equals(p.PlantillaId)).Any())
                throw new ExErrorRelacional(p.PlantillaId);

            if(!esActualizar)
            {
                if (!this.contexto.TipoDato.Where(x => x.Id.Equals(p.TipoDatoId)).Any())
                    throw new ExErrorRelacional(p.TipoDatoId);
            }
            

            if (esActualizar)
            {
                if (this.contexto.PropiedadPlantilla.Where(x => 
                x.PlantillaId == p.PlantillaId && x.Id!=p.Id
                && x.Nombre.Equals(p.Nombre.Trim())).Any())
                {
                    throw new ExElementoExistente(p.Nombre);
                }
            } else
            {
                if (this.contexto.PropiedadPlantilla.Where(x =>
                x.PlantillaId == p.PlantillaId
                && x.Nombre.Equals(p.Nombre.Trim())).Any())
                {
                    throw new ExElementoExistente(p.Nombre);
                }
                try
                {
                    p.IndiceOrdenamiento = this.contexto.PropiedadPlantilla
                        .Where(x=>x.PlantillaId==p.PlantillaId)
                        .Max(x=>x.IndiceOrdenamiento);
                }
                catch (Exception ex)
                {
                    p.IndiceOrdenamiento = 1;
                }

                
                p.IndiceOrdenamiento++;
                p.Id = Guid.NewGuid().ToString();
            }

            p.ControlHTML = string.IsNullOrEmpty(p.ControlHTML) ? "NONE" : p.ControlHTML;
            return p;

        }

        public async Task<PropiedadPlantilla> CrearAsync(PropiedadPlantilla entity, CancellationToken cancellationToken = default)
        {

            seguridad.EstableceDatosProceso<PropiedadPlantilla>();
            if (!await seguridad.AccesoCachePlantillas(entity.PlantillaId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(entity.PlantillaId);
            }

            entity = ValidaPropiedadPlantilla(entity, false);
            var elementos = await this.repo.ObtenerAsync(x => x.PlantillaId == entity.PlantillaId);

            entity.IdNumericoPlantilla = elementos.Count == 0 ? 1 : elementos.Max(x => x.IdNumericoPlantilla) + 1;
            await this.repo.CrearAsync(entity);

            UDT.SaveChanges();

            await PlantillaActualizada(entity.PlantillaId);

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }


        private async Task PlantillaActualizada(string Id)
        {


            var p = await this.UDT.Context.Plantilla.Where(x => x.Id == Id).FirstOrDefaultAsync();


            if (p != null)
            {
                var tipoDatos = await this.UDT.Context.TipoDato.ToListAsync();
                p.Propiedades = await this.UDT.Context.PropiedadPlantilla.Where(x => x.PlantillaId == p.Id).ToListAsync();
                foreach (var prop in p.Propiedades)
                {
                    prop.TipoDato = tipoDatos.Where(t => t.Id == prop.TipoDatoId).FirstOrDefault();
                    

                    if (prop.TipoDatoId == TipoDato.tInt32 ||
                        prop.TipoDatoId == TipoDato.tInt64 ||
                        prop.TipoDatoId == TipoDato.tDouble)
                    {
                        prop.ValidadorNumero = await this.UDT.Context.ValidadorNumero.Where(x => x.PropiedadId == prop.Id).SingleOrDefaultAsync();
                    }

                    if (prop.TipoDatoId == TipoDato.tString)
                    {
                        prop.ValidadorTexto = await this.UDT.Context.ValidadorTexto.Where(x => x.PropiedadId == prop.Id).SingleOrDefaultAsync();
                    }

                    if (prop.TipoDatoId == TipoDato.tList)
                    {
                        prop.ValoresLista = await this.UDT.Context.ValoresListaPropiedad.Where(x => x.PropiedadId == prop.Id).ToListAsync();
                    }

                }
                await this.repositorioMetadatos.ActualizarIndice(p);

            }

        }

        public async Task ActualizarAsync(PropiedadPlantilla entity)
        {
            seguridad.EstableceDatosProceso<PropiedadPlantilla>();
            PropiedadPlantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (!await seguridad.AccesoCachePlantillas(o.PlantillaId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(o.PlantillaId);
            }

            string original = o.Flat();
            entity = ValidaPropiedadPlantilla(entity, true);

            o.IndiceOrdenamiento = entity.IndiceOrdenamiento;
            o.Nombre = entity.Nombre;
            o.Requerido = entity.Requerido;
            o.AtributoTabla = entity.AtributoTabla;
            o.Autogenerado = entity.Autogenerado;
            o.Buscable = entity.Buscable;
            o.ControlHTML = entity.ControlHTML;
            o.EsFiltroJerarquia = entity.EsFiltroJerarquia;
            o.EsIdClaveExterna = entity.EsIdClaveExterna;
            o.EsIdRegistro = entity.EsIdRegistro;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(o.Flat()));
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

        public async Task<IPaginado<PropiedadPlantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PropiedadPlantilla>, IIncludableQueryable<PropiedadPlantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PropiedadPlantilla>();
            if (!Query.Filtros.Any(x=>x.Propiedad == "PlantillaId"))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            return respuesta;
        }


        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<PropiedadPlantilla>();
            List<PropiedadPlantilla> listaEliminados = new List<PropiedadPlantilla>();
            foreach (var Id in ids)
            {
                PropiedadPlantilla pp = await this.repo.UnicoAsync(x => x.Id == Id);
                if (pp != null)
                {

                    if (!await seguridad.AccesoCachePlantillas(pp.PlantillaId))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(pp.PlantillaId);
                    }
                    listaEliminados.Add(pp);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var pp in listaEliminados)
                {
                    UDT.Context.Entry(pp).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(pp.Id, pp.Nombre);
                }
                UDT.SaveChanges();
            }
            
            return listaEliminados.Select(x=>x.Id).ToList();
        }

        public async Task<PropiedadPlantilla> UnicoAsync(Expression<Func<PropiedadPlantilla, bool>> predicado = null, Func<IQueryable<PropiedadPlantilla>, IOrderedQueryable<PropiedadPlantilla>> ordenarPor = null, Func<IQueryable<PropiedadPlantilla>, IIncludableQueryable<PropiedadPlantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            PropiedadPlantilla d = await this.repo.UnicoAsync(predicado);
            if (!await seguridad.AccesoCachePlantillas(d.PlantillaId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(d.PlantillaId);
            }
            return d.Copia();
        }

        #region Sin Implementar

        public Task<IEnumerable<PropiedadPlantilla>> CrearAsync(params PropiedadPlantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PropiedadPlantilla>> CrearAsync(IEnumerable<PropiedadPlantilla> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<List<PropiedadPlantilla>> ObtenerAsync(Expression<Func<PropiedadPlantilla, bool>> predicado)
        {

            throw new NotImplementedException();
        }

        public Task<List<PropiedadPlantilla>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<PropiedadPlantilla>> ObtenerPaginadoAsync(Expression<Func<PropiedadPlantilla, bool>> predicate = null, Func<IQueryable<PropiedadPlantilla>, IOrderedQueryable<PropiedadPlantilla>> orderBy = null, Func<IQueryable<PropiedadPlantilla>, IIncludableQueryable<PropiedadPlantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<PropiedadPlantilla> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
