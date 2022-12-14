using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestrctura.Reportes;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
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
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioPrestamo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioPrestamo
    {
        private const string DEFAULT_SORT_COL = "FechaCreacion";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Prestamo> repo;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private ILogger<ServicioActivoPrestamo> lap;
        private ILogger<ServicioComentarioPrestamo> lp;
        private ConfiguracionServidor Config;
        private IAppCache cache;
        private IServicioReporteEntidad ServicioReporteEntidad;

        public ServicioPrestamo(
            IAppCache cache,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            IRegistroAuditoria registroAuditoria,
           ILogger<ServicioLog> Logger,
           IOptions<ConfiguracionServidor> Config,
           IServicioReporteEntidad ServicioReporteEntidad) : base(registroAuditoria, proveedorOpciones, Logger,
               cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_PRESTAMO)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Prestamo>(new QueryComposer<Prestamo>());
            this.Config = Config.Value;
            this.ServicioReporteEntidad = ServicioReporteEntidad;
            this.cache = cache;
        }

       
       
        public async Task<bool> Existe(Expression<Func<Prestamo, bool>> predicado)
        {
            List<Prestamo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Prestamo> CrearAsync(Prestamo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Prestamo>();
            await  seguridad.AccesoValidoPrestamo(entity);

            if (await Existe(x => 
                x.ArchivoId == entity.ArchivoId &&
                x.Folio.Equals(entity.Folio, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Folio);
                }

            entity.CantidadActivos = 0;
            entity.Devuelto = false;
            entity.FechaCreacion = DateTime.UtcNow;
            entity.Id = Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Folio);

            return entity.Copia();
        }

        public async Task<Prestamo> CrearDesdeTemaAsync(Prestamo entity, string TemaId, CancellationToken cancellationToken = default) {

            seguridad.EstableceDatosProceso<Prestamo>();
            await seguridad.AccesoValidoPrestamo(entity);
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            string queryIdsArchivo = ArchivosUsuario.AListaSQL();
            if (string.IsNullOrEmpty(queryIdsArchivo))
            {
                queryIdsArchivo = $"and 1=0";
            }
            else
            {
                queryIdsArchivo = $"and a.ArchivoId in ({queryIdsArchivo})";
            }

            string sql = @$"SELECT count(*) FROM  {DBContextGestionDocumental.TablaActivoSelecionado} 
                    s inner join {DBContextGestionDocumental.TablaActivos} a 
                    on s.Id = a.Id where a.Eliminada = 0 {queryIdsArchivo} and s.TemaId = '{TemaId}' and a.EnPrestamo =0 and a.EnTransferencia=0;";


            this.UDT.Context.Database.GetDbConnection().Open();
            var cmd = this.UDT.Context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            int conteo = Convert.ToInt32(cmd.ExecuteScalar());
            if (conteo == 0)
            {
                throw new ExDatosNoValidos("Algunos de los elementos se encuentran en préstamo");
            }

            if (await Existe(x => x.Folio.Equals(entity.Folio, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Folio);
            }

            // Actualiza los activos en prestamo
            sql = @$"update {DBContextGestionDocumental.TablaActivos} set EnPrestamo =1 where Id in 
(SELECT s.Id FROM  {DBContextGestionDocumental.TablaActivoSelecionado} s where s.TemaId = '{TemaId}');";

            await this.UDT.Context.Database.ExecuteSqlRawAsync(sql);


            // Crea rl prestamo
            entity.CantidadActivos = conteo;
            entity.Devuelto = false;
            entity.FechaCreacion = DateTime.UtcNow;
            entity.Id = Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;

            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            List<ActivoSeleccionado> activos = await this.UDT.Context.ActivosSeleccionados.Where(x => x.TemaId == TemaId).ToListAsync();
            activos.ForEach(a =>
            {
                ActivoPrestamo ap = new ActivoPrestamo()
                {
                    ActivoId = a.Id,
                    Devuelto = false,
                    Id = Guid.NewGuid().ToString(),
                    PrestamoId = entity.Id
                };
                UDT.Context.ActivosPrestamo.Add(ap);
            });

            // añade los elementos de prestamo
            sql = $@"INSERT INTO {DBContextGestionDocumental.TablaActivosPrestamo} (Id, PrestamoId, ActivoId, Devuelto,FechaDevolucion ) 
SELECT UUID() as Id, '{entity.Id}' as PrestamoId, s.Id as ActivoId, 0 as Devuelto, null as FechaDevolucion 
FROM  {DBContextGestionDocumental.TablaActivoSelecionado} s inner join {DBContextGestionDocumental.TablaActivos} a 
on s.Id = a.Id where s.TemaId = '{TemaId}';";

            await this.UDT.Context.Database.ExecuteSqlRawAsync(sql);
            this.UDT.Context.Database.GetDbConnection().Close();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Folio);

            return entity.Copia();

        }

        public async Task ActualizarAsync(Prestamo entity)
        {
            seguridad.EstableceDatosProceso<Prestamo>();
            await seguridad.AccesoValidoPrestamo(entity);

            Prestamo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            await seguridad.AccesoValidoPrestamo(o);

            string original = System.Text.Json.JsonSerializer.Serialize(o.Copia());

            if (await Existe(x =>
            x.Id != entity.Id && x.Folio == entity.Folio))
            {
                throw new ExElementoExistente(entity.Folio);
            }

            o.FechaProgramadaDevolucion = entity.FechaProgramadaDevolucion;
            o.Descripcion = entity.Descripcion;

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar( o.Id, o.Folio, original.JsonDiff(JsonConvert.SerializeObject(o.Copia())));
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

        public async Task<IPaginado<Prestamo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Prestamo>, 
            IIncludableQueryable<Prestamo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Prestamo>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            List<Expression<Func<Prestamo, bool>>> filtros = new List<Expression<Func<Prestamo, bool>>>();
            filtros.Add(x => ArchivosUsuario.Contains(x.ArchivoId));

            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null, filtros);

            return respuesta;
        }

        public Task<IEnumerable<Prestamo>> CrearAsync(params Prestamo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Prestamo>> CrearAsync(IEnumerable<Prestamo> entities, CancellationToken cancellationToken = default)
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
            seguridad.EstableceDatosProceso<Prestamo>();
            List<Prestamo> listaEliminados = new List<Prestamo>();
            foreach (var Id in ids)
            {
                Prestamo p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {
                    await seguridad.AccesoValidoPrestamo(p);
                    listaEliminados.Add(p);
                }
            }

            foreach(var p in listaEliminados)
            {

                string sql;
                // Si el prestamo ha sido devuelto los activos ya se han marcado como no prestados
                if (!p.Devuelto)
                {
                    // Actualiza los activos en prestamo
                    sql = @$"update {DBContextGestionDocumental.TablaActivos} set EnPrestamo =0 where Id in 
 (SELECT s.ActivoId FROM  {DBContextGestionDocumental.TablaActivosPrestamo} s where s.PrestamoId = '{p.Id}');";
                    await this.UDT.Context.Database.ExecuteSqlRawAsync(sql);
                }

                // Una vez actualziados se eliminan las entradas de la relación
                sql = $"DElETE FROM {DBContextGestionDocumental.TablaActivosPrestamo} where PrestamoId='{p.Id}'";
                await this.UDT.Context.Database.ExecuteSqlRawAsync(sql);



                UDT.Context.Entry(p).State = EntityState.Deleted;
                await seguridad.RegistraEventoEliminar( p.Id, p.Folio);
            }

            if (listaEliminados.Count > 0)
            {
                UDT.SaveChanges();
            }
            
            return listaEliminados.Select(p=>p.Id).ToList();
        }

        public Task<List<Prestamo>> ObtenerAsync(Expression<Func<Prestamo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Prestamo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<Prestamo>> ObtenerPaginadoAsync(Expression<Func<Prestamo, bool>> predicate = null, Func<IQueryable<Prestamo>, IOrderedQueryable<Prestamo>> orderBy = null, Func<IQueryable<Prestamo>, IIncludableQueryable<Prestamo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<Prestamo> UnicoAsync(Expression<Func<Prestamo, bool>> predicado = null, Func<IQueryable<Prestamo>, IOrderedQueryable<Prestamo>> ordenarPor = null, Func<IQueryable<Prestamo>, IIncludableQueryable<Prestamo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Prestamo p = await this.repo.UnicoAsync(predicado);
            await seguridad.AccesoValidoPrestamo(p); 
            return p.Copia();
        }

        public async Task<List<string>> Purgar()
        {
            //List<Prestamo> ListaPrestamo = await this.repo.ObtenerAsync(x=>x.Eliminada==true);
            //if (ListaPrestamo.Count > 0)
            //{
            //    ServicioActivoPrestamo sap = new ServicioActivoPrestamo(this.cache, this.registroAuditoria, this.proveedorOpciones, this.logger);
            //    ServicioComentarioPrestamo scp = new ServicioComentarioPrestamo(this.registroAuditoria, this.proveedorOpciones, this.logger);
            //    string[] IdPrestamoeliminados = ListaPrestamo.Select(x => x.Id).ToArray();
            //    await sap.EliminarActivosPrestamos(1, IdPrestamoeliminados);
            //    List<ComentarioPrestamo> cp = await scp.ObtenerAsync(x => x.PrestamoId.Contains(ListaPrestamo.Select(x => x.Id).FirstOrDefault()));
            //    string[] ComentariosPrestamosId = cp.Select(x => x.Id).ToArray();
            //    scp = new ServicioComentarioPrestamo(this.registroAuditoria, this.proveedorOpciones,this.logger);
            //    await scp.Eliminar(ComentariosPrestamosId);
            //}
            //return ListaPrestamo.Select(x=>x.Id).ToList();
            throw new NotImplementedException();
        }


        public async Task<RespuestaComandoWeb> ComandoWeb(string command, object payload) {

            dynamic d = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(payload));
            var id = Convert.ToString(d.Id);
            switch (command)
            {
                case "gd-prestamo-entregar":
                    return await EntregaPrestamo(id);

                case "gd-prestamo-devolver":

                    
                    var x = await DevuelvePrestamo(Convert.ToString(id));
                    return x;

                default:
                    return new RespuestaComandoWeb() { Estatus = false, MensajeId = RespuestaComandoWeb.Novalido, Payload = null }; 
            }
        }


        private async Task<RespuestaComandoWeb> DevuelvePrestamo(string Id)
        {
            seguridad.EstableceDatosProceso<Prestamo>();
            RespuestaComandoWeb r = new RespuestaComandoWeb() { Estatus = false };
            
            Prestamo p = await this.UDT.Context.Prestamos.FindAsync(Id);

            if (p != null)
            {
                await seguridad.AccesoValidoPrestamo(p);
                string original = JsonConvert.SerializeObject(p.Copia());

                if (p.Entregado == false || p.Devuelto == true)
                {
                    r.MensajeId = "error-prestamo-devuelto";
                }
                p.Devuelto = true;
                p.FechaDevolucion = DateTime.UtcNow;
                UDT.Context.Entry(p).State = EntityState.Modified;
                await UDT.Context.SaveChangesAsync();
                r.MensajeId = "prestamo-devuelto";
                r.Estatus = true;


                // Actualiza los activos en prestamo
                string sql = @$"update {DBContextGestionDocumental.TablaActivos} set EnPrestamo =0 where Id in 
 (SELECT s.ActivoId FROM  {DBContextGestionDocumental.TablaActivosPrestamo} s where s.PrestamoId = '{p.Id}');";
                await this.UDT.Context.Database.ExecuteSqlRawAsync(sql);

                sql = @$"update {DBContextGestionDocumental.TablaActivosPrestamo} set Devuelto =1, FechaDevolucion='{p.FechaDevolucion.Value.ToString("s")}' where PrestamoId = '{p.Id}'";
                await this.UDT.Context.Database.ExecuteSqlRawAsync(sql);



                await seguridad.RegistraEventoActualizar( p.Id, p.Folio, original.JsonDiff(JsonConvert.SerializeObject(p.Copia())));

            }
            else
            {
                r.MensajeId = RespuestaComandoWeb.NoEncontrado;
            }

            return r;
        }


        private async Task<RespuestaComandoWeb> EntregaPrestamo(string Id) {
            seguridad.EstableceDatosProceso<Prestamo>();
            RespuestaComandoWeb r = new RespuestaComandoWeb() { Estatus = false };
            Prestamo p = await this.UDT.Context.Prestamos.FindAsync(Convert.ToString(Id));

            if (p != null)
            {
                await seguridad.AccesoValidoPrestamo(p);
                string original = JsonConvert.SerializeObject(p);
                if (p.Entregado)
                {
                    r.MensajeId = "error-prestamo-entregado";
                }
                p.Entregado = true;
                UDT.Context.Entry(p).State = EntityState.Modified;
                await UDT.Context.SaveChangesAsync();
                r.MensajeId = "prestamo-entregado";
                r.Estatus = true;

                await seguridad.RegistraEventoActualizar(p.Id, p.Folio, original.JsonDiff(JsonConvert.SerializeObject(p)));
            }
            else
            {
                r.MensajeId = RespuestaComandoWeb.NoEncontrado;
            }

            return r;
        }



        /// <summary>
        /// Crea el reporte de guía simple de archivo
        /// </summary>
        /// <param name="ArchivoId">Identificador del archivo para el reporte</param>
        /// <returns></returns>
        public async Task<byte[]> ReportePrestamo(string ExtensionSalida, string PrestamoId, UsuarioPrestamo Prestador, UsuarioPrestamo Prestatario, string Dominio= "*", string UnidadOrganizacinal = "*")
        {

            seguridad.EstableceDatosProceso<Prestamo>();
            logger.LogInformation("ReportePrestamo");
            ReportePrestamoData g = new ReportePrestamoData
            {
                Prestamo = await repo.UnicoAsync(x => x.Id == PrestamoId),
                Prestador = Prestador,
                Prestatario = Prestatario
            };

            await seguridad.AccesoValidoPrestamo(g.Prestamo);

            g.Fecha = DateTime.Now.ToString("dd/MM/yyyy");
            g.FechaDevolucionPrevista = g.Prestamo.FechaProgramadaDevolucion.ToString("dd/MM/yyyy");
            g.FechaDevolucionReal = g.Prestamo.FechaDevolucion.HasValue ? g.Prestamo.FechaDevolucion.Value.ToString("dd/MM/yyyy") : "";
            g.Estado = g.Prestamo.Devuelto ? "Devuelto" : "Abierto";

            if (g.Prestamo == null) throw new EXNoEncontrado(PrestamoId);

            var r = await ServicioReporteEntidad.UnicoAsync(x => x.Id == "reporte-prestamo" && x.TipoOrigenId == Dominio && x.OrigenId == UnidadOrganizacinal);
            if (r == null) throw new EXNoEncontrado("reporte: reporte-prestamo");


            string sqls = $@"select a.*  from gd$activoprestamo ap inner join gd$prestamo p on ap.PrestamoId = p.Id
inner join gd$activo a on ap.ActivoId = a.Id where p.Id = '{PrestamoId}'";


            var activos = this.UDT.Context.Activos.FromSqlRaw(sqls).OrderBy(x => x.Nombre).ToList();

            int indice = 1;
            activos.ForEach(x =>
            {
                g.Activos.Add(x.CopiaActivoPrestamo(indice));
                indice++;
            });


            string jsonString = System.Text.Json.JsonSerializer.Serialize(g);
            byte[] data = Convert.FromBase64String(r.Plantilla);


            await seguridad.RegistraEventoCrear(g.Prestamo.Id, g.Prestamo.Folio);

            return ReporteEntidades.ReportePlantilla(data, jsonString, Config.ruta_cache_fisico, true);

        }


        public async Task<Prestamo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        // ---------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------------



    }
}
