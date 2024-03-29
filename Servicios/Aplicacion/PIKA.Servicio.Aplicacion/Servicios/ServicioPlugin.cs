﻿using System;
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
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Servicio.AplicacionPlugin.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{

    public class ServicioPlugin : ContextoServicioAplicacion
        , IServicioInyectable, IServicioPlugin

    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Plugin> repo;
        private ICompositorConsulta<Plugin> compositor;
        private ICompositorConsulta<PluginInstalado> compositorPI;
        private ICompositorConsulta<VersionPlugin> compositorVP;
        private UnidadDeTrabajo<DbContextAplicacion> UDT;
        public IServicioVersionPlugin ServicioVersionPlugin;
        private IProveedorOpcionesContexto<DbContextAplicacion> DB;
        private IServicioCache servicioCacheData;
        private ILogger<ServicioLog> Logger;
        public ServicioPlugin(
         IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones,
         ICompositorConsulta<Plugin> compositorConsulta,
         ILogger<ServicioLog> Logger,
         IServicioCache servicioCache) : base(proveedorOpciones, Logger, servicioCache)

        {
            this.UDT = new UnidadDeTrabajo<DbContextAplicacion>(contexto);
            this.compositor = compositorConsulta;
            this.repo = UDT.ObtenerRepositoryAsync<Plugin>(compositor);
        }

        public async Task<bool> Existe(Expression<Func<Plugin, bool>> predicado)
        {
            List<Plugin> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Plugin> CrearAsync(Plugin entity, CancellationToken cancellationToken = default)
        {
        
            try
            {
                if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
                entity.Id = System.Guid.NewGuid().ToString();
                entity.versionPlugins.FirstOrDefault().Id = System.Guid.NewGuid().ToString();
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();


                return ClonaPlugin(entity);
            }
            catch (Exception ex)
            {
                await this.repo.Eliminar(entity);
                UDT.SaveChanges();
                throw (ex);
            }
        }

        private Plugin ClonaPlugin(Plugin entidad)
        {
            Plugin resuldtado = new Plugin()
            {
                Id = entidad.Id,
                Gratuito = entidad.Gratuito,
                Nombre = entidad.Nombre
                
            };

            return resuldtado;
        }

        public async Task ActualizarAsync(Plugin entity)
        {

            Plugin o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

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

            //o.VersionPluiginid = entity.VersionPluiginid;
            o.Gratuito = entity.Gratuito;
           
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
        public async Task<IPaginado<Plugin>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Plugin>, IIncludableQueryable<Plugin, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public Task<IEnumerable<Plugin>> CrearAsync(params Plugin[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Plugin>> CrearAsync(IEnumerable<Plugin> entities, CancellationToken cancellationToken = default)
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
            Plugin o;
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

        public Task<List<Plugin>> ObtenerAsync(Expression<Func<Plugin, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Plugin>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Plugin>> ObtenerPaginadoAsync(Expression<Func<Plugin, bool>> predicate = null, Func<IQueryable<Plugin>, IOrderedQueryable<Plugin>> orderBy = null, Func<IQueryable<Plugin>, IIncludableQueryable<Plugin, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Plugin> UnicoAsync(Expression<Func<Plugin, bool>> predicado = null, Func<IQueryable<Plugin>, IOrderedQueryable<Plugin>> ordenarPor = null, Func<IQueryable<Plugin>, IIncludableQueryable<Plugin, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Plugin d = await this.repo.UnicoAsync(predicado);
            return d.CopiaPlugin();
        }

        public async Task<List<string>> Purgar()
        {
            ServicioPluginInstalado spi = new ServicioPluginInstalado(DB,compositorPI,Logger,servicioCacheData);
            List<Plugin>listap = await this.repo.ObtenerAsync(x=>x.Eliminada==true).ConfigureAwait(false);
            string[] IDsELiminar = listap.Select(x=>x.Id).ToArray();
            await spi.Eliminar(IDsELiminar).ConfigureAwait(false);
            return IDsELiminar.ToList();
        }
    }
}
