using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
  public  class ServicioEntradaClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioEntradaClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<EntradaClasificacion> repo;
        private IRepositorioAsync<ValoracionEntradaClasificacion> repoEC;
        private IRepositorioAsync<TipoValoracionDocumental> repoTEC;
        private IRepositorioAsync<ElementoClasificacion> repoEL;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;

        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
         
        public ServicioEntradaClasificacion(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioEntradaClasificacion> Logger
           ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoEC = UDT.ObtenerRepositoryAsync<ValoracionEntradaClasificacion>(new QueryComposer<ValoracionEntradaClasificacion>());
            this.repoEL = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
            this.repoTD = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoTEC = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
        }


        public async Task<bool> Existe(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            List<EntradaClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<EntradaClasificacion> CrearAsync(EntradaClasificacion entity, CancellationToken cancellationToken = default)
        {
            if (await Existelemento(x => x.Id == entity.ElementoClasificacionId && x.Eliminada == true))
                throw new EXNoEncontrado(entity.ElementoClasificacionId);
            if (!await Existelemento(x => x.Id == entity.ElementoClasificacionId))
                throw new EXNoEncontrado(entity.ElementoClasificacionId);
            if(!String.IsNullOrEmpty( entity.TipoDisposicionDocumentalId))
            if (!await ExisteTipoDisposicionDocumental(x => x.Id == entity.TipoDisposicionDocumentalId))
                throw new ExDatosNoValidos(entity.TipoDisposicionDocumentalId);

            if (await Existe(x => x.Clave.Equals(entity.Clave.Trim(),
                StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
                && x.ElementoClasificacionId == entity.ElementoClasificacionId
                ))
            {
                throw new ExElementoExistente(entity.Clave.Trim());
            }
            
            entity.Id = System.Guid.NewGuid().ToString();
           entity.Nombre= entity.Nombre.Trim();
            entity.Clave=entity.Clave.Trim();
            if(!String.IsNullOrEmpty(entity.ElementoClasificacionId))
                entity.ElementoClasificacionId =entity.ElementoClasificacionId.Trim();
            if (!String.IsNullOrEmpty(entity.TipoDisposicionDocumentalId))
                entity.TipoDisposicionDocumentalId = entity.TipoDisposicionDocumentalId.Trim();
            await this.repo.CrearAsync(entity);


            if(entity.TipoValoracionDocumentalId != null 
                && entity.TipoValoracionDocumentalId.Length > 0)
            {


                foreach(string id in entity.TipoValoracionDocumentalId)
                {
                    var tipo = repoTEC.UnicoAsync(x => x.Id.Equals(id.Trim(), StringComparison.InvariantCultureIgnoreCase) );
                    if( tipo != null)
                    {
                        ValoracionEntradaClasificacion vec = new ValoracionEntradaClasificacion()
                        {
                            EntradaClasificacionId = entity.Id,
                            TipoValoracionDocumentalId = id.Trim()
                        };
                        await repoEC.CrearAsync(vec);
                    }
                }
            }

            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task<bool> Existelemento(Expression<Func<ElementoClasificacion, bool>> predicadoelemento)
        {
            List<ElementoClasificacion> l = await this.repoEL.ObtenerAsync(predicadoelemento);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteTipoDisposicionDocumental(Expression<Func<TipoDisposicionDocumental, bool>> predicadoelemento)
        {
            List<TipoDisposicionDocumental> l = await this.repoTD.ObtenerAsync(predicadoelemento);
            if (l.Count() == 0) return false;
            return true;
        }
        

        public async Task ActualizarAsync(EntradaClasificacion entity)
        {
            EntradaClasificacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            if (await Existelemento(x=>x.Id==entity.ElementoClasificacionId && x.Eliminada==true))
                    throw new EXNoEncontrado(entity.ElementoClasificacionId);
           if(!await Existelemento(x => x.Id == entity.ElementoClasificacionId))
                throw new EXNoEncontrado(entity.ElementoClasificacionId);
            if (!String.IsNullOrEmpty(entity.TipoDisposicionDocumentalId))
            if (!await ExisteTipoDisposicionDocumental(x => x.Id == entity.TipoDisposicionDocumentalId))
                throw new ExDatosNoValidos(entity.TipoDisposicionDocumentalId);

          if (await Existe(x =>x.Id != entity.Id 
          && x.Clave.Equals(entity.Clave.Trim(),StringComparison.InvariantCultureIgnoreCase)
          && x.Eliminada != true
          && x.ElementoClasificacionId == entity.ElementoClasificacionId
                ))
            {
                throw new ExElementoExistente(entity.Clave);
            }
            
            o.Nombre = entity.Nombre.Trim();
            o.Eliminada = entity.Eliminada;
            o.Clave = entity.Clave.Trim();
            o.DisposicionEntrada = entity.DisposicionEntrada;
            o.MesesVigenciConcentracion = entity.MesesVigenciConcentracion;
            o.MesesVigenciHistorico = entity.MesesVigenciHistorico;
            o.MesesVigenciTramite = entity.MesesVigenciTramite;
            o.Posicion = entity.Posicion;
            
            if(!String.IsNullOrEmpty(entity.TipoDisposicionDocumentalId))
                o.TipoDisposicionDocumentalId = entity.TipoDisposicionDocumentalId.Trim();

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            List<ValoracionEntradaClasificacion> lista = await repoEC.ObtenerAsync(x => x.EntradaClasificacionId.Equals( entity.Id.Trim() , StringComparison.InvariantCultureIgnoreCase));

            if (entity.TipoValoracionDocumentalId != null
                && entity.TipoValoracionDocumentalId.Length > 0)
            {

                // Añade las faltantes
                foreach (string id in entity.TipoValoracionDocumentalId)
                {
                    var tipo = repoTEC.UnicoAsync(x => x.Id.Equals(id.Trim(), StringComparison.InvariantCultureIgnoreCase));
                    if (tipo != null)
                    {

                    }
                    
                    if ((await repoEC.ObtenerAsync(x=>
                        x.TipoValoracionDocumentalId.Equals(id.Trim(), StringComparison.InvariantCultureIgnoreCase)
                        && x.EntradaClasificacionId.Equals(entity.Id.Trim(), StringComparison.InvariantCultureIgnoreCase)))
                        .Count() == 0)
                    {

                            ValoracionEntradaClasificacion vec = new ValoracionEntradaClasificacion()
                            {
                                EntradaClasificacionId = entity.Id,
                                TipoValoracionDocumentalId = id.Trim()
                            };
                            await repoEC.CrearAsync(vec);
                        UDT.SaveChanges();
                    }
                }
            } else
            {
                entity.TipoValoracionDocumentalId = new string [0];
            }

            List<string> elementos = entity.TipoValoracionDocumentalId.ToList();
            foreach (var item in lista)
            {
                if (elementos.IndexOf(item.TipoValoracionDocumentalId) < 0)
                {
                    logger.LogError($"- {item.TipoValoracionDocumentalId}");
                    UDT.Context.Entry(item).State = EntityState.Deleted;
                }
            }
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
        public async Task<bool> ValidaElementoCuadroClasificacion(string ElemtoCuadroClasificacionID)
        {
            if (!String.IsNullOrEmpty(ElemtoCuadroClasificacionID))
            {
                ElementoClasificacion C = await this.repoEL.UnicoAsync(x => x.Id == ElemtoCuadroClasificacionID);
                if (C != null)
                {
                    if (C.Eliminada == true)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<IPaginado<EntradaClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            foreach(var item in respuesta.Elementos)
            {
                item.TipoValoracionDocumentalId = await ObtieneValoracionDocumental(item); 
            }
            return respuesta;
        }

        private async Task<string[]> ObtieneValoracionDocumental(EntradaClasificacion item)
        {
            var l = await repoEC.ObtenerAsync(x => x.EntradaClasificacionId.Equals(item.Id, StringComparison.InvariantCultureIgnoreCase));
            item.TipoValoracionDocumentalId = new string[l.Count];
            for(int i=0; i < l.Count; i++)
            {
                item.TipoValoracionDocumentalId[i] = l[i].TipoValoracionDocumentalId;
            }
            return item.TipoValoracionDocumentalId;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            EntradaClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    try
                    {
                        c.Eliminada = true;
                        UDT.Context.Entry(c).State = EntityState.Modified;
                        listaEliminados.Add(c.Id);
                    }
                    catch (Exception)
                    { }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        private async Task<string> RestaurarNombre(string Clave, string ElementoCuadroClasificacionId, string id, string Nombre)
        {

            if (await Existe(x =>
        x.Id != id && x.Clave.Equals(Clave, StringComparison.InvariantCultureIgnoreCase)
        && x.Eliminada == false
         && x.ElementoClasificacionId == ElementoCuadroClasificacionId))
            {

                Nombre = Nombre + " restaurado " + DateTime.Now.Ticks;
            }
            else {
            }


            return Nombre;

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            EntradaClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    c.Nombre = await RestaurarNombre(c.Clave, c.ElementoClasificacionId, c.Id, c.Nombre);
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }



        public async Task<EntradaClasificacion> UnicoAsync(Expression<Func<EntradaClasificacion, bool>> predicado = null, Func<IQueryable<EntradaClasificacion>, IOrderedQueryable<EntradaClasificacion>> ordenarPor = null, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            EntradaClasificacion c = await this.repo.UnicoAsync(predicado);
            c.TipoValoracionDocumentalId = await ObtieneValoracionDocumental(c);
            return c.Copia();
        }

      
        
        public Task<List<EntradaClasificacion>> ObtenerAsync(Expression<Func<EntradaClasificacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }
        public Task<List<EntradaClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        #region No implmentados



        public Task<IEnumerable<EntradaClasificacion>> CrearAsync(params EntradaClasificacion[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EntradaClasificacion>> CrearAsync(IEnumerable<EntradaClasificacion> entities, CancellationToken cancellationToken = default)
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



        

     

        public Task<IPaginado<EntradaClasificacion>> ObtenerPaginadoAsync(Expression<Func<EntradaClasificacion, bool>> predicate = null, Func<IQueryable<EntradaClasificacion>, IOrderedQueryable<EntradaClasificacion>> orderBy = null, Func<IQueryable<EntradaClasificacion>, IIncludableQueryable<EntradaClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

      

        




        #endregion
    }
}