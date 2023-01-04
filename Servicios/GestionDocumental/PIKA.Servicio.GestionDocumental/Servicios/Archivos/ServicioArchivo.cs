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
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using LazyCache;
using Newtonsoft.Json;
using Nest;

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

        public ServicioArchivo(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IOptions<ConfiguracionServidor> Config,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
        {
            this.Config = Config;
            this.repo = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoTA = UDT.ObtenerRepositoryAsync<TipoArchivo>(new QueryComposer<TipoArchivo>());
        }



        public async Task<Archivo> CrearAsync(Archivo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Archivo>();

            await seguridad.AccesoValidoArchivo(entity);
            if (!await ExisteTipoArchivos(x => x.Id == entity.TipoArchivoId.Trim()))
                throw new ExDatosNoValidos(entity.TipoArchivoId);

            if (await Existe(x => x.OrigenId.Equals(RegistroActividad.UnidadOrgId, StringComparison.InvariantCultureIgnoreCase)
                                   && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
                                   && x.Eliminada == false
                                   && x.TipoArchivoId.Equals(entity.TipoArchivoId, StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Nombre = entity.Nombre.Trim();
            entity.Id = Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();

        }

        public async Task ActualizarAsync(Archivo entity)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            await seguridad.AccesoValidoArchivo(entity);

            Archivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id && x.OrigenId == entity.OrigenId);
            string original = o.Flat();

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (!await ExisteTipoArchivos(x => x.Id == entity.TipoArchivoId.Trim()))
                throw new ExDatosNoValidos(entity.TipoArchivoId);
            if (await Existe(x => x.OrigenId.Equals(RegistroActividad.UnidadOrgId, StringComparison.InvariantCultureIgnoreCase) &&
                                x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
                                && x.Id != entity.Id && x.Eliminada != true 
                                && x.TipoArchivoId.Equals(entity.TipoArchivoId, StringComparison.InvariantCultureIgnoreCase)
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

            await seguridad.RegistraEventoActualizar(o.Id,  o.Nombre, original.JsonDiff(o.Flat()));
            
        }

        

        private Consulta GetDefaultQuery(Consulta query)
        {
            // Virifica la unidad y el dominio del contexto 
            if (!this.usuario.Accesos.Any(a => a.OU == this.RegistroActividad.UnidadOrgId && a.Dominio == RegistroActividad.DominioId))
            {
                throw new ExErrorDatosSesion();
            }
            
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

            // Añade un filtro permanente para la unidad organizacional
            query.Filtros.RemoveAll(x => x.Propiedad == "OrigenId");
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "OrigenId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.UnidadOrgId });

            return query;
        }
        public async Task<IPaginado<Archivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            List<Archivo> listaEliminados = new List<Archivo>();
            foreach (var Id in ids)
            {
                Archivo c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    await seguridad.AccesoValidoArchivo(c);


                    // No se pued eliminar el archivo sie tiene contenido o una unidad administratica asociada
                    if(UDT.Context.UnidadesAdministrativasArchivo.Any(x=>x.ArchivoTramiteId == Id) 
                        || UDT.Context.UnidadesAdministrativasArchivo.Any(x => x.ArchivoConcentracionId == Id)
                        || UDT.Context.UnidadesAdministrativasArchivo.Any(x => x.ArchivoHistoricoId == Id)
                        || UDT.Context.Activos.Any(x=>x.ArchivoId == Id) )
                    {
                        throw new ExElementoExistente();
                    }

                    listaEliminados.Add(c);
                }
            }

            foreach(var c in listaEliminados)
            {
                c.Eliminada = true;
                UDT.Context.Entry(c).State = EntityState.Modified;
                await seguridad.RegistraEventoEliminar( c.Id, c.Nombre);
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x=>x.Id).ToList();

        }
        
        public Task<List<Archivo>> ObtenerAsync(string SqlCommand)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            return this.repo.ObtenerAsync(SqlCommand);
        }
        
        public Task<List<Archivo>> ObtenerAsync(Expression<Func<Archivo, bool>> predicado)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<Archivo> UnicoAsync(Expression<Func<Archivo, bool>> predicado = null, Func<IQueryable<Archivo>, IOrderedQueryable<Archivo>> ordenarPor = null, Func<IQueryable<Archivo>, IIncludableQueryable<Archivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Archivo a = await this.repo.UnicoAsync(predicado);
            await seguridad.AccesoValidoArchivo(a);
            return a.Copia();
        }

        private async Task<string> RestaurarNombre(string nombre, string TipoArchivoId, string id)
        {
            if (await Existe(x =>
                    x.OrigenId.Equals(RegistroActividad.UnidadOrgId, StringComparison.InvariantCultureIgnoreCase)
                    && x.Id != id && x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)
                    && x.Eliminada == false
                    && x.TipoArchivoId == TipoArchivoId))
            {

                nombre += $" ({DateTime.Now.Ticks})";
            }

            return nombre;
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

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            ICollection<string> listaEliminados = new HashSet<string>();
            List<Archivo> lista = new List<Archivo>();
            foreach (var Id in ids)
            {
                Archivo c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    await seguridad.AccesoValidoArchivo(c);
                    lista.Add(c);
                }
            }

            foreach (var c in lista)
            {
                c.Nombre = await RestaurarNombre(c.Nombre, c.TipoArchivoId, c.Id);
                c.Eliminada = false;
                UDT.Context.Entry(c).State = EntityState.Modified;
                listaEliminados.Add(c.Id);
            }

            UDT.SaveChanges();
            return listaEliminados;
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
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
            var resultados = await this.repo.ObtenerAsync(x => x.OrigenId.Equals( RegistroActividad.UnidadOrgId, StringComparison.InvariantCultureIgnoreCase) 
            && Lista.Contains(x.Id.Trim()));
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
            seguridad.EstableceDatosProceso<Archivo>();
            List<Archivo> purgar = UDT.Context.Archivos.Where(x=>x.OrigenId == RegistroActividad.DominioId && x.Eliminada == true).ToList();
            List<Archivo> listaEliminados = new List<Archivo>();
            foreach (var c in purgar)
            {
                await seguridad.AccesoValidoArchivo(c);


                // sólo se pueden purgar archivos sin vínculos con otros elementps
                if (!(UDT.Context.UnidadesAdministrativasArchivo.Any(x => x.ArchivoTramiteId == c.Id)
                    || UDT.Context.UnidadesAdministrativasArchivo.Any(x => x.ArchivoConcentracionId == c.Id)
                    || UDT.Context.UnidadesAdministrativasArchivo.Any(x => x.ArchivoHistoricoId == c.Id)
                    || UDT.Context.Activos.Any(x => x.ArchivoId == c.Id)
                    || UDT.Context.Transferencias.Any(x=>x.ArchivoOrigenId == c.Id)
                    || UDT.Context.Transferencias.Any(x => x.ArchivoDestinoId == c.Id)
                    ))
                {
                    listaEliminados.Add(c);
                }

            }

            foreach (var c in listaEliminados)
            {
                string sqls;

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaActivoContenedorAlmacen} aa WHERE aa.ContenedorAlmacenId IN (SELECT Id from {DBContextGestionDocumental.TablaContenedorAlmacen} ca WHERE ca.ArchivoId = '{c.Id}')";
                await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaZonasAlmacen} WHERE ArchivoId='{c.Id}'";
                await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaPosicionAlmacen} WHERE ArchivoId='{c.Id}'";
                await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaContenedorAlmacen} WHERE ArchivoId='{c.Id}'";
                await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                sqls = $"DELETE FROM {DBContextGestionDocumental.TablaPermisosArchivo} WHERE ArchivoId='{c.Id}'";
                await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

            }


            return listaEliminados.Select(x => x.Id).ToList();
        }

        /// <summary>
        /// Crea el reporte de guía simple de archivo
        /// </summary>
        /// <param name="ArchivoId">Identificador del archivo para el reporte</param>
        /// <returns></returns>
        public async Task<byte[]> ReporteGuiaSimpleArchivo(string ArchivoId, List<ElementoReporte> reportes)
        {
            seguridad.EstableceDatosProceso<Archivo>();
            GuiaSimpleArchivo g = new GuiaSimpleArchivo
            {
                Archivo = this.UDT.Context.Archivos.Find(ArchivoId)
            };

            await seguridad.AccesoValidoArchivo(g.Archivo);

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
   
            string jsonString = System.Text.Json.JsonSerializer.Serialize(g);
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
            seguridad.EstableceDatosProceso<Archivo>();
            var Archivo = this.UDT.Context.Archivos.Find(ArchivoId);
            if(Archivo == null)
            {
                throw new EXNoEncontrado();
            }

            await seguridad.AccesoValidoArchivo(Archivo);

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

        public Task<Archivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
