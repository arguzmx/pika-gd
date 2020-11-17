using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
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
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;
using SixLabors.ImageSharp.ColorSpaces;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioCarpeta : ContextoServicioContenido,
        IServicioInyectable, IServicioCarpeta
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Carpeta> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private ComunesCarpetas helperCarpetas;
        public ServicioCarpeta(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Carpeta>(new QueryComposer<Carpeta>());
            this.helperCarpetas = new ComunesCarpetas(this.repo);
        }

        public async Task<bool> Existe(Expression<Func<Carpeta, bool>> predicado)
        {
            List<Carpeta> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<Carpeta> CrearAsync(Carpeta entity, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrEmpty(entity.CarpetaPadreId))
            {
                if (await Existe(x => x.Nombre == entity.Nombre
                 && x.EsRaiz == entity.EsRaiz
                 && x.PuntoMontajeId == entity.PuntoMontajeId))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }
            else
            {
                if (await Existe(x => x.Nombre == entity.Nombre
                 && x.CarpetaPadreId == entity.CarpetaPadreId
                 && x.PuntoMontajeId == entity.PuntoMontajeId))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }



            try
            {

                entity.Id = System.Guid.NewGuid().ToString();
                entity.FechaCreacion = DateTime.Now;
                entity.Eliminada = false;

                await this.repo.CrearAsync(entity);

                UDT.SaveChanges();

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de la carpeta padre no es válido");
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
           

        }
        public async Task ActualizarAsync(Carpeta entity)
        {
            Carpeta o = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (string.IsNullOrEmpty(entity.CarpetaPadreId))
            {
                if (await Existe(x => x.Nombre == entity.Nombre
                 && x.EsRaiz == entity.EsRaiz
                 && x.PuntoMontajeId == entity.PuntoMontajeId 
                 && x.Id == entity.Id))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }
            else
            {
                if (await Existe(x => x.Nombre == entity.Nombre
                 && x.CarpetaPadreId == entity.CarpetaPadreId
                 && x.PuntoMontajeId == entity.PuntoMontajeId
                 && x.Id == entity.Id))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
            }

            try
            {

                // No debe permitir mover a una carpeta hija
                bool eshija = await this.helperCarpetas.EsCarpetaHija(entity.Id, entity.CarpetaPadreId);
                if(eshija )
                {
                    throw new ExDatosNoValidos("La carpeta padre es hija de la actual");
                }

                // La carpeta destino o algno de sus padres no debe estar marcada como eliminada
                if (await this.helperCarpetas.EsCarpetaEliminada(entity.CarpetaPadreId))
                {
                    throw new ExDatosNoValidos("La carpeta seleciconada está eliminada");
                }

                o.Nombre = entity.Nombre;
                o.CarpetaPadreId = entity.CarpetaPadreId;
                o.PermisoId = entity.PermisoId;
                o.EsRaiz = (string.IsNullOrEmpty(entity.CarpetaPadreId));

                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de la carpeta padre no es válido");
            }
            catch (Exception ex)
            {
                
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
        public async Task<IPaginado<Carpeta>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Carpeta>, IIncludableQueryable<Carpeta, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Carpeta o;
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
            Carpeta o;
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
        public async Task<Carpeta> UnicoAsync(Expression<Func<Carpeta, bool>> predicado = null, Func<IQueryable<Carpeta>, IOrderedQueryable<Carpeta>> ordenarPor = null, Func<IQueryable<Carpeta>, IIncludableQueryable<Carpeta, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Carpeta d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);


            return d.Copia();
        }
        public async Task<List<NodoJerarquico>> ObtenerRaices(string IdJerarquia, int N)
        {
            int nivel = 0;
            List<NodoJerarquico> lista = new List<NodoJerarquico>();
            lista = await this.ObtenerRaices(IdJerarquia, "", N, nivel);
            return lista;
        }
        private async Task<List<NodoJerarquico>> ObtenerRaices(string IdJerarquia,
        string Id, int N, int Actual)
        {

            List<Carpeta> resultados;
            if (string.IsNullOrEmpty(Id))
            {
               resultados = await this.repo.ObtenerAsync(x => x.PuntoMontajeId == IdJerarquia
               && x.EsRaiz == true, y => y.OrderBy(y => y.Nombre));
            } else
            {
                resultados = await this.repo.ObtenerAsync(x => x.PuntoMontajeId == IdJerarquia
                && x.CarpetaPadreId == Id, y => y.OrderBy(y => y.Nombre));
            }
         

            if (resultados.Count == 0)
            {
                return null;
            }

            List<NodoJerarquico> l = resultados.Select(x => new NodoJerarquico()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre,
                Hijos = null
            }).ToList();


            if (N > Actual)
            {
                return l;
            }
            else
            {
                Actual++;
                if(Actual > N)
                {
                    return l;
                } else
                {
                    foreach (NodoJerarquico n in l)
                    {
                        n.Hijos = await ObtenerRaices(IdJerarquia, n.Id, N, Actual);
                    }
                    return l;
                }
                
            }
        }
        public async  Task<List<NodoJerarquico>> ObtenerDescendientes(string IdJerarquia, string Id, int N)
        {
            int nivel = 0;
            List<NodoJerarquico> lista = new List<NodoJerarquico>();
            lista = await this.ObtenerDescendientes(IdJerarquia, Id, N, nivel);
            return lista;
        }
        private async Task<List<NodoJerarquico>> ObtenerDescendientes(string IdJerarquia, 
            string Id, int N, int Actual)
        {
            var resultados = await this.repo.ObtenerAsync(x => x.PuntoMontajeId == IdJerarquia
                && x.CarpetaPadreId == Id, y => y.OrderBy(y => y.Nombre));
            
            if(resultados.Count == 0)
            {
                return null;
            }
            
            List<NodoJerarquico> l = resultados.Select(x => new NodoJerarquico()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre,
                Hijos = null
            }).ToList();


            if (N > Actual)
            {
                return l;
            } else
            {
                Actual++;
                if (Actual > N)
                {
                    return l;
                }
                else
                {
                    foreach (NodoJerarquico n in l)
                    {
                        n.Hijos = await ObtenerRaices(IdJerarquia, n.Id, N, Actual);
                    }
                    return l;
                }
            }

        }


        public async Task<List<Carpeta>> ObtenerHijosAsync(string PadreId, string JerquiaId)
        {
            var l = await this.repo.ObtenerAsync(x => x.PuntoMontajeId == JerquiaId
           && x.CarpetaPadreId == PadreId && x.Eliminada == false);

            return l.ToList().OrderBy(x => x.NombreJerarquico).ToList();
        }
        public async Task<List<Carpeta>> ObtenerRaicesAsync(string JerquiaId)
        {

            var l = await this.repo.ObtenerAsync(x => x.PuntoMontajeId == JerquiaId
            && x.EsRaiz == true && x.Eliminada == false);
            return l.ToList().OrderBy(x => x.NombreJerarquico).ToList();

        }
        #region No Implemenatdaos

        public Task<IEnumerable<Carpeta>> CrearAsync(params Carpeta[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Carpeta>> CrearAsync(IEnumerable<Carpeta> entities, CancellationToken cancellationToken = default)
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

        public Task<List<Carpeta>> ObtenerAsync(Expression<Func<Carpeta, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Carpeta>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Carpeta>> ObtenerPaginadoAsync(Expression<Func<Carpeta, bool>> predicate = null, Func<IQueryable<Carpeta>, IOrderedQueryable<Carpeta>> orderBy = null, Func<IQueryable<Carpeta>, IIncludableQueryable<Carpeta, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> Purgar()
        {
            //ServicioElemento se = new ServicioElemento(proveedorOpciones,this.logger);
            //ServicioPuntoMontaje spm = new ServicioPuntoMontaje(proveedorOpciones,this.logger);
            //List<Carpeta> ListaCarpeta = await this.ObtenerAsync(x=>x.Eliminada==true).ConfigureAwait(false);
            //List<Elemento>ListaElemento= await se.ObtenerAsync(x => x.CarpetaId.Contains(ListaCarpeta.Select(x => x.Id).FirstOrDefault()));
            //string[] ElementosEliminados = ListaElemento.Select(x=>x.Id).ToArray();
            //string[] PuntajeEliminado = ListaCarpeta.Select(x=>x.PuntoMontajeId).ToArray();
            //se = new ServicioElemento(proveedorOpciones, this.logger);
            //await spm.Eliminar(PuntajeEliminado);
            //await se.Eliminar(ElementosEliminados);
            //await  se.Purgar();
            //await  spm.Purgar();

            //return ListaCarpeta.Select(x => x.Id).ToList();

            return await Task.FromResult(new List<string>());
        }


        #endregion
    }

}