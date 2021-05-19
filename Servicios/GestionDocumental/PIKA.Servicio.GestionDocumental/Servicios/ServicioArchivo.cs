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
            o.TipoArchivoId = o.TipoArchivoId.Trim();

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
        public async Task<byte[]> ReporteGuiaSimpleArchivo(string ArchivoId)
        {

            GuiaSimpleArchivo g = new GuiaSimpleArchivo
            {
                Archivo = await repo.UnicoAsync(x => x.Id == ArchivoId)
            };

            if (g.Archivo == null) throw new EXNoEncontrado(ArchivoId);

            var r = await ServicioReporteEntidad.UnicoAsync(x => x.Id == "guiasimplearchivo");
            if (r == null) throw new EXNoEncontrado("reporte: guiasimplearchivo");

            IRepositorioAsync<EstadisticaClasificacionAcervo> repoestadistica 
                = UDT.ObtenerRepositoryAsync<EstadisticaClasificacionAcervo>(new QueryComposer<EstadisticaClasificacionAcervo>()); ;
            
            IRepositorioAsync<EntradaClasificacion> repoentradas
                = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>()); ;

            //Lista de estadistica
            var estadistica = await repoestadistica.ObtenerAsync(x => x.ArchivoId == ArchivoId);

            // Ontiene las entradas para llenar el reporte
            var cuadros = estadistica.Select(x => x.CuadroClasificacionId).Distinct();
            List<EntradaClasificacion> entradas = new List<EntradaClasificacion>();
            foreach (string id in cuadros)
            {
                entradas.AddRange(
                    await repoentradas.ObtenerAsync(x => x.CuadroClasifiacionId == id 
                    && x.Eliminada == false));
            }

            // Añade las entradas al reporte con la cantidad adecuada
            entradas = entradas.OrderBy(x => x.CuadroClasifiacionId).ThenBy(x => x.Posicion).ToList();
            entradas.ForEach(e =>
            {
                var entrada = estadistica.Where(x => x.EntradaClasificacionId == e.Id).SingleOrDefault();
                int cantidad = entrada == null? 0: entrada.ConteoActivos;
                string fMinima = entrada == null ? "-" : entrada.FechaMinApertura.HasValue ? entrada.FechaMinApertura.Value.ToString("dd/MM/yyyy") : "";
                string fMaxima = entrada == null ? "-" : entrada.FechaMinApertura.HasValue ? entrada.FechaMinApertura.Value.ToString("dd/MM/yyyy") : "";

                g.Elementos.Add(new ElementoGuiaSimpleArchivo()
                {
                    Cantidad = cantidad,
                    Clave = e.Clave,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion ?? "" ,
                    FechaMaximaCierre = fMaxima,
                    FechaMinimaApertura = fMinima
                });
            });

            string jsonString = JsonSerializer.Serialize(g);

            byte[] data = Convert.FromBase64String(r.Plantilla);

            return ReporteEntidades.ReportePlantilla( data , jsonString, Config.Value.ruta_cache_fisico, true);
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
            string q = $"select *  from gd$activo where ArchivoId='{ArchivoId}'";
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
