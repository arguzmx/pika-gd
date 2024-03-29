﻿
using DocumentFormat.OpenXml.Bibliography;
using FluentValidation.Validators;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IOTransferencia ioT;


        public ServicioTransferencia(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IOptions<ConfiguracionServidor> Config) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA)
        {

            this.ConfiguracionServidor = Config.Value;
            this.repo = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.ioT = new IOTransferencia(registroAuditoria, Logger, proveedorOpciones);
        }


        public Task<Transferencia> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        public async Task<RespuestaComandoWeb> ComandoWeb(string command, object payload)
        {
            seguridad.EstableceDatosProceso<Transferencia>();
            RespuestaComandoWeb r = new RespuestaComandoWeb() { Estatus = false, MensajeId = RespuestaComandoWeb.Novalido, Payload = null };
            int e = 0;
            dynamic d = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(payload));
            string id = (string)d.Id;
            Transferencia t = await this.repo.UnicoAsync(x => x.Id == id);
            seguridad.NombreEntidad = t.Nombre;
            seguridad.IdEntidad = t.Id;
            switch (command)
            {
                case "enviar-transferencia":
                    try
                    {

                        await this.EstadoTrasnferencia((string)d.Id, EstadoTransferencia.ESTADO_ESPERA_APROBACION);
                        r.MensajeId = $"{command}-ok";
                        r.Estatus = true;

                        e = AplicacionGestionDocumental.EventosAdicionales.EnviarTransferencia.GetHashCode();
                        await seguridad.RegistraEvento(e, true);

                    }
                    catch (Exception)
                    {
                        e = AplicacionGestionDocumental.EventosAdicionales.EnviarTransferencia.GetHashCode();
                        await seguridad.RegistraEvento(e, false);
                        throw;
                    }
                    break;

                case "aceptar-transferencia":
                    try
                    {
                        await this.EstadoTrasnferencia((string)d.Id, EstadoTransferencia.ESTADO_RECIBIDA);
                        r.MensajeId = $"{command}-ok";
                        r.Estatus = true;
                        e = AplicacionGestionDocumental.EventosAdicionales.RecibirTransferencia.GetHashCode();
                        await seguridad.RegistraEvento(e, true);
                    }
                    catch (Exception)
                    {
                        e = AplicacionGestionDocumental.EventosAdicionales.RecibirTransferencia.GetHashCode();
                        await seguridad.RegistraEvento(e, false);
                        throw;
                    }
                    break;

                case "declinar-transferencia":
                    try
                    {
                        await this.EstadoTrasnferencia((string)d.Id, EstadoTransferencia.ESTADO_DECLINADA);
                        r.MensajeId = $"{command}-ok";
                        r.Estatus = true;
                        e = AplicacionGestionDocumental.EventosAdicionales.DeclinarTrasnferencia.GetHashCode();
                        await seguridad.RegistraEvento(e, true);
                    }
                    catch (Exception)
                    {
                        e = AplicacionGestionDocumental.EventosAdicionales.DeclinarTrasnferencia.GetHashCode();
                        await seguridad.RegistraEvento(e, false);
                        throw;
                    }
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

        /// <summary>
        /// Obtiene una pagina de trasnferencias
           /// </summary>
        /// <param name="Texto"></param>
        /// <param name="Query"></param>
        /// <param name="include"></param>
        /// <param name="disableTracking"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(string Texto, Consulta Query, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Transferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();

            Query = this.GetDefaultQuery(Query);

            var filtro = Query.Filtros.Where(f => f.Propiedad == "ArchivoId").FirstOrDefault();
            string ArchivoId = filtro != null ? filtro.Valor : "-";

            if (!ArchivosUsuario.Contains(ArchivoId))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }


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
            seguridad.EstableceDatosProceso<Transferencia>();
            await VerificaDatosCreacion(entity, false);
            List<string> activos = (await this.UDT.Context.ActivosSeleccionados.Where(x => x.TemaId == TemaId).ToListAsync())
                .Select(a => a.Id).ToList();
            List<string> validos = await this.UDT.Context.ActivosValidosTransferencia(activos, entity.RangoDias, entity.ArchivoOrigenId, entity.CuadroClasificacionId, entity.EntradaClasificacionId);

            var archivo = this.UDT.Context.Archivos.Where(x => x.Id == entity.ArchivoOrigenId).First();
            var tipo = TipoArchivoDeArchivo(archivo.Id);

            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;
            entity.CantidadActivos = validos.Count;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            
            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            foreach (string id in validos)
            {
                var activo = this.UDT.Context.Activos.Where(x => x.Id == id).First();
                
                var r = (tipo.Id == TipoArchivo.IDARCHIVO_TRAMITE || tipo.Tipo == ArchivoTipo.tramite ) ? activo.FechaRetencionAT.Value : activo.FechaRetencionAC.Value;

                this.UDT.Context.ActivosTransferencia.Add(
                    new ActivoTransferencia()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ActivoId = id,
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


            if (EliminarTema && validos.Count > 0)
            {
                string sqls = $"delete from {DBContextGestionDocumental.TablaActivoSelecionado} where TemaId = '{TemaId}' " +
                    $"and Id in ({validos.MergeSQLStringList()})";
                await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                if (activos.Count == validos.Count)
                {
                    sqls = $"delete from {DBContextGestionDocumental.TablaTemasActivos} where Id='{TemaId}'";
                    await UDT.Context.Database.ExecuteSqlRawAsync(sqls);
                }
            }

            this.UDT.Context.AdicionaEventoTransferencia(entity.Id, EstadoTransferencia.ESTADO_NUEVA, usuario.Id);

            return entity.Copia();

        }



        private async Task VerificaDatosCreacion(Transferencia entity, bool update)
        {
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            if (!update && !UDT.Context.EstadosTransferencia.Any(x => x.Id.Equals(entity.EstadoTransferenciaId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExErrorRelacional(entity.EstadoTransferenciaId);
            }

            if (!UDT.Context.Archivos.Any(x => x.Id.Equals(entity.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExErrorRelacional(entity.ArchivoOrigenId);
            }

            if (!UDT.Context.Archivos.Any(x => x.Id.Equals(entity.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExErrorRelacional(entity.ArchivoDestinoId);
            }

            if(!ArchivosUsuario.Contains(entity.ArchivoOrigenId) || !ArchivosUsuario.Contains(entity.ArchivoDestinoId))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }
            
            if(update)
            {
                if (await Existe(x => x.Id != entity.Id && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
                { throw new ExElementoExistente(entity.Nombre); }
            }
            else
            {
                if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
                { throw new ExElementoExistente(entity.Nombre); }

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

        }


        public async Task<Transferencia> CrearAsync(Transferencia entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Transferencia>();
            await VerificaDatosCreacion(entity, false);
            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear( entity.Id, entity.Nombre);

            return entity.Copia();
        }

        public async Task ActualizarAsync(Transferencia entity)
        {
            seguridad.EstableceDatosProceso<Transferencia>();
            await VerificaDatosCreacion(entity, true);

            Transferencia o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            string original = o.Flat();

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
            if (reset)
            {
                o.CantidadActivos = 0;
            }

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id,  o.Nombre, original.JsonDiff(o.Flat()));
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
            seguridad.EstableceDatosProceso<Transferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();

            var qArchivo = Query.Filtros.First(p => p.Propiedad == "ArchivoOrigenId");
            string archivo = qArchivo.Valor;

            if (!ArchivosUsuario.Contains(archivo))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }
            
            var qRecibidas = Query.Filtros.FirstOrDefault(p => p.Propiedad == "filtro-recibidas");
            Query.Filtros.Remove(qArchivo);
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
                }
            }
            

            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null, filtros);
            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Transferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();

            List<Transferencia> listaEliminados = new List<Transferencia>();
            foreach (var Id in ids)
            {
                Transferencia tx = this.UDT.Context.Transferencias.FirstOrDefault(t => t.Id == Id);
                if(tx!=null)
                {

                    if (!(ArchivosUsuario.Contains(tx.ArchivoDestinoId) || ArchivosUsuario.Contains(tx.ArchivoDestinoId)))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos();
                    }

                    listaEliminados.Add(tx);

                    
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var tx in listaEliminados)
                {
                    string sqls;

                    // SI la trasnferencia tiene activos en proceso remover el estado
                    if ((new List<string>() { EstadoTransferencia.ESTADO_NUEVA,
                        EstadoTransferencia.ESTADO_ESPERA_APROBACION }).IndexOf(tx.EstadoTransferenciaId) >= 0)
                    {
                        sqls = @$"UPDATE {DBContextGestionDocumental.TablaActivos} SET EnTransferencia=0 WHERE 
Id IN (SELECT ActivoId FROM {DBContextGestionDocumental.TablaActivosTransferencia} WHERE  TransferenciaId='{tx.Id}')";
                        UDT.Context.Database.ExecuteSqlRaw(sqls);
                    }

                    sqls = $"DELETE FROM {DBContextGestionDocumental.TablaEventosTransferencia} WHERE TransferenciaId='{tx.Id}'";
                    UDT.Context.Database.ExecuteSqlRaw(sqls);

                    sqls = $"DELETE FROM {DBContextGestionDocumental.TablaActivosTransferencia} WHERE TransferenciaId='{tx.Id}'";
                    UDT.Context.Database.ExecuteSqlRaw(sqls);

                    sqls = $"DELETE FROM {DBContextGestionDocumental.TablaTransferencias} WHERE Id='{tx.Id}'";
                    UDT.Context.Database.ExecuteSqlRaw(sqls);

                    await seguridad.RegistraEventoEliminar(tx.Id, tx.Nombre);
                }
            }

            return listaEliminados.Select(x => x.Id).ToList(); ;
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
            seguridad.EstableceDatosProceso<Transferencia>();
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


        public async Task EstadoTrasnferencia(string TransferenciaId, string EstadoId)
        {
            seguridad.EstableceDatosProceso<Transferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            var tx = await this.UnicoAsync(t => t.Id.Equals(TransferenciaId, StringComparison.InvariantCultureIgnoreCase));
            if (tx == null)
            {
                throw new EXNoEncontrado();
            }

            if (!(ArchivosUsuario.Contains(tx.ArchivoDestinoId) || ArchivosUsuario.Contains(tx.ArchivoDestinoId)))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
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
                    List<string> totales = UDT.Context.ActivosTransferencia.Where(x => x.TransferenciaId == tx.Id).Select(x => x.ActivoId).ToList();


                    if (aceptados.Count == 0)
                    {
                        throw new ExDatosNoValidos("APICODE-TX-SIN-ACPTADOS");
                    }

                    if(aceptados.Count + declinados.Count != totales.Count)
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
