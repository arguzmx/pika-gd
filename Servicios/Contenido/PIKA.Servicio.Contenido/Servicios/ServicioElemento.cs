﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Helpers;
using PIKA.Servicio.Contenido.Interfaces;
using PIKA.Servicio.Contenido.Servicios.Busqueda;
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
        private IRepositorioAsync<Permiso> repoPerm;
        private IRepositorioAsync<Carpeta> repoCarpetas;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private ComunesCarpetas helperCarpetas;
        private IAppCache appcache;

        public UsuarioAPI Usuario { get; set; }

        public ServicioElemento(
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.appcache = cache;
            try
            {
                this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
                this.repo = UDT.ObtenerRepositoryAsync<Elemento>(new QueryComposer<Elemento>(cache));
                this.repoVol = UDT.ObtenerRepositoryAsync<Volumen>(new QueryComposer<Volumen>());
                this.repoVPM = UDT.ObtenerRepositoryAsync<VolumenPuntoMontaje>(new QueryComposer<VolumenPuntoMontaje>());
                this.repoPerm = UDT.ObtenerRepositoryAsync<Permiso>(new QueryComposer<Permiso>());
                this.repoPM = UDT.ObtenerRepositoryAsync<PuntoMontaje>(new QueryComposer<PuntoMontaje>());
                this.repoCarpetas = UDT.ObtenerRepositoryAsync<Carpeta>(new QueryComposer<Carpeta>());
                this.helperCarpetas = new ComunesCarpetas(this.repoCarpetas);
            }
            catch (Exception ex)
            {

                
                throw ex;
            }
            
        }

        public async Task ActualizaTamanoBytes(string Id, long Tamano)
        {

            string sqls = $"UPDATE {DbContextContenido.TablaElemento} set TamanoBytes={Tamano} where Id='{Id}'";
            await UDT.Context.Database.ExecuteSqlCommandAsync(sqls);
        }

        public async Task ActualizaConteoPartes(string Id, int Conteo)
        {
            string sqls = $"UPDATE {DbContextContenido.TablaElemento} set ConteoAnexos={Conteo} where Id='{Id}'";
            await UDT.Context.Database.ExecuteSqlCommandAsync(sqls);
        }

        public async Task ActualizaVersion(string Id, string VersionId) {

            string sqls = $"UPDATE {DbContextContenido.TablaElemento} set VersionId='{VersionId}' where Id='{Id}'";
            await UDT.Context.Database.ExecuteSqlCommandAsync(sqls);
        }

        public async Task<int> ACLPuntoMontaje(string PuntoMontajeId)
        {
            if (Usuario == null) return 0;
            int mascara = int.MaxValue;

            var pm = UDT.Context.PuntosMontaje.Where(x => x.Id == PuntoMontajeId).SingleOrDefault();
            if (pm != null)
            {
                foreach (var a in Usuario.Accesos) {
                    if (a.Admin && pm.OrigenId == a.OU)
                    {
                        return mascara - MascaraPermisos.PDenegarAcceso;
                    }
                }
            }

            foreach(string r in Usuario.Roles)
            {
                var permiso = await UDT.Context.PermisosPuntoMontaje.Where(x => x.DestinatarioId == r && x.PuntoMontajeId == PuntoMontajeId).SingleOrDefaultAsync();
                if(permiso!=null)
                {
                    MascaraPermisos m = new MascaraPermisos()
                    {
                        Admin = false,
                        Ejecutar = false,
                        Eliminar = permiso.Elminar,
                        Escribir = permiso.Actualizar,
                        Leer = permiso.Leer
                    };

                    // asigna los permisos mínimos de acuerdo a los roles
                    mascara &= m.ObtenerMascara();
                }
            }

            return mascara;
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

            if(!esActualizacion)
            {
                var pm = await repoPM.UnicoAsync(x => x.Id == entity.PuntoMontajeId);
                if (pm == null)
                {
                    throw new ExDatosNoValidos($"Punto de montaje no válido");
                }

                var vol = await repoVol.UnicoAsync(x => x.Id == pm.VolumenDefaultId);
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
            }
           

            if (await Existe(x => x.Nombre == entity.Nombre
                  && x.PuntoMontajeId == entity.PuntoMontajeId
                 && x.CarpetaId == entity.CarpetaId && x.Eliminada == false && x.Id != entity.Id))
            {
                if(entity.AutoNombrar.HasValue && entity.AutoNombrar == true)
                {
                    int index = 1;
                    entity.Nombre = $"{entity.Nombre}-{index}";
                    while(
                        await Existe(x => x.Nombre == entity.Nombre
                            && x.PuntoMontajeId == entity.PuntoMontajeId
                            && x.CarpetaId == entity.CarpetaId && x.Eliminada == false && x.Id != entity.Id)
                        ){
                        index++;
                        entity.Nombre = $"{entity.Nombre}-{index}";
                    }

                } else
                {
                    throw new ExElementoExistente(entity.Nombre);
                }
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
                    //var lista = await this.repoVPM.ObtenerAsync(x => x.PuntoMontajeId == carpeta.PuntoMontajeId
                    //&& x.VolumenId == entity.VolumenId);
                    //if(lista.Count == 0)
                    //{
                    //    throw new ExDatosNoValidos($"Carpeta {entity.CarpetaId} no pertenece al volumen {entity.VolumenId}");
                    //}

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

                var pm = await repoPM.UnicoAsync(x => x.Id == entity.PuntoMontajeId);

                entity.VolumenId = pm.VolumenDefaultId;
                entity.Id = System.Guid.NewGuid().ToString();
                entity.FechaCreacion = DateTime.Now;
                entity.Eliminada = false;
                entity.Versionado = true;
                entity.VersionId = entity.Id;

                await this.repo.CrearAsync(entity);


                UDT.SaveChanges();

                return entity.Copia();
            }
            catch (DbUpdateException)
            {
                throw new ExErrorRelacional("El identificador de relació no es válido");
            }
            catch (Exception ex)
            {
                
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

        public async Task<List<Elemento>> ObtenerPaginadoByIdsAsync(ConsultaAPI q)
        {

            if (!string.IsNullOrEmpty(q.IdCache))
            {
                q.ord_columna = "";
                q.ord_direccion = "";
                var consulta = GetDefaultQuery(q.AConulta());
                consulta.Filtros.Add(new FiltroConsulta()
                {
                    Negacion = false,
                    NivelFuzzy = -1,
                    Operador = FiltroConsulta.OP_INLIST,
                    Propiedad = "Id",
                    Valor = q.IdCache,
                    ValorString = q.IdCache
                });


                    return (await this.repo.ObtenerPaginadoAsync(consulta)).Elementos.ToList();

            } else
            {
                var list = await this.UDT.Context.Elemento.Where(x => q.Ids.Contains(x.Id)).ToListAsync();
                if (q.tamano>0 && (list.Count() < q.tamano))
                {

                   
                    int desde = q.indice * q.tamano;
                    q.tamano = q.tamano - list.Count();
                    var respuesta = await this.repo.ObtenerPaginadoDesdeAsync(GetDefaultQuery(q.AConulta()), desde);
                    q.tamano = q.tamano + list.Count();

                    foreach (var e in respuesta.Elementos)
                    {
                        if (!list.Any(x => x.Id == e.Id))
                        {
                            list.Add(e);
                            if (list.Count() == q.tamano) break;
                        }
                    }
                }
                return list;
            }
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
        public async Task<ICollection<string>> Eliminar(List<Elemento> ListaElemento)
        {
            Elemento o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var elemento in ListaElemento)
            {
                o = await this.repo.UnicoAsync(x => x.Id == elemento.Id.Trim());
                if (o != null)
                {
                    o.Eliminada = true;
                    UDT.Context.Entry(o).State = EntityState.Deleted;
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
       
        public async Task<List<string>> Purgar()
        {
            List<Elemento> ListaElementos = await this.ObtenerAsync(x=>x.Eliminada==true);
            if (ListaElementos.Count > 0)
            {
                //List<Carpeta> ListaCarpeta = await this.repoCarpetas.ObtenerAsync(x => x.Id.Contains(ListaElementos.Select(x => x.CarpetaId).FirstOrDefault()));
                //List<Volumen> ListaVolumen = await this.repoVol.ObtenerAsync(x => x.Id.Contains(ListaElementos.Select(x => x.VolumenId).FirstOrDefault()));
                //List<PuntoMontaje> ListaPuntaje = await this.repoPM.ObtenerAsync(x => x.Id.Contains(ListaElementos.Select(x => x.PuntoMontajeId).FirstOrDefault()));
                //List<VolumenPuntoMontaje> ListaVolumenPuntoMontaje = await this.repoVPM.ObtenerAsync(x => x.VolumenId.Contains(ListaVolumen.Select(x => x.Id).FirstOrDefault()));
                //ServicioParte sp = new ServicioParte(this.proveedorOpciones, this.logger);
                //List<Parte> ListaPartes = await sp.ObtenerAsync(x => x.ElementoId.Contains(ListaElementos.Select(x => x.Id).FirstOrDefault()));
                //string[] PartesEliminadas = ListaPartes.Select(x => x.Id).ToArray();
                //ServicioPuntoMontaje spm = new ServicioPuntoMontaje(this.proveedorOpciones,this.logger);
                //ServicioVolumenPuntoMontaje vem = new ServicioVolumenPuntoMontaje(this.proveedorOpciones,this.logger);
                //string[] Idvol = ListaVolumen.Select(x=>x.Id).ToArray();
                //foreach (PuntoMontaje puntaje in ListaPuntaje)
                //{
                //    PuntoMontaje p = await repoPM.UnicoAsync(x=>x.Id.Equals(puntaje.Id,StringComparison.InvariantCultureIgnoreCase));
                //    p.Eliminada = true;
                //    await spm.ActualizarAsync(p);
                //    await vem.Eliminar(puntaje.Id,Idvol);

                //}
                //Console.WriteLine($"Ingreso al metodos de la funcion Purgar {Idvol.Count()}");
            }
            //await repoVol.EliminarRango(ListaVolumen);
            //await repoVer.EliminarRango(ListaVersion);
            //await repoPM.EliminarRango(ListaPuntaje);
            //await this.Eliminar(ListaElementos);


            return ListaElementos.Select(x=>x.Id).ToList();
        }


        


        #endregion
    }

}