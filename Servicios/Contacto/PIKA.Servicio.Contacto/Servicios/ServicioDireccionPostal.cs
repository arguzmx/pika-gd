﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Contacto;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contacto
{
    public class ServicioDireccionPostal : ContextoServicioOContacto,
        IServicioInyectable, IServicioDireccionPostal
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<DireccionPostal> repo;
        private IRepositorioAsync<Pais> repoPais;
        private IRepositorioAsync<Estado> repoEstado;
        private UnidadDeTrabajo<DbContextContacto> UDT;
        
        public ServicioDireccionPostal(
            IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones,
            ILogger<ServicioDireccionPostal> Logger) : 
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContacto>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<DireccionPostal>(
                new QueryComposer<DireccionPostal>());

            this.repoPais = UDT.ObtenerRepositoryAsync<Pais>(
                new QueryComposer<Pais>());

            this.repoEstado = UDT.ObtenerRepositoryAsync<Estado>(
                new QueryComposer<Estado>());
        }

        public async Task<bool> Existe(Expression<Func<DireccionPostal, bool>> predicado)
        {
            List<DireccionPostal> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<DireccionPostal> CrearAsync(DireccionPostal entity, CancellationToken cancellationToken = default)
        {

            if (!string.IsNullOrEmpty(entity.PaisId)) {
                var p = await this.repoPais.UnicoAsync(x => x.Id == entity.PaisId);
                if (p == null) throw new EXNoEncontrado($"PaisId: { entity.PaisId } ");
            }

            if (!string.IsNullOrEmpty(entity.EstadoId))
            {
                var e = await this.repoEstado.UnicoAsync(x => x.Id == entity.EstadoId);
                if (e == null) throw new EXNoEncontrado($"EstadoId: { entity.EstadoId } ");
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            // Los datos en las direcciones postales puede repetirse  
            // pero sólo puece existir una por defualt
            if (entity.EsDefault) await ActualizaDefault(entity);

            return entity.Copia();
        }

        /// <summary>
        /// Establece las direcciones distintas a EsDeafult=false
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task ActualizaDefault(DireccionPostal entity)
        {
            List<DireccionPostal> l = await this.repo.ObtenerAsync(
                x => x.EsDefault == true
                && x.TipoOrigenId == entity.TipoOrigenId &&
                x.OrigenId == entity.OrigenId && x.Id != entity.Id);

            foreach (var item in l)
            {
                item.EsDefault = false;
                UDT.Context.Entry(item).State = EntityState.Modified;
            }
            UDT.SaveChanges();
        }

        public async Task ActualizarAsync(DireccionPostal entity)
        {

            DireccionPostal o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            o.Nombre = entity.Nombre;
            o.Calle = entity.Calle;
            o.NoInterno = entity.NoInterno;
            o.NoExterno = entity.NoExterno;
            o.Colonia = entity.Colonia;
            o.CP = entity.CP;
            o.Municipio = entity.Municipio;
            o.EstadoId = entity.EstadoId;
            o.PaisId = entity.PaisId;
            o.EsDefault = entity.EsDefault;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            if (o.EsDefault) await ActualizaDefault(o);
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
                query = new Consulta() { indice = 0, tamano = 20, ord_columna= DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<DireccionPostal>> ObtenerPaginadoAsync(Consulta Query, 
            Func<IQueryable<DireccionPostal>, IIncludableQueryable<DireccionPostal, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync (Query, include);

            return respuesta;
        }



        public async Task EjecutarSql(string sqlCommand)
        {
            await  this.contexto.Database.ExecuteSqlRawAsync(sqlCommand);
        }

        public async Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            foreach(string s in sqlCommand)
            {
                await this.contexto.Database.ExecuteSqlRawAsync(s);
            }
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            DireccionPostal dp;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                dp = await this.repo.UnicoAsync(x => x.Id == Id);
                if (dp != null)
                {
                    await this.repo.Eliminar(dp);
                    listaEliminados.Add(dp.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }


        public async Task EstablecerPrincipal(string Id)
        {
            DireccionPostal dp;
            dp = await this.repo.UnicoAsync(x => x.Id == Id);
            if (dp != null)
            {
                await this.ActualizaDefault(dp);
                dp.EsDefault = true;
                UDT.Context.Entry(dp).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }


        public async Task RemoverPrincipal(string Id)
        {
            DireccionPostal dp;
            dp = await this.repo.UnicoAsync(x => x.Id == Id);
            if (dp != null)
            {
                dp.EsDefault = true;
                UDT.Context.Entry(dp).State = EntityState.Modified;
                UDT.SaveChanges();
            }
        }

        public async Task<DireccionPostal> UnicoAsync(Expression<Func<DireccionPostal, bool>> predicado = null, Func<IQueryable<DireccionPostal>, IOrderedQueryable<DireccionPostal>> ordenarPor = null, Func<IQueryable<DireccionPostal>, IIncludableQueryable<DireccionPostal, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            
            DireccionPostal d = await this.repo.UnicoAsync(predicado, null, incluir);
            return d.Copia();
        }

        
        public Task<List<DireccionPostal>> ObtenerAsync(Expression<Func<DireccionPostal, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        #region sin implementar

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<List<DireccionPostal>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<DireccionPostal>> ObtenerPaginadoAsync(Expression<Func<DireccionPostal, bool>> predicate = null, Func<IQueryable<DireccionPostal>, IOrderedQueryable<DireccionPostal>> orderBy = null, Func<IQueryable<DireccionPostal>, IIncludableQueryable<DireccionPostal, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<DireccionPostal>> CrearAsync(params DireccionPostal[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DireccionPostal>> CrearAsync(IEnumerable<DireccionPostal> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
