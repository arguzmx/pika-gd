﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
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
   public class ServicioPropiedadPlantilla : IServicioInyectable, IServicioPropiedadPlantilla
    {
        //perame voy a buscar a mi exploradoroki
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IServicioCache cache;
        private IRepositorioAsync<PropiedadPlantilla> repo;
        private ICompositorConsulta<PropiedadPlantilla> compositor;
        private ILogger<ServicioPropiedadPlantilla> logger;
        private DbContextMetadatos contexto;
        private UnidadDeTrabajo<DbContextMetadatos> UDT;
        private IServicioTipoDatoPropiedadPlantilla servicioTipoDatoPropiedadPlantilla;
        public ServicioPropiedadPlantilla(DbContextMetadatos contexto,
            ICompositorConsulta<PropiedadPlantilla> compositorConsulta,
            ILogger<ServicioPropiedadPlantilla> Logger,
            IServicioCache servicioCache,
            IServicioTipoDatoPropiedadPlantilla servicioTipoDatoPropiedadPlantilla
            )
        {


            this.contexto = contexto;
            this.UDT = new UnidadDeTrabajo<DbContextMetadatos>(contexto);
            this.cache = servicioCache;
            this.logger = Logger;
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<PropiedadPlantilla>(compositor);
            this.servicioTipoDatoPropiedadPlantilla = servicioTipoDatoPropiedadPlantilla;
        }






        public async Task<bool> Existe(Expression<Func<PropiedadPlantilla, bool>> predicado)
        {
            List<PropiedadPlantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<PropiedadPlantilla> CrearAsync(PropiedadPlantilla entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);

            UDT.SaveChanges();

            try
            {
                var linkDatoPropiedad = await createtipodato(entity.TipoDatoId, entity.Id);


                return ClonaPropiedadPlantilla(entity);
            }
            catch (Exception ex)
            {
                await this.repo.Eliminar(entity);
                UDT.SaveChanges();
                throw (ex);
            }
            
        }
        public async Task<TipoDatoPropiedadPlantilla> createtipodato(string tipodatoid, string propiedadid)
        {
            TipoDatoPropiedadPlantilla t = new TipoDatoPropiedadPlantilla();
            t.TipoDatoId = tipodatoid;
            t.PropiedadPlantillaId = propiedadid;
            return await this.servicioTipoDatoPropiedadPlantilla.CrearAsync(t).ConfigureAwait(false);
            
        }


        private PropiedadPlantilla ClonaPropiedadPlantilla(PropiedadPlantilla entidad)
        {
            PropiedadPlantilla resuldtado = new PropiedadPlantilla()
            {
                Id = entidad.Id,
                PlantillaId = entidad.PlantillaId,
                Nombre = entidad.Nombre,
                TipoDatoId=entidad.TipoDatoId,
                IndiceOrdenamiento=entidad.IndiceOrdenamiento,
                Buscable=entidad.Buscable,
                Ordenable=entidad.Ordenable,
                Visible=entidad.Visible,
                EsIdClaveExterna=entidad.EsIdClaveExterna,
                EsIdRegistro=entidad.EsIdRegistro,
                EsIdJerarquia=entidad.EsIdJerarquia,
                EsTextoJerarquia=entidad.EsTextoJerarquia,
                EsFiltroJerarquia=entidad.EsFiltroJerarquia,
                EsIdPadreJerarquia=entidad.EsIdPadreJerarquia,
                EsIndice=entidad.EsIndice,
                Requerido=entidad.Requerido,
                Autogenerado=entidad.Autogenerado,
                ControlHTML=entidad.ControlHTML
            };

            return resuldtado;
        }

        private List<PropiedadPlantilla> ClonaPropiedadListaPlantilla(List<PropiedadPlantilla> entidades)
        {
            List<PropiedadPlantilla> resuldtado = new List<PropiedadPlantilla>();
             
            foreach(var elemento in entidades)
            {
                resuldtado.Add(ClonaPropiedadPlantilla(elemento));
            }

            return resuldtado;
        }


        public async Task ActualizarAsync(PropiedadPlantilla entity)
        {

            PropiedadPlantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Atributos = entity.Atributos;
            o.AtributoTabla = entity.AtributoTabla;
            o.Autogenerado = entity.Autogenerado;
            o.Buscable = entity.Buscable;
            o.ControlHTML = entity.ControlHTML;
            o.EsFiltroJerarquia = entity.EsFiltroJerarquia;
            o.EsIdClaveExterna = entity.EsIdClaveExterna;
            o.TipoDatoId = entity.TipoDatoId;
            o.EsIdRegistro = entity.EsIdRegistro;
            TipoDatoPropiedadPlantilla tipoDato = new TipoDatoPropiedadPlantilla();
            tipoDato.PropiedadPlantillaId = o.Id;
            tipoDato.TipoDatoId = o.TipoDatoId;
            if (! await servicioTipoDatoPropiedadPlantilla.Existe(x => x.PropiedadPlantillaId == o.Id))
            {
                await this.servicioTipoDatoPropiedadPlantilla.CrearAsync(tipoDato);
            }
            else
            {
                await servicioTipoDatoPropiedadPlantilla.ActualizarAsync(tipoDato);
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
        public async Task<IPaginado<PropiedadPlantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PropiedadPlantilla>, IIncludableQueryable<PropiedadPlantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<PropiedadPlantilla>> CrearAsync(params PropiedadPlantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PropiedadPlantilla>> CrearAsync(IEnumerable<PropiedadPlantilla> entities, CancellationToken cancellationToken = default)
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

        public Task Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<PropiedadPlantilla>> ObtenerAsync(Expression<Func<PropiedadPlantilla, bool>> predicado)
        {

            throw new NotImplementedException();
        }

        public Task<IEnumerable<PropiedadPlantilla>> ObtenerListaAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<PropiedadPlantilla>> ObtenerPaginadoAsync(Expression<Func<PropiedadPlantilla, bool>> predicate = null, Func<IQueryable<PropiedadPlantilla>, IOrderedQueryable<PropiedadPlantilla>> orderBy = null, Func<IQueryable<PropiedadPlantilla>, IIncludableQueryable<PropiedadPlantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<PropiedadPlantilla> UnicoAsync(Expression<Func<PropiedadPlantilla, bool>> predicado = null, Func<IQueryable<PropiedadPlantilla>, IOrderedQueryable<PropiedadPlantilla>> ordenarPor = null, Func<IQueryable<PropiedadPlantilla>, IIncludableQueryable<PropiedadPlantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }
    }
}