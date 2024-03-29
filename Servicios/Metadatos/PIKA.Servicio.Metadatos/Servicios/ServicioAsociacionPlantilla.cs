﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public class ServicioAsociacionPlantilla : ContextoServicioMetadatos, IServicioInyectable, IServicioAsociacionPlantilla
    {
        private const string DEFAULT_SORT_COL = "plantillaId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<AsociacionPlantilla> repo;
        private ICompositorConsulta<AsociacionPlantilla> compositor;
        
        public ServicioAsociacionPlantilla(
           IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS)
        {
            this.repo = UDT.ObtenerRepositoryAsync<AsociacionPlantilla>(compositor);
        }
        public async Task<bool> Existe(Expression<Func<AsociacionPlantilla, bool>> predicado)
        {
            throw new NotImplementedException();
            List<AsociacionPlantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<AsociacionPlantilla> CrearAsync(AsociacionPlantilla entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            if (await Existe(x => x.PlantillaId.Equals(entity.PlantillaId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.PlantillaId);
            }
            try
            {
                entity.Id = System.Guid.NewGuid().ToString();
                entity.PlantillaId = entity.PlantillaId.Trim();
                entity.TipoOrigenId = entity.TipoOrigenId.Trim();
                entity.OrigenId = entity.OrigenId.Trim();
                entity.IdentificadorAlmacenamiento = entity.IdentificadorAlmacenamiento.Trim();
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional(entity.PlantillaId);
            }
            catch (Exception)
            {
                throw;
            }
            

            return entity.Copia();
        }

        public async Task ActualizarAsync(AsociacionPlantilla entity)
        {
            throw new NotImplementedException();
            AsociacionPlantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id
            && x.PlantillaId.Equals(entity.PlantillaId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.PlantillaId);
            }

            o.PlantillaId = entity.PlantillaId.Trim();
            o.IdentificadorAlmacenamiento = entity.IdentificadorAlmacenamiento.Trim();
            try
            {
                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional(entity.PlantillaId);
    }
            catch (Exception)
            {
                throw;
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
        public async Task<IPaginado<AsociacionPlantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<AsociacionPlantilla>, IIncludableQueryable<AsociacionPlantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<AsociacionPlantilla>> CrearAsync(params AsociacionPlantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AsociacionPlantilla>> CrearAsync(IEnumerable<AsociacionPlantilla> entities, CancellationToken cancellationToken = default)
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
            throw new NotImplementedException();
            AsociacionPlantilla o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            await this.repo.Eliminar(o);
                        }
                        this.UDT.SaveChanges();
                        listaEliminados.Add(o.Id);
                    }
                    catch (DbUpdateException)
                    {
                        throw new ExErrorRelacional(Id);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;
        }

        public Task<List<AsociacionPlantilla>> ObtenerAsync(Expression<Func<AsociacionPlantilla, bool>> predicado)
        {
            throw new NotImplementedException();
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<AsociacionPlantilla> UnicoAsync(Expression<Func<AsociacionPlantilla, bool>> predicado = null, Func<IQueryable<AsociacionPlantilla>, IOrderedQueryable<AsociacionPlantilla>> ordenarPor = null, Func<IQueryable<AsociacionPlantilla>, IIncludableQueryable<AsociacionPlantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            AsociacionPlantilla d = await this.repo.UnicoAsync(predicado);

            return d.Copia();
        }

 public Task<List<AsociacionPlantilla>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
    
        #region Sin Implementar


        public Task<IPaginado<AsociacionPlantilla>> ObtenerPaginadoAsync(Expression<Func<AsociacionPlantilla, bool>> predicate = null, Func<IQueryable<AsociacionPlantilla>, IOrderedQueryable<AsociacionPlantilla>> orderBy = null, Func<IQueryable<AsociacionPlantilla>, IIncludableQueryable<AsociacionPlantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<AsociacionPlantilla> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
