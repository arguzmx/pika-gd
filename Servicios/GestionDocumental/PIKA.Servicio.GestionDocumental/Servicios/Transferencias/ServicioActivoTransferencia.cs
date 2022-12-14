
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioActivoTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioActivoTransferencia
    {
        private const string DEFAULT_SORT_COL = "ActivoId";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ActivoTransferencia> repo;
        private IRepositorioAsync<Transferencia> repoT;
        private IRepositorioAsync<Archivo> repoAr;
        private IRepositorioAsync<Activo> repoAct;


        public ServicioActivoTransferencia(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones, 
            ILogger<ServicioLog> Logger) 
            : base(registroAuditoria, proveedorOpciones, Logger,
              cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_TRANSFERENCIA)
        {
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<ActivoTransferencia>(new QueryComposer<ActivoTransferencia>());
            this.repoT = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.repoAr = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.repoAct = UDT.ObtenerRepositoryAsync<Activo>(new QueryComposer<Activo>());
        }



        public async Task<IPaginado<ActivoTransferencia>> ObtenerPaginadoAsync(string Texto, Consulta Query, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            Query = this.GetDefaultQuery(Query);

            var filtro = Query.Filtros.Where(f => f.Propiedad == "TransferenciaId").FirstOrDefault();
            string TransferenciaId = filtro != null ? filtro.Valor : "-";

            bool valido = await seguridad.AccesoCacheTransferencia(TransferenciaId);
            if (!valido)
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }


            Paginado<ActivoTransferencia> p = new Paginado<ActivoTransferencia>();
            string sqlbase = $"select act.* from {DBContextGestionDocumental.TablaActivosTransferencia} act inner join gd$activo a on act.ActivoId = a.Id " +
                $"where act.TransferenciaId = '{TransferenciaId}' " +
                $"AND CONCAT( COALESCE(act.Notas,''), COALESCE(A.Nombre,''), COALESCE(act.MotivoDeclinado,'') )  like '%{Texto}%'"; 

            string sqls = sqlbase
                + $"order by {Query.ord_columna} {Query.ord_direccion} "
                + $"limit {Query.indice * Query.tamano},{Query.tamano};";

            string sqlsCount = sqlbase.Replace("act.*", "count(*)");

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

            p.Elementos = await this.UDT.Context.ActivosTransferencia.FromSqlRaw(sqls, new object[] { }).ToListAsync();
            p.ConteoFiltrado = 0;
            p.ConteoTotal = conteo;

            p.Indice = Query.indice;
            p.Paginas = 0;
            p.Tamano = Query.tamano;
            p.Desde = Query.indice * Query.tamano;

            return p;
        }

        public async Task<bool> Existe(Expression<Func<ActivoTransferencia, bool>> predicado)
        {
            List<ActivoTransferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }



        public async Task EliminarVinculosTodos(string Id)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            var t = await this.UDT.Context.Transferencias.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (t == null)
            {
                throw new EXNoEncontrado();
            }

            if (!(ArchivosUsuario.Any(x => x.Equals(t.ArchivoOrigenId)) || ArchivosUsuario.Any(x => x.Equals(t.ArchivoDestinoId))))
            {
                await seguridad.EmiteDatosSesionIncorrectos(t.Id);
            }

            if (t.EstadoTransferenciaId != EstadoTransferencia.ESTADO_NUEVA)
            {
                throw new ExDatosNoValidos("APICODEACT-TX-TRANSFERENCIA-CERRADA");
            }

            List<string> activos = (await UDT.Context.ActivosTransferencia.Where(x => x.TransferenciaId == Id).ToListAsync()).Select(a => a.ActivoId).ToList();

            await this.UDT.Context.EliminaActivosEnTrasnferencia(activos);
            await this.UDT.Context.EstableceConteoActivosTrasnferencia(0, Id);
            await this.UDT.Context.ActualizaActivosEnTrasnferencia(activos, false);

            this.UDT.Context.AdicionaEventoTransferencia(t.Id, t.EstadoTransferenciaId, usuario.Id, "Todos los activos eliminados");
        }

        
        public async Task<ActivoTransferencia> CrearAsync(ActivoTransferencia entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            bool valido = await seguridad.AccesoCacheTransferencia(entity.TransferenciaId);
            if (!valido)
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            var activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == entity.ActivoId);
            if(activo == null)
            {
                throw new EXNoEncontrado(entity.ActivoId);
            }
            
            await seguridad.AccesoValidoActivo(activo);

            if (await Existe(x => x.ActivoId == entity.ActivoId && x.TransferenciaId ==entity.TransferenciaId))
            {
                throw new ExElementoExistente(entity.ActivoId);
            }

            var t = await this.UDT.Context.Transferencias.Where(x => x.Id == entity.TransferenciaId).FirstOrDefaultAsync();
            if(t==null)
            {
                throw new EXNoEncontrado();
            }

            if(t.EstadoTransferenciaId != EstadoTransferencia.ESTADO_NUEVA)
            {
                throw new ExDatosNoValidos("APICODEACT-TX-TRANSFERENCIA-CERRADA");
            }

            List<string> validos = await this.UDT.Context.ActivosValidosTransferencia( new List<string>() { entity.ActivoId }, t.RangoDias, t.ArchivoOrigenId);
            if(validos.Count>0)
            {

                var archivo = this.UDT.Context.Archivos.Where(x => x.Id == activo.ArchivoId).First();
                var tipo = TipoArchivoDeArchivo(archivo.Id);

                entity.Id = Guid.NewGuid().ToString();
                entity.UsuarioId = this.usuario.Id;
                entity.Declinado = false;
                entity.Aceptado = false;
                entity.EntradaClasificacionId = activo.EntradaClasificacionId;
                entity.CuadroClasificacionId = activo.CuadroClasificacionId;
                entity.FechaRetencion = (tipo.Id == TipoArchivo.IDARCHIVO_TRAMITE || tipo.Tipo == ArchivoTipo.tramite) ? activo.FechaRetencionAT.Value : activo.FechaRetencionAC.Value;
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();

                await this.UDT.Context.ActualizaConteoActivosTrasnferencia(1, entity.TransferenciaId);
                await this.UDT.Context.ActualizaActivosEnTrasnferencia(new List<string>() {  entity.ActivoId }, true);

                this.UDT.Context.AdicionaEventoTransferencia(t.Id, t.EstadoTransferenciaId, usuario.Id, "Activos adicionados");

                await seguridad.RegistraEventoCrear(entity.Id);

                return entity.Copia();
            } else
            {
                throw new ExErrorRelacional("APICODE-ACTIVOTRANSFERENCIA-DATOSINCORRECTOS");
            }


        }

        public async Task ActualizarAsync(ActivoTransferencia entity)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            bool valido = await seguridad.AccesoCacheTransferencia(entity.TransferenciaId);
            if (!valido)
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            var activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == entity.ActivoId);
            if (activo == null)
            {
                throw new EXNoEncontrado(entity.ActivoId);
            }

            await seguridad.AccesoValidoActivo(activo);

            var temp = await UnicoAsync(x => x.Id == entity.Id);
            if (temp==null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            string original = JsonConvert.SerializeObject(temp);

            var t = await this.UDT.Context.Transferencias.Where(x => x.Id == entity.TransferenciaId).FirstOrDefaultAsync();
            if (t == null)
            {
                throw new EXNoEncontrado();
            }
            
            if (t.EstadoTransferenciaId != EstadoTransferencia.ESTADO_NUEVA)
            {
                throw new ExDatosNoValidos("APICODEACT-TX-TRANSFERENCIA-CERRADA");
            }

            this.UDT.Context.AdicionaEventoTransferencia(t.Id, t.EstadoTransferenciaId, usuario.Id, "Actualizacion activo");
            temp.Notas = entity.Notas;
            UDT.Context.Entry(temp).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar( temp.Id, null, original.JsonDiff(JsonConvert.SerializeObject(temp)));

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

        public async Task<IPaginado<ActivoTransferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            List<ActivoTransferencia> listaEliminados = new List<ActivoTransferencia>();
            Transferencia t = null;
            foreach (var Id in ids)
            {
                ActivoTransferencia a = await this.repo.UnicoAsync(x => x.ActivoId == Id);
                t = await this.UDT.Context.Transferencias.Where(x => x.Id == a.TransferenciaId).FirstOrDefaultAsync();

                if (t == null)
                {
                    throw new EXNoEncontrado();
                }

                var valido = await seguridad.AccesoCacheTransferencia(t.Id);
                if (!valido)
                {
                    await seguridad.EmiteDatosSesionIncorrectos(t.Id);
                }

                if (t.EstadoTransferenciaId != EstadoTransferencia.ESTADO_NUEVA)
                {
                    throw new ExDatosNoValidos("APICODEACT-TX-TRANSFERENCIA-CERRADA");
                }

                if (a != null)
                {
                    
                    listaEliminados.Add(a);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach (var al in listaEliminados)
                {
                    await this.repo.Eliminar(al);
                    await seguridad.RegistraEventoEliminar(al.Id);
                }
                UDT.SaveChanges();
            }

            if(t!=null)
            {
                this.UDT.Context.AdicionaEventoTransferencia(t.Id, t.EstadoTransferenciaId, usuario.Id, "Activos eliminados");
            }
            return listaEliminados.Select(c=>c.Id).ToList();
        }

      
        public Task<List<ActivoTransferencia>> ObtenerAsync(Expression<Func<ActivoTransferencia, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<ActivoTransferencia>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
    
        public async Task<ActivoTransferencia> UnicoAsync(Expression<Func<ActivoTransferencia, bool>> predicado = null, Func<IQueryable<ActivoTransferencia>, IOrderedQueryable<ActivoTransferencia>> ordenarPor = null, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ActivoTransferencia t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }

        public async Task<ICollection<string>> EliminarActivoTransferencia(string[] ids)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            List<ActivoTransferencia> listaEliminados = new List<ActivoTransferencia>();
            ActivoTransferencia a;
            Transferencia t=null;
            int conteo = 0;
            string txId = "";
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    if( t==null || t.Id != a.TransferenciaId)
                    {
                        t = UDT.Context.Transferencias.FirstOrDefault(x => x.Id == a.TransferenciaId);
                    }

                    

                    if (t != null)
                    {
                        if (!(ArchivosUsuario.Any(x => x.Equals(t.ArchivoOrigenId)) || ArchivosUsuario.Any(x => x.Equals(t.ArchivoDestinoId))))
                        {
                            await seguridad.EmiteDatosSesionIncorrectos(t.Id);
                        }

                        if ((new List<string>() { EstadoTransferencia.ESTADO_NUEVA,
                            EstadoTransferencia.ESTADO_DECLINADA }).IndexOf(t.EstadoTransferenciaId) >= 0)
                        {
                            listaEliminados.Add(a);

                        } else
                        {
                            throw new ExDatosNoValidos("APICODE-TX-ESTADO-INVALIDO");
                        }
                        
                    }

                    conteo--;
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var at in listaEliminados)
                {
                    UDT.Context.Entry(at).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(at.Id);
                }
                UDT.SaveChanges();

                await this.UDT.Context.ActualizaActivosEnTrasnferencia(listaEliminados.Select(x => x.Id).ToList(), false);
                await this.UDT.Context.ActualizaConteoActivosTrasnferencia(conteo, txId);

            }
            return listaEliminados.Select(x => x.Id).ToList();
        }


        public async Task<RespuestaComandoWeb> ComandoWeb(string command, object payload)
        {
            seguridad.EstableceDatosProceso<ActivoTransferencia>();
            var ArchivosUsuario = await seguridad.CreaCacheArchivos();
            var r = new RespuestaComandoWeb() { Estatus = false, MensajeId = RespuestaComandoWeb.Novalido, Payload = null };


            dynamic d = JObject.Parse(System.Text.Json.JsonSerializer.Serialize(payload));
            JArray arr = d.Id;
            var ids = arr.ToObject<List<string>>();

            string sqls = @$"select t.* from {DBContextGestionDocumental.TablaActivosTransferencia} a
inner join {DBContextGestionDocumental.TablaActivos} t on a.ActivoId = t.Id
where t.EnTransferencia =1;";


            var activos = UDT.Context.Activos.FromSqlRaw(sqls).ToList();


            foreach(var t in activos)
            {
                if (!(ArchivosUsuario.Any(x => x.Equals(t.ArchivoId))))
                {
                    await seguridad.EmiteDatosSesionIncorrectos(t.Id);
                }
            }


            bool permisoArchivo = true;
            List<string> archivos = activos.Select(x => x.ArchivoId).Distinct().ToList();
            List<PermisosArchivo> permisos = this.contexto.PermisosArchivo(this.usuario.Id);

            archivos.ForEach(a =>
            {
                if (!permisos.Any(p => p.ArchivoId == a && p.RecibirTrasnferencia == true))
                {
                    permisoArchivo = false;
                }
            });

            if (!permisoArchivo)
            {
                if (!permisoArchivo)
                {
                    throw new ExDatosNoValidos("APICODE-TX-PERMISO-INVALIDO");
                }
            }

            switch (command)
            {
                case "aceptar-activos-tx":

                    sqls = @$"update {DBContextGestionDocumental.TablaActivosTransferencia} 
set Aceptado=1, FechaVoto=UTC_DATE(), UsuarioReceptorId='{usuario.Id}' where Id In ({ids.MergeSQLStringList()})";
                    await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                    r.MensajeId = $"{command}-ok";
                    r.Estatus = true;
                    break;


                case "declinar-activos-tx":
                    sqls = @$"update {DBContextGestionDocumental.TablaActivosTransferencia} 
set Declinado=1, FechaVoto=UTC_DATE(), UsuarioReceptorId='{usuario.Id}', MotivoDeclinado='{(string)d.data.motivo}' 
where Id In ({ids.MergeSQLStringList()})";
                    await UDT.Context.Database.ExecuteSqlRawAsync(sqls);


                    r.MensajeId = $"{command}-ok";
                    r.Estatus = true;
                    break;
            }

            return r;
        }



        #region Sin Implementar
        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public Task<IPaginado<ActivoTransferencia>> ObtenerPaginadoAsync(Expression<Func<ActivoTransferencia, bool>> predicate = null, Func<IQueryable<ActivoTransferencia>, IOrderedQueryable<ActivoTransferencia>> orderBy = null, Func<IQueryable<ActivoTransferencia>, IIncludableQueryable<ActivoTransferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
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


        public Task<IEnumerable<ActivoTransferencia>> CrearAsync(params ActivoTransferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ActivoTransferencia>> CrearAsync(IEnumerable<ActivoTransferencia> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task AdiconarVinculosLista(string Id, List<string> IdsVinculados)
        {
            throw new NotImplementedException();
        }

        public Task<ActivoTransferencia> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
