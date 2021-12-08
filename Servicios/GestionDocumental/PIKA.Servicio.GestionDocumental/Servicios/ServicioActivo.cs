using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestrctura.Reportes;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Modelo.GestorDocumental.Temas;
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

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public partial class ServicioActivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivo
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";


        //private IRepositorioAsync<CuadroClasificacion> repoCC;
        //private IRepositorioAsync<ElementoClasificacion> repoEL;
        //private IRepositorioAsync<EntradaClasificacion> repoEC;
        //private IRepositorioAsync<Archivo> repoA;


        private IRepositorioAsync<Activo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private IServicioEstadisticaClasificacionAcervo servEstadisticas;
        private readonly IAppCache cache;
        private IOptions<ConfiguracionServidor> Config;
        private IServicioReporteEntidad ServicioReporteEntidad;

        public ServicioActivo(
            IAppCache cache,
            IServicioReporteEntidad ServicioReporteEntidad,
            IServicioEstadisticaClasificacionAcervo servEstadisticas,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger, IOptions<ConfiguracionServidor> Config
           ) : base(proveedorOpciones, Logger)
        {
            this.ServicioReporteEntidad = ServicioReporteEntidad;
            this.servEstadisticas = servEstadisticas;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
            this.cache = cache;
            this.Config = Config;
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

        public async Task<bool> Existe(Expression<Func<Activo, bool>> predicado)
        {
            return await repo.UnicoAsync(predicado) != null; ;
        }

        public async Task<Activo> UnicoAsync(Expression<Func<Activo, bool>> predicado = null, Func<IQueryable<Activo>, IOrderedQueryable<Activo>> ordenarPor = null, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Activo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        public Task<List<Activo>> ObtenerAsync(Expression<Func<Activo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

      


        public async Task<Activo> CrearAsync(Activo entity, CancellationToken cancellationToken = default)
        {
            Activo valido = await ActivoValidado(entity, false);
            
            await this.repo.CrearAsync(valido);
            UDT.SaveChanges();
            
            await servEstadisticas.ActualizaConteoEstadistica(valido.CuadroClasificacionId,
                valido.EntradaClasificacionId, valido.ArchivoId, 1,
                valido.FechaApertura, valido.FechaCierre);

            return valido.Copia();

        }

        public async Task ActualizarAsync(Activo entity)
        {
            Activo valido = await ActivoValidado(entity, true);
            UDT.Context.Entry(valido).State = EntityState.Modified;
            UDT.SaveChanges();

            if(entity.EntradaClasificacionId != valido.EntradaClasificacionId)
            {
                // Actualiza la entarda previa
                await servEstadisticas.ActualizaConteoEstadistica(entity.CuadroClasificacionId,
                entity.EntradaClasificacionId, entity.ArchivoId, -1, null, null);

                // Actualiza la entrada nueva
                await servEstadisticas.ActualizaConteoEstadistica(valido.CuadroClasificacionId,
                valido.EntradaClasificacionId, entity.ArchivoId, 1,
                valido.FechaApertura, valido.FechaCierre);
            } 
            else
            {
                // Solo actualiz a las fechas extremas
                await servEstadisticas.ActualizaConteoEstadistica(valido.CuadroClasificacionId,
                valido.EntradaClasificacionId, entity.ArchivoId, 0,
                valido.FechaApertura, valido.FechaCierre);
            }
        }


        public async Task<IPaginado<Activo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);

            List<string> especiales = new List<string>() { "Vencidos" };
            List<FiltroConsulta> filtrosespeciales = new List<FiltroConsulta>();
            List<Expression<Func<Activo, bool>>> filtros = new List<Expression<Func<Activo, bool>>>();

            foreach (var f in Query.Filtros)
            {
                if (especiales.IndexOf(f.Propiedad) >= 0)
                {
                    filtrosespeciales.Add(f);
                }
            }

            foreach (var f in filtrosespeciales)
            {
                Query.Filtros.Remove(f);
                switch (f.Propiedad)
                {
                    case "Vencidos":
                        filtros.Add(ObtieneFiltroVencimientos(f));
                        break;
                }
            }

            IPaginado<Activo> respuesta = null;
            if (string.IsNullOrEmpty(Query.IdSeleccion))
            {
                respuesta = await this.repo.ObtenerPaginadoAsync(Query, null, filtros);
            }
            else {
                respuesta = await ObtenerPaginadoSeleccionAsync(Query);
            }

            
            await ObtieneVencimientos(respuesta);

            return respuesta;
        }


        private async Task<IPaginado<Activo>> ObtenerPaginadoSeleccionAsync(Consulta Query)
        {
            Paginado<Activo> p = new Paginado<Activo>();
            string sqls = @$"
select a.*  from {DBContextGestionDocumental.TablaActivos} a inner join {DBContextGestionDocumental.TablaActivoSelecionado} p
on a.Id = p.Id 
where p.TemaId='{Query.IdSeleccion}' 
order by {Query.ord_columna} {Query.ord_direccion} 
limit {Query.indice *  Query.tamano}, {Query.tamano};";

            Console.WriteLine(sqls);
            try
            {
                p.Elementos = await this.UDT.Context.Activos.FromSqlRaw(sqls, new object[] { }).ToListAsync();
                p.ConteoFiltrado = await this.UDT.Context.ActivosSeleccionados.Where(x => x.TemaId == Query.IdSeleccion).CountAsync();
                p.ConteoTotal = p.ConteoFiltrado;

                p.Indice = Query.indice;
                p.Paginas = 0;
                p.Tamano = Query.tamano;
                p.Desde = Query.indice * Query.tamano;

                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }



        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            List<Activo> eliminados = new List<Activo>();
            Activo c;
            
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {

                    c.Eliminada = true;
                    UDT.Context.Entry(c).State = EntityState.Modified;
                    eliminados.Add(c.Copia());

                }
            }
            UDT.SaveChanges();

            var actualizables = eliminados.GroupBy(x => new { x.CuadroClasificacionId, x.EntradaClasificacionId, x.ArchivoId })
                .Select(y => new {
                    CCId = y.Key.CuadroClasificacionId,
                    ECId= y.Key.EntradaClasificacionId,
                    AId= y.Key.ArchivoId,
                    Conteo = (y.Count() * -1)
             });


            foreach( var item  in actualizables)
            {
                await servEstadisticas.ActualizaConteoEstadistica(item.CCId, item.ECId, item.AId, item.Conteo, null, null);
            }

            return eliminados.Select(x=>x.Id).ToList();

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            List<Activo> restaurados = new List<Activo>();
            Activo c;
            
            foreach (var Id in ids)
            {

                c = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (c != null)
                {
                    c.Nombre = await RestaurarNombre(c.Nombre, c.ArchivoId, c.Id, c.EntradaClasificacionId);
                    c.Eliminada = false;
                    UDT.Context.Entry(c).State = EntityState.Modified;

                    restaurados.Add(c.Copia());

                }
            }
            UDT.SaveChanges();

            var actualizables = restaurados.GroupBy(x => new { x.CuadroClasificacionId, x.EntradaClasificacionId, x.ArchivoId })
                       .Select(y => new {
                           CCId = y.Key.CuadroClasificacionId,
                           ECId = y.Key.EntradaClasificacionId,
                           AId = y.Key.ArchivoId,
                           Conteo = y.Count() 
                       });

            foreach (var item in actualizables)
            {
                await servEstadisticas.ActualizaConteoEstadistica(item.CCId, item.ECId, item.AId, item.Conteo, null, null);
            }

            return restaurados.Select(x=>x.Id).ToList();
        }

        #region métodos soporte CRUD


        private async Task<string> RestaurarNombre(string nombre, string ArchivoId, string id, string IdElemento)
        {

            if (await Existe(x =>
                    x.Nombre.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)
                    && x.ArchivoId.Equals(ArchivoId, StringComparison.InvariantCultureIgnoreCase)
                    && x.EntradaClasificacionId.Equals(IdElemento, StringComparison.InvariantCultureIgnoreCase)
                    && x.Id != id
                    && x.Eliminada == false
                    ))
            {

                nombre = $"{nombre} {DateTime.Now.Ticks}";
            }
            else
            {
            }

            return nombre;
        }



        private async Task<Activo> ActivoValidado(Activo activo, bool actualizar)
        {
            Activo a = activo.Copia();

            IRepositorioAsync<EntradaClasificacion> repoEC = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());
            IRepositorioAsync<Archivo> repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());

            EntradaClasificacion ec = await repoEC.UnicoAsync(x => x.Id == a.EntradaClasificacionId);

            if (ec == null || ec.Eliminada == true) throw new ExErrorRelacional(a.EntradaClasificacionId);

            Archivo archivo = await repoA.UnicoAsync(x => x.Id.Equals(a.ArchivoId.Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (archivo == null || archivo.Eliminada) throw new ExErrorRelacional(a.ArchivoId);

            if (actualizar)
            {
                if (!string.IsNullOrEmpty(a.IDunico) && await Existe(x => x.Id != a.Id && x.Eliminada == false &&
                    x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ExElementoExistente(a.IDunico);
                var tmp = await this.repo.UnicoAsync(x => x.Id == a.Id);
                if (tmp == null)
                {
                    throw new EXNoEncontrado(a.Id);
                }

                
                a.ArchivoOrigenId = tmp.ArchivoOrigenId;
            }
            else
            {
                if (!string.IsNullOrEmpty(a.IDunico) && await Existe(x => x.Eliminada == false &&
                    x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
                    throw new ExElementoExistente(a.IDunico);
                a.ArchivoOrigenId = a.ArchivoId;
                a.Id = Guid.NewGuid().ToString();
            }

            a.TipoArchivoId = archivo.TipoArchivoId;
            a.EntradaClasificacionId = ec.Id;
            a.CuadroClasificacionId = ec.CuadroClasifiacionId;

            if (a.FechaCierre.HasValue)
            {
                a.FechaRetencionAT = ((DateTime)a.FechaCierre).AddYears(ec.VigenciaTramite);
                a.FechaRetencionAC = ((DateTime)a.FechaCierre).AddYears(ec.VigenciaTramite + ec.VigenciaConcentracion);
            }
            else
            {
                a.FechaRetencionAT = null;
                a.FechaRetencionAC = null;
            }
            return a;
        }


        private async Task ObtieneVencimientos(IPaginado<Activo> respuesta)
        {

            IRepositorioAsync<EntradaClasificacion> repoEC = UDT.ObtenerRepositoryAsync<EntradaClasificacion>(new QueryComposer<EntradaClasificacion>());

            // Busca las entradas de cuaddro qu eno esten en caché para el calculo de vencimientos
            List<string> idsEntrada = new List<string>();
            respuesta.Elementos.GroupBy(x => x.EntradaClasificacionId)
                .Select(x => new { id = x.Key }).ToList().ForEach(e =>
                {
                    EntradaClasificacion ec = this.cache.Get<EntradaClasificacion>($"ec-{e.id}");
                    if (ec == null)
                    {
                        idsEntrada.Add(e.id);
                    }
                });


            if (idsEntrada.Count > 0)
            {
                List<EntradaClasificacion> l = await repoEC.ObtenerAsync(x => idsEntrada.Contains(x.Id.Trim()));
                foreach (var ec in l)
                {
                    cache.Add<EntradaClasificacion>($"ec-{ec.Id}", ec, DateTimeOffset.UtcNow.AddSeconds(60));
                }
            }

            for (int i = 0; i < respuesta.Elementos.Count; i++)
            {
                EntradaClasificacion ec = this.cache.Get<EntradaClasificacion>($"ec-{respuesta.Elementos[i].EntradaClasificacionId}");
                if (ec != null)
                {
                    if (respuesta.Elementos[i].FechaCierre.HasValue)
                    {
                        switch (respuesta.Elementos[i].TipoArchivoId)
                        {
                            case TipoArchivo.IDARCHIVO_CONSERVACION:
                                var lapsoc = ((DateTime)respuesta.Elementos[i].FechaRetencionAC) - DateTime.UtcNow;
                                if (lapsoc.TotalDays >= 0) respuesta.Elementos[i].Vencidos = (int)lapsoc.TotalDays;
                                break;

                            case TipoArchivo.IDARCHIVO_TRAMITE:
                                var lapsoa = ((DateTime)respuesta.Elementos[i].FechaRetencionAC) - DateTime.UtcNow;
                                if (lapsoa.TotalDays >= 0) respuesta.Elementos[i].Vencidos = (int)lapsoa.TotalDays;
                                break;
                        }
                    }

                }
            }
        }

        private Expression<Func<Activo, bool>> ObtieneFiltroVencimientos(FiltroConsulta f)
        {
            DateTime fechaLimite = DateTime.Now.AddDays(int.Parse(f.Valor));
            return x => (x.TipoArchivoId == TipoArchivo.IDARCHIVO_TRAMITE
                           && x.FechaRetencionAT < fechaLimite)
                           ||
                           (x.TipoArchivoId == TipoArchivo.IDARCHIVO_CONSERVACION
                           && x.FechaRetencionAC < fechaLimite
                           );
        }

        #endregion


        #region reportes

        /// <summary>
        /// Crea el reporte de guía simple de archivo
        /// </summary>
        /// <param name="ArchivoId">Identificador del archivo para el reporte</param>
        /// <returns></returns>
        public async Task<byte[]> ReporteCaratulaActivo(string Dominio, string UnidadOrganizacinal, string ActivoId)
        {
            try
            {
                logger.LogInformation("Reporte");
                ActivoAcervo g = new ActivoAcervo
                {
                    Activo = await repo.UnicoAsync(x => x.Id == ActivoId, null,
                    y => y.Include(z => z.EntradaClasificacion)
                    .ThenInclude(a => a.DisposicionEntrada)),
                    Dominio = Dominio,
                    FechaApertura = "",
                    FechaCierre = "",
                    FechaRetencionAC = "",
                    FechaRetencionAT = "",
                    TecnicaSeleccion = "",
                    UnidadOrganizacional = UnidadOrganizacinal,
                    Valoraciones = new List<TipoValoracionDocumental>()
                };

                if (g.Activo == null) throw new EXNoEncontrado(ActivoId);

                var r = await ServicioReporteEntidad.UnicoAsync(x => x.Id == "caratulaactivo");
                if (r == null) throw new EXNoEncontrado("reporte: caratulaactivo");

                g.FechaApertura = g.Activo.FechaApertura.ToString("dd/MM/yyyy");
                g.FechaCierre = g.Activo.FechaCierre.HasValue ? g.Activo.FechaCierre.Value.ToString("dd/MM/yyyy") : "";
                g.FechaRetencionAC = g.Activo.FechaRetencionAC.HasValue ? g.Activo.FechaRetencionAC.Value.ToString("dd/MM/yyyy") : "";
                g.FechaRetencionAT = g.Activo.FechaRetencionAT.HasValue ? g.Activo.FechaRetencionAT.Value.ToString("dd/MM/yyyy") : "";
                g.TecnicaSeleccion = g.Activo.EntradaClasificacion.DisposicionEntrada.Nombre;
                g.EntradaClasificacion = g.Activo.EntradaClasificacion;
                foreach (var item in UDT.Context.ValoracionEntradaClasificacion.Where(x => x.EntradaClasificacionId == g.Activo.EntradaClasificacionId).ToList())
                {
                    g.Valoraciones.Add(UDT.Context.TipoValoracionDocumental.Find(item.TipoValoracionDocumentalId));
                }

                string jsonString = JsonSerializer.Serialize(g);

                byte[] data = Convert.FromBase64String(r.Plantilla);

                return ReporteEntidades.ReportePlantilla(data, jsonString, Config.Value.ruta_cache_fisico, true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }

          
        }

        #endregion

        // ---------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------------







        public Task<List<Activo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
      



        public async Task<List<string>> Purgar()
        {

            await Task.Delay(1);
            return null;

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
                    Negacion = false,
                    Operador = "eq",
                    Valor = "false"
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

            return l.OrderBy(x => x.Texto).ToList();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }




        #region ActivosSeleccionados

        public async Task<List<Activo>> ObtieneSeleccionados(string TemaId, string UsuarioId)
        {
            if (!Guid.TryParse(UsuarioId, out _)) throw new ExDatosNoValidos();
            string template = @$"select *  from {DBContextGestionDocumental.TablaActivos} where Id in (
                select Id from {DBContextGestionDocumental.TablaActivoSelecionado} where UsuarioId = '{UsuarioId}' and TemaId='{TemaId}')";
            return await UDT.Context.Activos.FromSqlRaw(template, null).ToListAsync();
        }

        private async Task CrearSeleccionLocal(string TemaId, string Id, string UsuarioId, bool guardar = true)
        {
            if (!(await this.UDT.Context.ActivosSeleccionados.AnyAsync(x => x.Id == Id && x.UsuarioId == UsuarioId && x.TemaId == TemaId)))
            {
                var item = new ActivoSeleccionado()
                {
                    TemaId = TemaId,
                    UsuarioId = UsuarioId,
                    Id = Id
                };
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(item));
                this.UDT.Context.ActivosSeleccionados.Add(item);



                if (guardar) await this.UDT.Context.SaveChangesAsync();
            }
        }

        public async Task CrearSeleccion(string TemaId, string Id, string UsuarioId)
        {
            await this.CrearSeleccionLocal(TemaId, Id, UsuarioId, false);
        }

        private async Task EliminaSeleccionLocal(string Id, string TemaId, string UsuarioId, bool guardar = true)
        {
            var seleccionado = await this.UDT.Context.ActivosSeleccionados.Where(x => x.Id == Id && x.UsuarioId == UsuarioId && x.TemaId == TemaId).SingleOrDefaultAsync();
            if (seleccionado != null)
            {
                this.UDT.Context.ActivosSeleccionados.Remove(seleccionado);
                if (guardar) await this.UDT.Context.SaveChangesAsync();
            }
        }

        public async Task EliminaSeleccion(string Id, string TemaId, string UsuarioId)
        {
            await this.EliminaSeleccionLocal(Id, TemaId, UsuarioId, false);
        }

        public async Task CrearSeleccion(List<string> Ids, string TemaId, string UsuarioId)
        {
            try
            {
                foreach (string id in Ids)
                {
                    await this.CrearSeleccionLocal(TemaId, id, UsuarioId, false);
                }
                await this.UDT.Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        public async Task EliminaSeleccion(List<string> Ids, string TemaId, string UsuarioId)
        {
            foreach (string id in Ids)
            {
                await this.EliminaSeleccionLocal(id, TemaId, UsuarioId, false);
            }
            await this.UDT.Context.SaveChangesAsync();
        }

        public async Task BorraSeleccion(string TemaId, string UsuarioId)
        {
            var seleccionados = await this.UDT.Context.ActivosSeleccionados.Where(x => x.UsuarioId == UsuarioId && x.TemaId == TemaId).ToListAsync();
            if (seleccionados != null && seleccionados.Count > 0)
            {
                this.UDT.Context.ActivosSeleccionados.RemoveRange(seleccionados);
                await this.UDT.Context.SaveChangesAsync();
            }
        }

        public async Task<string> CreaTema(string Tema, string UsuarioId)
        {
            Tema = Tema.TrimEnd().TrimStart();
            var tema = await this.UDT.Context.TemasActivos.Where(x => x.UsuarioId == UsuarioId &&
            x.Nombre.Equals(Tema, StringComparison.OrdinalIgnoreCase)).SingleOrDefaultAsync();
            if (tema == null)
            {
                TemaActivos t = new TemaActivos()
                {
                    Id = Guid.NewGuid().ToString(),
                    Nombre = Tema,
                    UsuarioId = UsuarioId
                };

                this.UDT.Context.TemasActivos.Add(t);
                await this.UDT.Context.SaveChangesAsync();

                return t.Id;
            }
            else
            {
                throw new ExElementoExistente(Tema);
            }

        }

        public async Task EliminaTema(string Id, string UsuarioId)
        {
            var tema = await this.UDT.Context.TemasActivos.Where(x => x.UsuarioId == UsuarioId && x.Id == Id).SingleOrDefaultAsync();
            if (tema != null)
            {
                this.UDT.Context.TemasActivos.Remove(tema);
                await this.UDT.Context.SaveChangesAsync();
            }
            else
            {
                throw new ExElementoExistente(Id);
            }

        }

        public async Task<List<ValorListaOrdenada>> ObtienTemas(string UsuarioId)
        {
            var lista = await this.UDT.Context.TemasActivos.Where(x => x.UsuarioId == UsuarioId).OrderBy(x => x.Nombre).ToListAsync();
            return lista.Select(x => new ValorListaOrdenada { Id = x.Id, Texto = x.Nombre, Indice = 0 }).ToList();
        }


        #endregion



        #region Sin Implementar

        public Task<IEnumerable<Activo>> CrearAsync(params Activo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Activo>> CrearAsync(IEnumerable<Activo> entities, CancellationToken cancellationToken = default)
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


        public Task<IPaginado<Activo>> ObtenerPaginadoAsync(Expression<Func<Activo, bool>> predicate = null, Func<IQueryable<Activo>, IOrderedQueryable<Activo>> orderBy = null, Func<IQueryable<Activo>, IIncludableQueryable<Activo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
