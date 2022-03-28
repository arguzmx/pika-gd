using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestrctura.Reportes;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.Reportes.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioArchivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Archivo> repo;
        private IRepositorioAsync<TipoArchivo> repoTA;
        private IOptions<ConfiguracionServidor> Config;
        private IServicioReporteEntidad ServicioReporteEntidad;

        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public ServicioArchivo(
            IServicioReporteEntidad ServicioReporteEntidad,
            IOptions<ConfiguracionServidor> Config,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(proveedorOpciones, Logger)
        {
            this.ServicioReporteEntidad = ServicioReporteEntidad;
            this.Config = Config;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoTA = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
        }
        public async Task<bool> Existe(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        private async Task<bool> ExisteTipoArchivos(Expression<Func<TipoArchivo, bool>> predicado) 
        {
            List<TipoArchivo> l = await this.repoTA.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<Archivo> CrearAsync(Archivo entity, CancellationToken cancellationToken = default)
        {
            if (!await ExisteTipoArchivos(x => x.Id == entity.TipoArchivoId.Trim()))
                throw new ExDatosNoValidos(entity.TipoArchivoId);

            if (await Existe(x=>x.Nombre.Equals(entity.Nombre,StringComparison.InvariantCultureIgnoreCase)
            && x.Id!=entity.Id && x.Eliminada!=true && x.TipoArchivoId.Equals(entity.TipoArchivoId,StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(Archivo entity)
        {
            Archivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (!await ExisteTipoArchivos(x => x.Id == entity.TipoArchivoId.Trim()))
                throw new ExDatosNoValidos(entity.TipoArchivoId);
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Id != entity.Id && x.Eliminada != true && x.TipoArchivoId.Equals(entity.TipoArchivoId, StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.PuntoMontajeId = entity.PuntoMontajeId;
            o.VolumenDefaultId = entity.VolumenDefaultId;
            o.Nombre = entity.Nombre.Trim();
            o.Eliminada = entity.Eliminada;
            o.TipoArchivoId = entity.TipoArchivoId.Trim();

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
        public async Task<IPaginado<Archivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Archivo c;
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
        public Task<List<Archivo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<Archivo>> ObtenerAsync(Expression<Func<Archivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<Archivo> UnicoAsync(Expression<Func<Archivo, bool>> predicado = null, Func<IQueryable<Archivo>, IOrderedQueryable<Archivo>> ordenarPor = null, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Archivo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        private async Task<string> RestaurarNombre(string nombre, string TipoArchivoId, string id)
        {

            if (await Existe(x =>
        x.Id != id && x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)
        && x.Eliminada == false
         && x.TipoArchivoId == TipoArchivoId))
            {

                nombre = nombre + " restaurado " + DateTime.Now.Ticks;
            }
            else
            {
            }


            return nombre;

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            Archivo c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    c.Nombre = await RestaurarNombre(c.Nombre, c.TipoArchivoId, c.Id);
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    listaEliminados.Add(c.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                }
            }
            
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = true,
                    Operador = "eq",
                    Valor = "true"
                });
            }
          
            Query = GetDefaultQuery(Query);
            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            logger.LogInformation($"{l.Count}");
            

            return l.OrderBy(x => x.Texto).ToList();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id.Trim()));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public async Task<ICollection<string>> Purgar()
        {
            await Task.Delay(1);

            return null;
            //ServicioHistorialArchivoActivo shaa = new ServicioHistorialArchivoActivo(this.proveedorOpciones,this.logger);
            //ServicioActivo sa = new ServicioActivo(cache,this.proveedorOpciones, this.logger, Config);
            //ServicioPrestamo sp = new ServicioPrestamo(this.proveedorOpciones, this.logger);
            //ServicioAlmacenArchivo saa = new ServicioAlmacenArchivo(this.proveedorOpciones, this.logger);
            //ServicioTransferencia st = new ServicioTransferencia(this.proveedorOpciones, this.logger, Config);
            //ServicioEstadisticaClasificacionAcervo servicioEstadistica = new ServicioEstadisticaClasificacionAcervo(this.proveedorOpciones,Config,logger);
            //List<Archivo> ListaArchivos = await this.repo.ObtenerAsync(x=>x.Eliminada==true).ConfigureAwait(false);
            //string[] IdArchivosEliminar = ListaArchivos.Select(x=>x.Id).ToArray();
            //    if (ListaArchivos.Count > 0) 
            //{
            //    List<HistorialArchivoActivo> listaHitorialArchivos =await  shaa.ObtenerAsync(x=>x.ArchivoId.Contains(ListaArchivos.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
            //    List<Activo> ListaActivos = await sa.ObtenerAsync(x=>x.ArchivoId.Contains(ListaArchivos.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
            //    List<Prestamo> ListaPrestamo = await sp.ObtenerAsync(x=>x.ArchivoId.Contains(ListaArchivos.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
            //    List<AlmacenArchivo> ListaAlmacen = await saa.ObtenerAsync(x=>x.ArchivoId.Contains(ListaArchivos.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);

            //    await shaa.Eliminar(ListaIdEliminar(listaHitorialArchivos.Select(x => x.Id).ToArray())).ConfigureAwait(false);
            //    await sa.Eliminar(ListaIdEliminar(ListaActivos.Select(x => x.Id).ToArray())).ConfigureAwait(false);
            //    await sa.Purgar().ConfigureAwait(false);
            //    //await sa.EliminarActivos(ListaIdEliminar(ListaActivos.Select(x => x.Id).ToArray())).ConfigureAwait(false);
            //    await sp.Eliminar(ListaIdEliminar(ListaPrestamo.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            //    await sp.Purgar().ConfigureAwait(false);
            //    await sp.EliminarPrestamo(ListaIdEliminar(ListaPrestamo.Select(x => x.Id).ToArray())).ConfigureAwait(false);
            //    await saa.EliminarRelaciones(ListaAlmacen);
            //    await saa.Eliminar(ListaIdEliminar(ListaAlmacen.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            //    await st.Eliminar(await st.EliminarRelaciones(ListaArchivos).ConfigureAwait(false));
            //    await servicioEstadistica.EliminarEstadisticos(1,ListaArchivos.Select(x=>x.Id).ToArray()).ConfigureAwait(false);
            //}

            //return  await EliminarArchivo(ListaIdEliminar(ListaArchivos.Select(x => x.Id).ToArray())).ConfigureAwait(false);
        }
        private string[] ListaIdEliminar(string[]ids) 
        {
            return ids;
        }
        private async Task<ICollection<string>> EliminarArchivo(string[] ids)
        {
            Archivo o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                try
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id);
                    if (o != null)
                    {
                        await this.repo.Eliminar(o);
                        listaEliminados.Add(o.Id);
                    }
                    this.UDT.SaveChanges();

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
            UDT.SaveChanges();

            return listaEliminados;

        }


        /// <summary>
        /// Crea el reporte de guía simple de archivo
        /// </summary>
        /// <param name="ArchivoId">Identificador del archivo para el reporte</param>
        /// <returns></returns>
        public async Task<byte[]> ReporteGuiaSimpleArchivo(string ArchivoId, List<ElementoReporte> reportes)
        {
            GuiaSimpleArchivo g = new GuiaSimpleArchivo
            {
                Archivo = this.UDT.Context.Archivos.Find(ArchivoId)
            };

            if (g.Archivo == null) return null;

            List<EstadisticaClasificacionAcervo> estadistica = await this.UDT.Context.EstadisticaClasificacionAcervo.Where(x => x.ArchivoId == ArchivoId && x.ConteoActivos > 0).ToListAsync();
            List<string> cuadros = estadistica.Select(x => x.CuadroClasificacionId).Distinct().ToList();
            List<string> unidades = estadistica.Select(x => x.UnidadAdministrativaArchivoId).Distinct().ToList();
            List<ElementoClasificacion> elementos = OntieneElementosCuadros(cuadros);
            List<EntradaClasificacion> entradas = OntieneEntradasActivas(ArchivoId);
            List<UnidadAdministrativaArchivo> uas = UDT.Context.UnidadesAdministrativasArchivo.Where(x => unidades.Contains(x.Id)).ToList();
            List<UnidadAdministrativaGuiaSimpleArchivo> uasguia = new List<UnidadAdministrativaGuiaSimpleArchivo>();

            estadistica.GroupBy(x => x.UnidadAdministrativaArchivoId).Select(c => new UnidadAdministrativaGuiaSimpleArchivo
            {
                Id = c.Key,
                Expedientes = c.Sum(x => x.ConteoActivos)
            }).ToList().ForEach(u =>
            {
                var ua = uas.Where(x => x.Id == u.Id).FirstOrDefault();
                if (ua != null)
                {
                    uasguia.Add(ua.AUnidadGuiaSimple(u.Expedientes));
                }
            });
            g.UnidadesAdministrativas = uasguia;



            for (int i = 0; i < g.UnidadesAdministrativas.Count(); i++)
            {
                string uaId = g.UnidadesAdministrativas[i].Id;
                foreach (var raiz in elementos.Where(x => x.EsRaiz == true).ToList())
                {
       
                    SeccionGuiaSimpleArchivo s = new SeccionGuiaSimpleArchivo() {
                        Nombre = raiz.Nombre,
                        Elementos = new List<ElementoGuiaSimpleArchivo>()
                    };

                    var series = entradas.Where(c => c.ElementoClasificacionId == raiz.Id).ToList().Select(x => x.Id).ToList();

                    // Obtiene las series
                    foreach (var e in estadistica.Where(x => series.Contains(x.EntradaClasificacionId) 
                    && x.UnidadAdministrativaArchivoId == uaId).ToList())
                    {
                        var entrada = entradas.Where(x => x.Id == e.EntradaClasificacionId).First();
                        s.Elementos.Add(new ElementoGuiaSimpleArchivo()
                        {
                            Cantidad = e.ConteoActivos,
                            Descripcion = entrada.Descripcion,
                            FechasLimites = $"{(e.FechaMinApertura.HasValue ? e.FechaMinApertura.Value.ToString("yyyy") : "")}-{(e.FechaMaxCierre.HasValue ? e.FechaMaxCierre.Value.ToString("yyyy") : "")}".TrimEnd('-'),
                            Serie = entrada.Clave,
                            Subserie = ""
                        }); ;
                    };

                    // Obtiene las sub-series
                    var subseriesIds = ObtieneIdSubseries(raiz.Id, elementos, entradas, true);
    
                    var encontrados = estadistica.Where(x => subseriesIds.Contains(x.EntradaClasificacionId)
                    && x.UnidadAdministrativaArchivoId == uaId).ToList();

                    foreach (var e in encontrados)
                    {
                        var entrada = entradas.Where(x => x.Id == e.EntradaClasificacionId).First();
                        var el = elementos.Where (x=>x.Id == entrada.ElementoClasificacionId).FirstOrDefault();
                        var elc = new ElementoGuiaSimpleArchivo()
                            {
                                Cantidad = e.ConteoActivos,
                                Descripcion = entrada.Descripcion,
                                FechasLimites = $"{(e.FechaMinApertura.HasValue ? e.FechaMinApertura.Value.ToString("yyyy") : "")}-{(e.FechaMaxCierre.HasValue ? e.FechaMaxCierre.Value.ToString("yyyy") : "")}",
                                Serie = el.Clave,
                                Subserie = entrada.Clave
                            };
                        s.Elementos.Add(elc); 
                    };

                    if (s.Elementos.Count > 0)
                    {
                        g.UnidadesAdministrativas[i].Secciones.Add(s);
                    }
                };
            }
   
            string jsonString = JsonSerializer.Serialize(g);
            return ReporteEntidades.ReportePlantilla( reportes, jsonString, Config.Value.ruta_cache_fisico, false, null, true);

        }

        private List<string> ObtieneIdSubseries(string padreId, List<ElementoClasificacion> elementos, List<EntradaClasificacion> entradas, bool esRaiz)
        {
            List<string> l = new List<string>();

            var hijos = elementos.Where(x => x.ElementoClasificacionId == padreId).ToList();
            foreach (var el in hijos)
            {
                l.AddRange(ObtieneIdSubseries(el.Id, elementos, entradas, false));
            }

             if(!esRaiz)
            {
                var es = entradas.Where(x => x.ElementoClasificacionId == padreId).ToList();
                foreach (var el in es)
                {
                    l.Add(el.Id);
                }
            }
           

            return l;
        }

        private List<EntradaClasificacion> OntieneEntradasActivas(string ArchivoId)
        {
            string sqls = $@"SELECT * FROM {DBContextGestionDocumental.TablaEntradaClasificacion} where Id in (
                            SELECT EntradaClasificacionId FROM {DBContextGestionDocumental.TablaEstadisticaClasificacionAcervo} where ArchivoId = '{ArchivoId}' and ConteoActivos>0 );";

            return this.UDT.Context.EntradaClasificacion.FromSqlRaw(sqls).ToList();
        }

        private List<ElementoClasificacion> OntieneElementosCuadros(List<string> cuadros)
        {
            List<ElementoClasificacion> l = new List<ElementoClasificacion>();
            List<Task<List<ElementoClasificacion>>> telementos = new List<Task<List<ElementoClasificacion>>>();
            foreach (var c in cuadros)
            {
                telementos.Add(this.UDT.Context.ElementosClasificacion.Where(x => x.CuadroClasifiacionId == c).ToListAsync());
            }
            Task.WaitAll(telementos.ToArray());

            foreach(var t in telementos)
            {
                if(t.Result.Count() > 0)
                {
                    l.AddRange(t.Result);
                }
            }

            return l;
        }


        /// <summary>
        /// Crea el reporte de inventario
        /// </summary>
        /// <param name="ArchivoId">Identificador del archivo para el reporte</param>
        /// <returns></returns>
        public async Task<string> ReporteGuiaInventario(string ArchivoId)
        {

            string nombreArchivo = $"activos{Guid.NewGuid().ToString().Replace("-","")}.csv";
            string ruta = Path.Combine(Config.Value.ruta_cache_fisico, nombreArchivo);
            string q = $"select *  from {DBContextGestionDocumental.TablaActivos} where Eliminada=0 and ArchivoId='{ArchivoId}'";
            List<Activo> activos = await this.UDT.Context.Activos.FromSqlRaw(q).ToListAsync();
            List<EntradaClasificacion> entradas = UDT.Context.EntradaClasificacion.ToList();

            FileStream fs = new FileStream(ruta, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

            sw.WriteLine("Nombre\tIdentificador\tClave\tFecha Apertura\tFecha Cierre\tRetención AT\tRetención AC\tEn Préstamo\tReservado\tConfidencial\tAmpliado\tCódigo Barras\tCódigo RFID\tAsunto");

            foreach (var a in activos)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(a.Nombre);
                sb.Append("\t");
                sb.Append(string.IsNullOrEmpty(a.IDunico) ? "" : a.IDunico);
                sb.Append("\t");
                sb.Append(entradas.Where(x=>x.Id == a.EntradaClasificacionId).First().NombreCompleto);
                sb.Append("\t");
                sb.Append(a.FechaApertura.ToString("dd/MM/yyyy"));
                sb.Append("\t");
                sb.Append(a.FechaCierre.HasValue? a.FechaCierre.Value.ToString("dd/MM/yyyy") :"");
                sb.Append("\t");
                sb.Append(a.FechaRetencionAT.HasValue ? a.FechaRetencionAT.Value.ToString("dd/MM/yyyy") : "");
                sb.Append("\t");
                sb.Append(a.FechaRetencionAC.HasValue ? a.FechaRetencionAC.Value.ToString("dd/MM/yyyy") : "");
                sb.Append("\t");
                sb.Append(a.EnPrestamo ? "X" : "");
                sb.Append("\t");
                sb.Append(a.Reservado ? "X" : "");
                sb.Append("\t");
                sb.Append(a.Confidencial ? "X" : "");
                sb.Append("\t");
                sb.Append(a.Ampliado ? "X" : "");
                sb.Append(string.IsNullOrEmpty(a.CodigoOptico) ? "": a.CodigoOptico);
                sb.Append("\t");
                sb.Append(string.IsNullOrEmpty(a.CodigoElectronico) ? "": a.CodigoElectronico);
                sb.Append("\t");
                sb.Append(string.IsNullOrEmpty(a.Asunto) ? "" : a.Asunto);
                sw.WriteLine(sb.ToString());
            }
            sw.Close();
            fs.Close();

            return ruta;
        }


        #region Sin Implementación
        public Task<IPaginado<Archivo>> ObtenerPaginadoAsync(Expression<Func<Archivo, bool>> predicate = null, Func<IQueryable<Archivo>, IOrderedQueryable<Archivo>> orderBy = null, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Archivo>> CrearAsync(params Archivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Archivo>> CrearAsync(IEnumerable<Archivo> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

       
        #endregion

    }
}
