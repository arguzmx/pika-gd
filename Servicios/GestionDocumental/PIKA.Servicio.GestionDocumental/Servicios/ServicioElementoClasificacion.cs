using Bogus.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.GestorDocumental;
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
    public class ServicioElementoClasificacion : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioElementoClasificacion
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ElementoClasificacion> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IRepositorioAsync<CuadroClasificacion> RepoCuadro;
        private IRepositorioAsync<EntradaClasificacion> RepoEntrda;
        private IRepositorioAsync<TipoArchivo> repoTA;
        private IRepositorioAsync<TipoDisposicionDocumental> repoTD;
        private IRepositorioAsync<TipoValoracionDocumental> repoTVD;
        private readonly ConfiguracionServidor ConfiguracionServidor;
        private ILogger<ServicioCuadroClasificacion> LoggerCuadro;
        private ILogger<ServicioEntradaClasificacion> LoggerentradaCuadro;
        private IOCuadroClasificacion ioCuadroClasificacion;
        IOptions<ConfiguracionServidor> Config;

        public ServicioElementoClasificacion(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioElementoClasificacion> Logger, IOptions<ConfiguracionServidor> Config) : base(proveedorOpciones, Logger)
        {
            this.ConfiguracionServidor = Config.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ElementoClasificacion>(new QueryComposer<ElementoClasificacion>());
            this.RepoCuadro= UDT.ObtenerRepositoryAsync<CuadroClasificacion>(new QueryComposer<CuadroClasificacion>());
            this.RepoEntrda = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            this.repoTA = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
            this.repoTD = UDT.ObtenerRepositoryAsync<TipoDisposicionDocumental>(new QueryComposer<TipoDisposicionDocumental>());
            this.repoTVD = UDT.ObtenerRepositoryAsync<TipoValoracionDocumental>(new QueryComposer<TipoValoracionDocumental>());
            this.ioCuadroClasificacion = new IOCuadroClasificacion(LoggerCuadro, proveedorOpciones);
            this.Config = Config;
        }
        public async Task<bool> Existe(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            List<ElementoClasificacion> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<ElementoClasificacion> CrearAsync(ElementoClasificacion entity, CancellationToken cancellationToken = default)
        {
       
                if (entity.EsRaiz)
                {
                    entity.ElementoClasificacionId = null;
                }
                else
                {
                    if (String.IsNullOrEmpty(entity.ElementoClasificacionId))
                    {
                        throw new ExDatosNoValidos("ElementoClasificacionId");
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(entity.ElementoClasificacionId))
                            if (await ValidaElementoCuadroClasificacion(entity.ElementoClasificacionId))
                                throw new ExDatosNoValidos(entity.ElementoClasificacionId);

                        entity.ElementoClasificacionId = entity.ElementoClasificacionId.Trim();
                    }
                }

                if (await ValidaCuadroClasificacion(entity.CuadroClasifiacionId))
                    throw new ExDatosNoValidos(entity.CuadroClasifiacionId);



                if (await Existe(x => x.Clave.Equals(entity.Clave.Trim(),
                    StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
                    && x.CuadroClasifiacionId == entity.CuadroClasifiacionId
                    ))
                {
                    throw new ExElementoExistente(entity.Clave);
                }

                entity.Id = System.Guid.NewGuid().ToString().Trim();
                entity.Clave = entity.Clave.Trim();
                entity.Nombre = entity.Nombre.Trim();
                entity.CuadroClasifiacionId = entity.CuadroClasifiacionId.Trim();


                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();
            await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(entity.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);

            return entity.Copia();
    
        }
       public async Task<bool> ValidaCuadroClasificacion(string CuadroClasificacionID)
        {
            if (!String.IsNullOrEmpty(CuadroClasificacionID))
            {
                CuadroClasificacion C = await this.RepoCuadro.UnicoAsync(x => x.Id == CuadroClasificacionID);
                if (C != null)
                {
                    if (C.Eliminada == true)
                        return true;
                    else
                        return false;
                }
                else {
                    return true;
                }
            }
            else {
                return false;
            }
        }
        public async Task<bool> ValidaElementoCuadroClasificacion(string ElemtoCuadroClasificacionID)
        {
            if (!String.IsNullOrEmpty(ElemtoCuadroClasificacionID))
            {
                ElementoClasificacion C = await this.repo.UnicoAsync(x => x.Id == ElemtoCuadroClasificacionID);
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
        public async Task<List<ElementoClasificacion>> LitsElement(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            List<ElementoClasificacion> l = await this.repo.ObtenerAsync(predicado);
           
            return l;
        }

        /// <summary>
        /// Obtine el status del padre  
        /// </summary>
        /// <param name="idelement">Id del padre del elemento</param>
        /// <returns></returns> 
        public async Task<bool> PadreEliminado(string idelement) {
            // esta prueba no la vamos a hacer la voy a implementa yo despues del commit
            // pero me quedo con la idea de que no puedes implementar codigo nuevo
            // revisemos todo lo demas ok, que tutorial seguiste par aprender programación recursiva?
            // muestrame la pagina un video turorial 
            await Task.Delay(0);
            return true;
       }
        public async Task<bool> ParientePadre(string Id)
        {
            bool r = false;
            List<ElementoClasificacion> El= await LitsElement(x => x.ElementoClasificacionId.Equals(Id, StringComparison.InvariantCultureIgnoreCase));
            if (El.Count() > 0)
            {
               
            }

            return r;


        }
        public async Task<bool> Hijos(string ElementoClasificacionId, string idPadre) 
        {
            List<ElementoClasificacion> ListHijos = await LitsElement(x => x.ElementoClasificacionId.Equals(ElementoClasificacionId, StringComparison.InvariantCultureIgnoreCase));

            if (ListHijos.Count > 0)
            {
                foreach (var item in ListHijos)
                {
                    if (item.Eliminada)
                        return false;
                    if (item.ElementoClasificacionId==idPadre) { return false; }
                    if (!String.IsNullOrEmpty(item.ElementoClasificacionId) && item.Eliminada != true)
                    {
                        ElementoClasificacion e = await this.repo.UnicoAsync(x => x.Id == ElementoClasificacionId);
                        if (!String.IsNullOrEmpty(e.ElementoClasificacionId)) {

                            if (e.Id == ElementoClasificacionId)
                            {
                                return false ;
                            }
                            else
                                return true;
                        }
                        else
                        {
                            await Hijos(e.ElementoClasificacionId, e.Id);
                            return true;
                        }
                    }
                    else
                        return true;
                }
                return false;
            }
            else
            {
                ElementoClasificacion e =  await this.repo.UnicoAsync(x => x.Id == ElementoClasificacionId);
                if (e.Id == idPadre)
                    return false;
                if (e.Eliminada)
                    return false;
                if (!String.IsNullOrEmpty(e.ElementoClasificacionId))
                    return true;
                else
                    await Hijos(e.ElementoClasificacionId,e.Id);
               
                return false;

            }

        }
        public async Task ActualizarAsync(ElementoClasificacion entity)
        {
            ElementoClasificacion o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }
            if (await Existe(x =>
            x.Id != entity.Id && x.Clave.Equals(entity.Clave.Trim(),
            StringComparison.InvariantCultureIgnoreCase) && x.Eliminada!=true
             && x.CuadroClasifiacionId == entity.CuadroClasifiacionId))
            {
                throw new ExElementoExistente(entity.Clave);
            }

            
            if(!String.IsNullOrEmpty(entity.ElementoClasificacionId))
             if (entity.Id == entity.ElementoClasificacionId) { throw new ExElementoExistente(entity.Id); }

            if (!String.IsNullOrEmpty(entity.CuadroClasifiacionId))
                if (await ValidaCuadroClasificacion(entity.CuadroClasifiacionId))
            {
                throw new ExElementoExistente(entity.CuadroClasifiacionId);
            }
           
            if (!String.IsNullOrEmpty(entity.ElementoClasificacionId))
                if (await ValidaElementoCuadroClasificacion(entity.ElementoClasificacionId))
                    throw new ExElementoExistente(entity.ElementoClasificacionId);
            if(entity.Posicion<0)
                    throw new ExElementoExistente(entity.Posicion.ToString());



            o.Nombre = entity.Nombre.Trim();
            o.Eliminada = entity.Eliminada;
            o.Posicion = entity.Posicion;
            o.Clave = entity.Clave.Trim();
            if(!String.IsNullOrEmpty(entity.ElementoClasificacionId))
                o.ElementoClasificacionId = entity.ElementoClasificacionId.Trim();

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(entity.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);

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
        public async Task<IPaginado<ElementoClasificacion>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            int x = 0;
            ElementoClasificacion e;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                e = await this.repo.UnicoAsync(x => x.Id == Id.Trim()).ConfigureAwait(false);
                if (e != null)
                {
                    x++;

                    e.Eliminada = true;
                    UDT.Context.Entry(e).State = EntityState.Modified;
                    listaEliminados.Add(e.Id);
                    await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(e.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta).ConfigureAwait(false);

                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            ElementoClasificacion c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    
                    c.Nombre = await RestaurarNombre(c.Clave,c.CuadroClasifiacionId,c.Id,c.Nombre);
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                    await ioCuadroClasificacion.EliminarCuadroCalsificacionExcel(c.CuadroClasifiacionId, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);

                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public async Task<List<ElementoClasificacion>> ObtenerHijosAsync(string PadreId, string JerquiaId)
        {
            var  l= await this.repo.ObtenerAsync( x=> x.CuadroClasifiacionId == JerquiaId 
            && x.ElementoClasificacionId == PadreId && x.Eliminada == false);

            return l.ToList().OrderBy(x => x.NombreJerarquico).ToList();
        }
        public async Task<List<ElementoClasificacion>> ObtenerRaicesAsync(string JerquiaId)
        {
 
         var l = await this.repo.ObtenerAsync(x => x.CuadroClasifiacionId == JerquiaId
         && x.EsRaiz == true && x.Eliminada == false);
         
          return l.ToList().OrderBy(x=>x.NombreJerarquico).ToList();
         
        }
        private async Task<string> RestaurarNombre(string Clave, string CuadroClasificacionId,string id,string Nombre)
        {
           
                if (await Existe(x =>
            x.Id != id && x.Clave.Equals(Clave,
            StringComparison.InvariantCultureIgnoreCase) && x.Eliminada != true
             && x.CuadroClasifiacionId == CuadroClasificacionId)) 
            {
                    Nombre = Nombre + " restaurado " + DateTime.Now.Ticks;
             }

            return Nombre;

        }
        public async Task<ElementoClasificacion> UnicoAsync(Expression<Func<ElementoClasificacion, bool>> predicado = null, Func<IQueryable<ElementoClasificacion>, IOrderedQueryable<ElementoClasificacion>> ordenarPor = null, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ElementoClasificacion c = await this.repo.UnicoAsync(predicado);
            return c.Copia();
        }
        public Task<List<ElementoClasificacion>> ObtenerAsync(Expression<Func<ElementoClasificacion, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ElementoClasificacion>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }


       public async Task<string[]> Purgar()
        {

            List<ElementoClasificacion> elemento = await this.repo.ObtenerAsync(x=>x.Eliminada==true).ConfigureAwait(false);

            if (elemento.Count>0) 
            {

                ServicioEntradaClasificacion servicioEntrada = new ServicioEntradaClasificacion(this.proveedorOpciones,LoggerentradaCuadro,Config);

                List<EntradaClasificacion> ListaEntradas = await servicioEntrada.ObtenerAsync(x=>x.ElementoClasificacionId.Contains(elemento.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
                List<ElementoClasificacion> ListaHijos = await this.repo.ObtenerAsync(x=>x.ElementoClasificacionId.Contains(elemento.Select(x=>x.ElementoClasificacionId).FirstOrDefault())).ConfigureAwait(false);
              
                await servicioEntrada.Eliminar(IdsELiminar(ListaEntradas.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
                await servicioEntrada.Purgar().ConfigureAwait(false);
                await this.EliminarElementos(IdsELiminar(ListaHijos.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
                if(await Existe(x=>x.Eliminada==true).ConfigureAwait(false))
                await this.EliminarElementos(IdsELiminar(elemento.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            }

            return IdsELiminar(elemento.Select(x=>x.Id).ToArray());
        }
        private async Task<ICollection<string>> EliminarElementos(string[] ids) 
        {
            ElementoClasificacion e;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                e = await this.repo.UnicoAsync(x => x.Id == Id).ConfigureAwait(false);
                if (e != null)
                {
                    await this.repo.Eliminar(e).ConfigureAwait(false);
                    listaEliminados.Add(e.Id);
                }

            Console.WriteLine($"\n Eliminar elementos {Id}");
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        private string[] IdsELiminar(string[] ids) 
        {
            return ids;
        }

        #region Sin implementar
        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<ElementoClasificacion>> CrearAsync(params ElementoClasificacion[] entities)
        {
            throw new NotImplementedException();
        }
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
    
        public Task<IPaginado<ElementoClasificacion>> ObtenerPaginadoAsync(Expression<Func<ElementoClasificacion, bool>> predicate = null, Func<IQueryable<ElementoClasificacion>, IOrderedQueryable<ElementoClasificacion>> orderBy = null, Func<IQueryable<ElementoClasificacion>, IIncludableQueryable<ElementoClasificacion, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<ElementoClasificacion>> CrearAsync(IEnumerable<ElementoClasificacion> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
