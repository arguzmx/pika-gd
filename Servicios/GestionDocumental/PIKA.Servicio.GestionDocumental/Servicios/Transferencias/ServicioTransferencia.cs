
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTransferencia
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Transferencia> repo;
        private IRepositorioAsync<EstadoTransferencia> repoET;
        private IRepositorioAsync<Archivo> repoA;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IOTransferencia ioT;
        private ILogger<ServicioCuadroClasificacion> LoggerCC;

        public UsuarioAPI usuario { get; set; }
        public PermisoAplicacion permisos { get; set; }

        public ServicioTransferencia(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IOptions<ConfiguracionServidor> Config) : base(proveedorOpciones, Logger)
        {
            this.ConfiguracionServidor = Config.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.repoET = UDT.ObtenerRepositoryAsync<EstadoTransferencia>(new QueryComposer<EstadoTransferencia>());
            this.repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.ioT = new IOTransferencia(Logger, proveedorOpciones);

        }

        public async Task<RespuestaComandoWeb> ComandoWeb(string command, object payload)
        {
            RespuestaComandoWeb r = new RespuestaComandoWeb() { Estatus = false, MensajeId = RespuestaComandoWeb.Novalido, Payload = null };

            dynamic d = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(payload));
            switch (command)
            {
                case "enviar-transferencia":
                    await this.EstadoTrasnferencia((string)d.Id, EstadoTransferencia.ESTADO_ESPERA_APROBACION);
                    r.MensajeId = $"{command}-ok";
                    r.Estatus = true;
                    break;

                case "aceptar-transferencia":
                    await this.EstadoTrasnferencia((string)d.Id, EstadoTransferencia.ESTADO_RECIBIDA);
                    r.MensajeId = $"{command}-ok";
                    r.Estatus = true;
                    break;

                case "declinar-transferencia":
                    await this.EstadoTrasnferencia((string)d.Id, EstadoTransferencia.ESTADO_DECLINADA);
                    r.MensajeId = $"{command}-ok";
                    r.Estatus = true;
                    break;

            }
            return r;
        }

        public async Task<bool> Existe(Expression<Func<Transferencia, bool>> predicado)
        {
            List<Transferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteET(Expression<Func<EstadoTransferencia, bool>> predicado)
        {
            List<EstadoTransferencia> l = await this.repoET.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteA(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repoA.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(string Texto, Consulta Query, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {

            Query = this.GetDefaultQuery(Query);

            var filtro = Query.Filtros.Where(f => f.Propiedad == "ArchivoId").FirstOrDefault();
            string ArchivoId = filtro != null ? filtro.Valor : "-";


            Paginado<Transferencia> p = new Paginado<Transferencia>();
            string sqlbase = $"SELECT t.*  FROM {DBContextGestionDocumental.TablaTransferencias} t " +
                $"WHERE t.ArchivoOrigenId = '{ArchivoId}' or t.ArchivoDestinoId= '{ArchivoId}' " +
                $"AND CONCAT(COALESCE(t.Nombre,''), COALESCE(t.Folio,'')) like '%{Texto}%' ";

            string sqls = sqlbase
                + $"order by {Query.ord_columna} {Query.ord_direccion} "
                + $"limit {Query.indice * Query.tamano},{Query.tamano};";

            string sqlsCount = sqlbase.Replace("t.*", "count(*)");

            var connection = new MySqlConnection(this.UDT.Context.Database.GetDbConnection().ConnectionString);
            await connection.OpenAsync();
            int conteo = 0;
            MySqlCommand cmd = new MySqlCommand(sqlsCount, connection);
            DbDataReader dr = await cmd.ExecuteReaderAsync();
            if (dr.Read())
            {
                conteo = dr.GetInt32(0);
            }
            dr.Close();
            await connection.CloseAsync();

            try
            {
                p.Elementos = await this.UDT.Context.Transferencias.FromSqlRaw(sqls, new object[] { }).ToListAsync();
                p.ConteoFiltrado = 0;
                p.ConteoTotal = conteo;

                p.Indice = Query.indice;
                p.Paginas = 0;
                p.Tamano = Query.tamano;
                p.Desde = Query.indice * Query.tamano;

                return p;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Transferencia> CrearDesdeTemaAsync(Transferencia entity, string TemaId, bool EliminarTema = false, CancellationToken cancellationToken = default)
        {

            try
            {

                await VerificaDatosCreacion(entity);
                List<string> activos = (await this.UDT.Context.ActivosSeleccionados.Where(x => x.TemaId == TemaId).ToListAsync())
                    .Select(a => a.Id).ToList();
                List<string> validos = await this.UDT.Context.ActivosValidosTransferencia(activos, entity.RangoDias, entity.ArchivoOrigenId, entity.CuadroClasificacionId, entity.EntradaClasificacionId);

                var archivo = this.UDT.Context.Archivos.Where(x => x.Id == entity.ArchivoOrigenId).First();

                entity.Nombre = entity.Nombre.Trim();
                entity.Id = System.Guid.NewGuid().ToString();
                entity.FechaCreacion = DateTime.UtcNow;
                entity.CantidadActivos = validos.Count;
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();



                foreach(string id in validos )
                {
                    var activo = this.UDT.Context.Activos.Where(x => x.Id == id).First();
                    var r = (archivo.TipoArchivoId == TipoArchivo.IDARCHIVO_TRAMITE) ? activo.FechaRetencionAT.Value : activo.FechaRetencionAC.Value;

                    this.UDT.Context.ActivosTransferencia.Add(
                        new ActivoTransferencia() { Id = Guid.NewGuid().ToString(), ActivoId = id, 
                            Declinado = false,
                            Aceptado = false,
                            CuadroClasificacionId = activo.CuadroClasificacionId,
                            EntradaClasificacionId = activo.EntradaClasificacionId,
                            TransferenciaId = entity.Id,
                            FechaRetencion = r,
                            UsuarioId = this.usuario.Id
                        }
                        );
                }
                this.UDT.SaveChanges();

                await this.UDT.Context.ActualizaActivosEnTrasnferencia(validos, true);


                if(EliminarTema && validos.Count >0)
                {
                    string sqls = $"delete from {DBContextGestionDocumental.TablaActivoSelecionado} where TemaId = '{TemaId}' " +
                        $"and Id in ({validos.MergeSQLStringList()})";
                    await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                    if(activos.Count == validos.Count)
                    {
                        sqls = $"delete from {DBContextGestionDocumental.TablaTemasActivos} where Id='{TemaId}'";
                        await UDT.Context.Database.ExecuteSqlRawAsync(sqls);
                    }
                }

                this.UDT.Context.AdicionaEventoTransferencia(entity.Id, EstadoTransferencia.ESTADO_NUEVA, usuario.Id);
                
                return entity.Copia();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

 

        private async Task VerificaDatosCreacion(Transferencia entity)
        {
            if (!await ExisteET(x => x.Id.Equals(entity.EstadoTransferenciaId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.EstadoTransferenciaId); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.ArchivoOrigenId); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.ArchivoDestinoId); }
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExElementoExistente(entity.Nombre); }

            if (entity.ArchivoOrigenId == entity.ArchivoDestinoId)
            {
                throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ODIDENTICO");
            }

            if (!string.IsNullOrEmpty(entity.CuadroClasificacionId) && !string.IsNullOrEmpty(entity.EntradaClasificacionId))
            {
                if (!this.UDT.Context.EntradaClasificacion.Any(x => x.CuadroClasifiacionId == entity.CuadroClasificacionId && x.Id == entity.EntradaClasificacionId))
                {
                    throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ERRORCCEC");
                }
            }

        }


        public async Task<Transferencia> CrearAsync(Transferencia entity, CancellationToken cancellationToken = default)
        {

            await VerificaDatosCreacion(entity);
            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }
        public async Task ActualizarAsync(Transferencia entity)
        {
            try
            {
                Transferencia o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.Id);
                }
                
                if (!await ExisteA(x => x.Id.Equals(entity.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)))
                { throw new ExErrorRelacional(entity.ArchivoOrigenId); }
                
                if (!await ExisteA(x => x.Id.Equals(entity.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase)))
                { throw new ExErrorRelacional(entity.ArchivoDestinoId); }
                
                if (await Existe(x => x.Id != entity.Id
                && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }

                if (entity.ArchivoOrigenId == entity.ArchivoDestinoId)
                {
                    throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ODIDENTICO");
                }

                if (!string.IsNullOrEmpty(entity.CuadroClasificacionId) && !string.IsNullOrEmpty(entity.EntradaClasificacionId))
                {
                    if (!this.UDT.Context.EntradaClasificacion.Any(x => x.CuadroClasifiacionId == entity.CuadroClasificacionId && x.Id == entity.EntradaClasificacionId))
                    {
                        throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ERRORCCEC");
                    }
                }

                bool reset = false;
                if (o.CuadroClasificacionId != entity.CuadroClasificacionId
                    || o.EntradaClasificacionId != entity.EntradaClasificacionId
                    || o.RangoDias != entity.RangoDias)
                {
                    var elementos = this.UDT.Context.ActivosTransferencia.Where(t => t.TransferenciaId == entity.Id).ToList();
                    if (elementos.Count > 0)
                    {
                        string ids = elementos.Select(x => x.Id).ToList().MergeSQLStringList();
                        string sqls = $"update {DBContextGestionDocumental.TablaActivos} set EnTransferencia=0 where Id in ({ids})";

                        UDT.Context.Database.ExecuteSqlRaw(sqls);

                        reset = true;
                        this.UDT.Context.ActivosTransferencia.RemoveRange(elementos);
                        this.UDT.Context.SaveChanges();
                    }
                }


                o.Nombre = entity.Nombre.Trim();
                o.Folio = entity.Folio;
                o.ArchivoDestinoId = entity.ArchivoDestinoId;
                o.EntradaClasificacionId = entity.EntradaClasificacionId;
                o.CuadroClasificacionId = entity.CuadroClasificacionId;
                o.RangoDias = entity.RangoDias;
                if(reset)
                {
                    o.CantidadActivos = 0;
                }

                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();

                this.UDT.Context.AdicionaEventoTransferencia(entity.Id, entity.EstadoTransferenciaId, usuario.Id, "Datos actualizados");

            }
            catch (Exception ex)
            {
                throw;
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
        public async Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {

            var qArchivo = Query.Filtros.First(p => p.Propiedad == "ArchivoOrigenId");
            var qRecibidas = Query.Filtros.FirstOrDefault(p => p.Propiedad == "Recibidas");
            Query.Filtros.Remove(qArchivo);
            string archivo = qArchivo.Valor;
            List<Expression<Func<Transferencia, bool>>> filtros = new List<Expression<Func<Transferencia, bool>>>();
            if(qRecibidas == null)
            {
                filtros.Add(x => x.ArchivoOrigenId == archivo || x.ArchivoDestinoId == archivo);

            } else
            {
                Query.Filtros.Remove(qRecibidas);
                if (qRecibidas.Valor.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    filtros.Add(x =>  x.ArchivoDestinoId == archivo);

                } else
                {
                    filtros.Add(x => x.ArchivoOrigenId == archivo);
                }
            }
            

            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null, filtros);
            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Transferencia a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                var tx = this.UDT.Context.Transferencias.FirstOrDefault(t => t.Id == Id);
                if(tx!=null)
                {
                    string sqls;

                    // SI la trasnferencia tiene activos en proceso remover el estado
                    if ((new List<string>() { EstadoTransferencia.ESTADO_NUEVA, 
                        EstadoTransferencia.ESTADO_ESPERA_APROBACION }).IndexOf(tx.EstadoTransferenciaId) >=0)
                    {
                        sqls = @$"UPDATE {DBContextGestionDocumental.TablaActivos} SET EnTransferencia=0 WHERE 
Id IN (SELECT ActivoId FROM {DBContextGestionDocumental.TablaActivosTransferencia} WHERE  TransferenciaId='{tx.Id}')";
                        UDT.Context.Database.ExecuteSqlRaw(sqls);
                    }

                    sqls = $"DELETE FROM {DBContextGestionDocumental.TablaEventosTransferencia} WHERE TransferenciaId='{Id}'";
                    UDT.Context.Database.ExecuteSqlRaw(sqls);

                    sqls = $"DELETE FROM {DBContextGestionDocumental.TablaActivosTransferencia} WHERE TransferenciaId='{Id}'";
                    UDT.Context.Database.ExecuteSqlRaw(sqls);

                    sqls = $"DELETE FROM {DBContextGestionDocumental.TablaTransferencias} WHERE Id='{Id}'";
                    UDT.Context.Database.ExecuteSqlRaw(sqls);

                    listaEliminados.Add(Id);
                }
            }
            return listaEliminados;
        }
        public Task<List<Transferencia>> ObtenerAsync(Expression<Func<Transferencia, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Transferencia>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public async Task<Transferencia> UnicoAsync(Expression<Func<Transferencia, bool>> predicado = null, Func<IQueryable<Transferencia>, IOrderedQueryable<Transferencia>> ordenarPor = null, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Transferencia t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }
        public async Task<byte[]> ReporteTransferencia(string TransferenciaId, string[] Columnas)
        {
            if (Columnas.Count() < 0)
                Columnas = "EntradaClasificacion.Clave,EntradaClasificacion.Nombre,Nombre,Asunto,FechaApertura,FechaCierre,CodigoOptico,CodigoElectronico,Reservado,Confidencial,Ampliado".Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();

            string f = await ioT.Obtenetdatos(TransferenciaId, Columnas, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
            byte[] b = File.ReadAllBytes(f);
            return b;
        }


        public async Task<string[]> EliminarRelaciones(List<Archivo> listaArchivos)
        {
            //ServicioEventoTransferencia set = new ServicioEventoTransferencia(this.proveedorOpciones, this.logger);
            //ServicioComentarioTransferencia cct = new ServicioComentarioTransferencia(this.proveedorOpciones,this.logger);
            //ServicioActivoTransferencia sat = new ServicioActivoTransferencia(this.proveedorOpciones, this.logger);

            //List<Transferencia> ListaTranferencia = await repo.ObtenerAsync(x => x.ArchivoOrigenId.Contains(listaArchivos.Select(x => x.Id).FirstOrDefault())).ConfigureAwait(false);
            //List<Transferencia> listaT =await repo.ObtenerAsync(x => x.ArchivoDestinoId.Contains(listaArchivos.Select(x => x.Id).FirstOrDefault())).ConfigureAwait(false);
            //ListaTranferencia.AddRange(listaT);
            //List<EventoTransferencia> ListaEvento = await set.ObtenerAsync(x=>x.TransferenciaId.Contains(ListaTranferencia.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
            //List<ComentarioTransferencia> ListaComentarioTransferencia = await cct.ObtenerAsync(x=>x.TransferenciaId.Contains(ListaTranferencia.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);

            //await set.Eliminar(IdsEliminados(ListaEvento.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            //await cct.Eliminar(IdsEliminados(ListaComentarioTransferencia.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            //await sat.EliminarActivosTransferencia().ConfigureAwait(false);


            return null;
        }

        private string[] IdsEliminados(string[] ids)
        {
            return ids;
        }

        #region Sin Implementar


        public Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(Expression<Func<Transferencia, bool>> predicate = null, Func<IQueryable<Transferencia>, IOrderedQueryable<Transferencia>> orderBy = null, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Transferencia>> CrearAsync(params Transferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transferencia>> CrearAsync(IEnumerable<Transferencia> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<Transferencia> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        public async Task EstadoTrasnferencia(string TransferenciaId, string EstadoId)
        {

            var tx = await this.UnicoAsync(t => t.Id.Equals(TransferenciaId, StringComparison.InvariantCultureIgnoreCase));
            if (tx == null)
            {
                throw new EXNoEncontrado();
            }

            List<PermisosArchivo> permisos;

            bool estadoValido = false;
            bool permisoValido = false;

            // Sólo puede ir a aprobada o declinda si esta en estado de espera
            if (EstadoId.Equals(EstadoTransferencia.ESTADO_RECIBIDA) || EstadoId.Equals(EstadoTransferencia.ESTADO_DECLINADA))
            {

                if (tx.EstadoTransferenciaId == EstadoTransferencia.ESTADO_ESPERA_APROBACION)
                {
                    estadoValido = true;
                    permisos = this.contexto.PermisosArchivo(this.usuario.Id, tx.ArchivoDestinoId);
                    if (permisos.Any(p => p.RecibirTrasnferencia))
                    {
                        permisoValido = true;
                    }
                }
            }


            // Sólo puede ir a espera si es nueva
            if (EstadoId.Equals(EstadoTransferencia.ESTADO_ESPERA_APROBACION))
            {

                if (tx.EstadoTransferenciaId == EstadoTransferencia.ESTADO_NUEVA)
                {
                    estadoValido = true;
                    permisos = this.contexto.PermisosArchivo(this.usuario.Id, tx.ArchivoOrigenId);
                    if (permisos.Any(p => p.EnviarTrasnferencia))
                    {
                        permisoValido = true;
                    }
                }
            }


            //// sólo se puede cancelar si es nueva o esta en revisión
            //if (EstadoId.Equals(EstadoTransferencia.ESTADO_CANCELADA))
            //{
            //    if (tx.EstadoTransferenciaId == EstadoTransferencia.ESTADO_ESPERA_APROBACION
            //        || tx.EstadoTransferenciaId == EstadoTransferencia.ESTADO_NUEVA)
            //    {
            //        estadoValido = true;
            //        permisos = this.contexto.PermisosArchivo(this.usuario.Id, tx.ArchivoOrigenId);
            //        if (permisos.Any(p => p.CancelarTrasnferencia))
            //        {
            //            permisoValido = true;
            //        }
            //    }
            //}

            if (!permisoValido)
            {
                throw new ExDatosNoValidos("APICODE-TX-PERMISO-INVALIDO");
            }

            if (!estadoValido)
            {
                throw new ExDatosNoValidos("APICODE-TX-ESTADO-INVALIDO");
            }

       
            tx.EstadoTransferenciaId = EstadoId;
            switch(EstadoId)
            {
                case EstadoTransferencia.ESTADO_DECLINADA:
                    // Todos los activos se desmarcan
                    string sqls = @$"UPDATE {DBContextGestionDocumental.TablaActivos} SET EnTransferencia=0 WHERE 
Id IN (SELECT ActivoId FROM {DBContextGestionDocumental.TablaActivosTransferencia} WHERE  TransferenciaId='{tx.Id}')";
                    await UDT.Context.Database.ExecuteSqlRawAsync(sqls);
                    await UDT.Context.ActualizaActivosEnTrasnferencia(false, tx.Id);
                    break;

                case EstadoTransferencia.ESTADO_RECIBIDA:
                    List<string> aceptados = UDT.Context.ActivosTransferencia.Where(x => x.TransferenciaId == tx.Id && x.Aceptado == true).Select(x => x.ActivoId).ToList();
                    List<string> declinados = UDT.Context.ActivosTransferencia.Where(x => x.TransferenciaId == tx.Id && x.Declinado == true).Select(x => x.ActivoId).ToList();

                    if(aceptados.Count == 0)
                    {
                        throw new ExDatosNoValidos("APICODE-TX-SIN-ACPTADOS");
                    }

                    if(aceptados.Count + declinados.Count != tx.CantidadActivos)
                    {
                        throw new ExDatosNoValidos("APICODE-TX-SIN-VALIDAR");
                    }

                    // los activos se desmarcan
                    await UDT.Context.ActualizaActivosEnTrasnferencia(false, tx.Id);

                    //Los aceptados se mueven al nuevo arvico
                    await UDT.Context.MueveActivosArchivo(aceptados, tx.ArchivoDestinoId);

                    tx.EstadoTransferenciaId = declinados.Count > 0 ? EstadoTransferencia.ESTADO_RECIBIDA_PARCIAL : EstadoTransferencia.ESTADO_RECIBIDA;
                    break;
            }

            UDT.Context.Entry(tx).State = EntityState.Modified;
            UDT.SaveChanges();

            this.UDT.Context.AdicionaEventoTransferencia(tx.Id, tx.EstadoTransferenciaId, usuario.Id);

        }




        #endregion



    }
}
