using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using PIKA.Servicio.Reportes.Servicios;
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
        private IServicioReporteEntidad ServicioReporteEntidad;

        public ServicioPrestamo(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
           ILogger<ServicioLog> Logger,
           IOptions<ConfiguracionServidor> Config,
           IServicioReporteEntidad ServicioReporteEntidad) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Prestamo>(new QueryComposer<Prestamo>());
            this.Config = Config.Value;
            this.ServicioReporteEntidad = ServicioReporteEntidad;
        }

        public async Task<bool> Existe(Expression<Func<Prestamo, bool>> predicado)
        {
            List<Prestamo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Prestamo> CrearAsync(Prestamo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x => x.Folio.Equals(entity.Folio, StringComparison.InvariantCultureIgnoreCase)))
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
            return entity.Copia();
        }

        public async Task<Prestamo> CrearDesdeTemaAsync(Prestamo entity, string TemaId, CancellationToken cancellationToken = default) {

            try
            {


            string sql = @$"SELECT count(*) FROM  {DBContextGestionDocumental.TablaActivoSelecionado} s inner join {DBContextGestionDocumental.TablaActivos} a 
                    on s.Id = a.Id where s.TemaId = '{TemaId}' and a.EnPrestamo =0 and a.EnTransferencia=0;";


            this.UDT.Context.Database.GetDbConnection().Open();
            var cmd = this.UDT.Context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            int conteo = Convert.ToInt32( cmd.ExecuteScalar());
            if(conteo==0)
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
                activos.ForEach(a => {
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
            return entity.Copia();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task ActualizarAsync(Prestamo entity)
        {

            Prestamo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.Folio == entity.Folio))
            {
                throw new ExElementoExistente(entity.Folio);
            }

            o.Folio = entity.Folio;
            o.Eliminada = entity.Eliminada;
            o.FechaProgramadaDevolucion = entity.FechaProgramadaDevolucion;
            o.FechaDevolucion = entity.FechaDevolucion;
            o.TieneDevolucionesParciales = entity.TieneDevolucionesParciales;

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
        public async Task<IPaginado<Prestamo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Prestamo>, 
            IIncludableQueryable<Prestamo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

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
            Prestamo p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {
                    p.Eliminada = true;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                    listaEliminados.Add(p.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public async Task<ICollection<string>> EliminarPrestamo(string[] ids)
        {
            Prestamo p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {
                    p.Eliminada = true;
                    UDT.Context.Entry(p).State = EntityState.Deleted;
                    listaEliminados.Add(p.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
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
            return p.Copia();
        }

        public async Task<List<string>> Purgar()
        {
            List<Prestamo> ListaPrestamo = await this.repo.ObtenerAsync(x=>x.Eliminada==true);
            if (ListaPrestamo.Count > 0)
            {
                ServicioActivoPrestamo sap = new ServicioActivoPrestamo(this.proveedorOpciones, this.logger);
                ServicioComentarioPrestamo scp = new ServicioComentarioPrestamo(this.proveedorOpciones, this.logger);
                string[] IdPrestamoeliminados = ListaPrestamo.Select(x => x.Id).ToArray();
                await sap.EliminarActivosPrestamos(1, IdPrestamoeliminados);
                List<ComentarioPrestamo> cp = await scp.ObtenerAsync(x => x.PrestamoId.Contains(ListaPrestamo.Select(x => x.Id).FirstOrDefault()));
                string[] ComentariosPrestamosId = cp.Select(x => x.Id).ToArray();
                scp = new ServicioComentarioPrestamo(this.proveedorOpciones,this.logger);
                await scp.Eliminar(ComentariosPrestamosId);
            }
            return ListaPrestamo.Select(x=>x.Id).ToList();
        }


        public async Task<RespuestaComandoWeb> ComandoWeb(string command, object payload) {

            switch (command)
            {
                case "gd-prestamo-entregar":
                    return await EntregaPrestamo(payload);

                case "gd-prestamo-devolver":
                    return await DevuelvePrestamo(payload);

                default:
                    return new RespuestaComandoWeb() { Estatus = false, MensajeId = RespuestaComandoWeb.Novalido, Payload = null }; 
            }
        }


        private async Task<RespuestaComandoWeb> DevuelvePrestamo(object payload)
        {
            RespuestaComandoWeb r = new RespuestaComandoWeb() { Estatus = false };
            try
            {
                dynamic d = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(payload));
                Prestamo p = await this.UDT.Context.Prestamos.FindAsync(Convert.ToString(d.Id));
                if (p != null)
                {
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
                }
                else
                {
                    r.MensajeId = RespuestaComandoWeb.NoEncontrado;
                }
            }
            catch (Exception ex)
            {
                r.MensajeId = RespuestaComandoWeb.ErrorProceso;
            }
            return r;
        }


        private async Task<RespuestaComandoWeb> EntregaPrestamo(object payload) {
            RespuestaComandoWeb r = new RespuestaComandoWeb() { Estatus = false };
            try
            {
                dynamic d = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(payload));
                Prestamo p = await this.UDT.Context.Prestamos.FindAsync(Convert.ToString( d.Id));
                if (p != null)
                {
                    if (p.Entregado)
                    {
                        r.MensajeId= "error-prestamo-entregado";
                    }
                    p.Entregado = true;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                    await UDT.Context.SaveChangesAsync();
                    r.MensajeId = "prestamo-entregado";
                    r.Estatus = true;
                }
                else
                {
                    r.MensajeId = RespuestaComandoWeb.NoEncontrado;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                r.MensajeId =  RespuestaComandoWeb.ErrorProceso;
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
            try
            {
                logger.LogInformation("ReportePrestamo");
                ReportePrestamoData g = new ReportePrestamoData
                {
                    Prestamo = await repo.UnicoAsync(x => x.Id == PrestamoId),
                    Prestador = Prestador, 
                    Prestatario = Prestatario
                };

                g.Fecha = DateTime.Now.ToString("dd/MM/yyyy");
                g.FechaDevolucionPrevista = g.Prestamo.FechaProgramadaDevolucion.ToString("dd/MM/yyyy");
                g.FechaDevolucionReal = g.Prestamo.FechaDevolucion.HasValue ? g.Prestamo.FechaDevolucion.Value.ToString("dd/MM/yyyy") : "";
                g.Estado = g.Prestamo.Devuelto ? "Devuelto" : "Abierto";

                if (g.Prestamo==null) throw new EXNoEncontrado(PrestamoId);

                var r = await ServicioReporteEntidad.UnicoAsync(x => x.Id == "reporte-prestamo" && x.TipoOrigenId == Dominio && x.OrigenId == UnidadOrganizacinal);
                if (r == null) throw new EXNoEncontrado("reporte: reporte-prestamo");

                List<Activo> activos = await this.UDT.Context.ActivosPrestamo.Join(
                    this.UDT.Context.Activos,
                    activospretamo => activospretamo.ActivoId,
                    activo => activo.Id,
                    (activospretamo, activo) => activo).OrderBy(n=>n.Nombre).ToListAsync();

                int indice = 1;
                activos.ForEach(x => {
                    g.Activos.Add(x.CopiaActivoPrestamo(indice));
                    indice++;
                });
                

                string jsonString = System.Text.Json.JsonSerializer.Serialize(g);
                byte[] data = Convert.FromBase64String(r.Plantilla);
                return ReporteEntidades.ReportePlantilla(data, jsonString, Config.ruta_cache_fisico, true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }


        }



        // ---------------------------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------------------------



    }
}
