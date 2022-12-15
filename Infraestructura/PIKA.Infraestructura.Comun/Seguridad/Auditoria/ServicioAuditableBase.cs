using LazyCache;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using PIKA.Infraestructura.Comun.Excepciones;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad.Auditoria
{
    public enum OrigenEntidad
    {
        dominio = 0, uo = 1
    }


    public abstract class SevicioAuditableBase
    {
        protected UsuarioAPI usuario;
        protected ContextoRegistroActividad RegistroActividad;
        protected List<EventoAuditoriaActivo> EventosActivos;
        protected IRegistroAuditoria registroAuditoria;
        protected IAppCache cache;
        protected string APP_ID;
        protected string MODULO_ID;
        protected Dictionary<string, string> Tablas;
        protected DbContext Context;
        protected StackFrame MarcoLLanada;
        protected string IdEntidad;
        protected string NombreEntidad;
        protected string TipoEntidad;

        public SevicioAuditableBase(string APP_ID, string MODULO_ID, UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad, List<EventoAuditoriaActivo> EventosActivos, 
            IRegistroAuditoria registroAuditoria, IAppCache cache, Dictionary<string, string> Tablas, DbContext context)
        {
            this.usuario = usuario;
            this.RegistroActividad = RegistroActividad;
            this.registroAuditoria = registroAuditoria;
            this.EventosActivos = EventosActivos;
            this.cache = cache;
            this.MODULO_ID = MODULO_ID;
            this.APP_ID = APP_ID;
            this.Tablas = Tablas;
            this.Context = context;
        }


        /// <summary>
        /// Establace el tipo de entidad y el marco de llamada actual
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void EstableceDatosProceso<T>()
        {
            Type t = typeof(T);
            this.TipoEntidad = t.Name;

            List<string> l = new List<string>() { "SetCallerMethod", "Start", "MoveNext", "InvokeAsync", "lambda_method", "ExecuteAsync", "InvokeNextActionFilterAsync", "InvokeNextActionFilterAwaitedAsync" };
            List<string> sources = new List<string>() { "PIKA.GD.API.Controllers", "PIKA.GD.API.Filters" };
            StackTrace st = new StackTrace(true);
            for (int i = 1; i < st.FrameCount; i++)
            {
                
                StackFrame sf = st.GetFrame(i);
                string assembly = sf.GetMethod().Module.Assembly.GetName().FullName;
                if(!assembly.StartsWith("System", StringComparison.InvariantCultureIgnoreCase) &&
                    !assembly.StartsWith("Microsoft", StringComparison.InvariantCultureIgnoreCase) &&
                    !assembly.StartsWith("Anonymously", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    string className = sf.GetMethod().DeclaringType.FullName;
                    if (!className.StartsWith("PIKA.GD.API.Controllers", StringComparison.InvariantCultureIgnoreCase) &&
                        !className.StartsWith("PIKA.GD.API.Filters", StringComparison.InvariantCultureIgnoreCase) &&
                        !className.StartsWith("PIKA.GD.API.Middlewares", StringComparison.InvariantCultureIgnoreCase))
                    {

                        if (l.IndexOf(sf.GetMethod().Name) < 0)
                        {
                            MarcoLLanada = sf;
                            break;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Determina si un Id de una entidad hijo esta en la lista de identificadores padres válido
        /// </summary>
        /// <typeparam name="Padre"></typeparam>
        /// <typeparam name="Hijo"></typeparam>
        /// <param name="Id"></param>
        /// <param name="OrigenPadre"></param>
        /// <returns></returns>
        /// <exception cref="ExErrorDatosSesion"></exception>
        public async Task PadreContieneId<Padre>(string Id, OrigenEntidad OrigenPadre, string Nombre =null)
        {
            this.IdEntidad = Id;
            this.NombreEntidad = Nombre;

            List<string> ids = new List<string>();

            switch (OrigenPadre)
            {
                case OrigenEntidad.uo:
                    ids = await CacheIdEntidadPorUOrg<Padre>();
                    break;

                case OrigenEntidad.dominio:
                    ids = await CacheIdEntidadPorDominio<Padre>();
                    break;
            }

            if (ids.IndexOf(Id) < 0)
            {
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }
        }


        public async Task<bool> IdEnUnidadOrg(string Id, string Nombre = null, bool GeneraExcepcion = true)
        {
            this.IdEntidad = Id;
            this.NombreEntidad = Nombre;

            if (!usuario.Accesos.Any(x => x.OU.Equals(Id)))
            {
                if (GeneraExcepcion)
                {
                    await RegistraEvento((int)EventosComunesAuditables.Leer,  false,  null,  (int)CodigoComunesFalla.DatosSesionIncorrectos);
                    throw new ExErrorDatosSesion();
                }
                return false;
            }

            return true;
        }


        public async Task<bool> IdEnDominio(string Id, string Nombre =null,  bool GeneraExcepcion = true)
        {
            this.IdEntidad = Id;
            this.NombreEntidad = Nombre;

            if (!usuario.Accesos.Any(x => x.Dominio.Equals(Id)))
            {
                if(GeneraExcepcion)
                {
                    await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                    throw new ExErrorDatosSesion();

                }
                return false;
            }

            return true;
        }


        /// <summary>
        /// Obtiene una lista de Ids que pertenecen a una unidad organizacional para la entidad T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="Tablas"></param>
        /// <param name="ColumnaId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<string>> CacheIdEntidadPorUOrg<T>(string ColumnaId = "Id")
        {

            Type t = typeof(T);
            string Tabla = Tablas.GetValueOrDefault(t.Name);
            if (string.IsNullOrEmpty(Tabla)) throw new Exception($"La entidad {t.Name} no esta en el diccionario");

            string key = $"uo-ids-{t.Name}-{usuario.Id}";

            List<string> ListaIds = null;
            ListaIds = await cache.GetAsync<List<string>>(key);

            if (ListaIds == null)
            {
                ListaIds = new List<string>();
                List<string> l = usuario.Accesos.Select(x => x.OU).ToList();
                string SqlDominios = ASQLList(l);

                if (l.Count > 0)
                {
                    string sqls = $"SELECT DISTINCT {ColumnaId} FROM {Tabla} WHERE OrigenId in ({SqlDominios})";
                    var connection = new MySqlConnection(Context.Database.GetDbConnection().ConnectionString);
                    await connection.OpenAsync();

                    MySqlCommand cmd = new MySqlCommand(sqls, connection);
                    DbDataReader dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        var s = dr.GetString(0);
                        if (!string.IsNullOrEmpty(s))
                        {
                            ListaIds.Add(s);
                        }
                    }
                    dr.Close();
                    await connection.CloseAsync();

                    cache.Add(key, ListaIds, DateTimeOffset.Now.AddMinutes(5));
                }
            }
            return ListaIds;
        }

        /// <summary>
        /// Obtiene una lista de Ids que pertenecen a un dominio para la entidad T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="Tablas"></param>
        /// <param name="ColumnaId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<string>> CacheIdEntidadPorDominio<T>(string ColumnaId = "Id")
        {

            Type t = typeof(T);
            string Tabla = Tablas.GetValueOrDefault(t.Name);
            if (string.IsNullOrEmpty(Tabla)) throw new Exception($"La entidad {t.Name} no esta en el diccionario");

            string key = $"d-ids-{t.Name}-{usuario.Id}";

            List<string> ListaIds = null;
            ListaIds = await cache.GetAsync<List<string>>(key);

            if (ListaIds == null)
            {
                ListaIds = new List<string>();
                List<string> l = usuario.Accesos.Select(x => x.Dominio).ToList();
                string SqlDominios = ASQLList(l);

                if (l.Count > 0)
                {
                    var cs = Context.Database.GetDbConnection().ConnectionString;
                    string sqls = $"SELECT DISTINCT {ColumnaId} FROM {Tabla} WHERE OrigenId in ({SqlDominios})";
                    var connection = new MySqlConnection(Context.Database.GetDbConnection().ConnectionString);
                    await connection.OpenAsync();

                    MySqlCommand cmd = new MySqlCommand(sqls, connection);
                    DbDataReader dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        var s = dr.GetString(0);
                        if (!string.IsNullOrEmpty(s))
                        {
                            ListaIds.Add(s);
                        }
                    }
                    dr.Close();
                    await connection.CloseAsync();

                    cache.Add(key, ListaIds, DateTimeOffset.Now.AddMinutes(5));
                }
            }
            return ListaIds;
        }


        /// <summary>
        /// Convierte una listas de strings a una lista de SQL para en oeprador IN
        /// </summary>
        /// <param name="l"></param>
        /// <param name="Limitador"></param>
        /// <returns></returns>
        protected string ASQLList(List<string> l, string Limitador = "'")
        {
            string s = "";
            l.ForEach(i =>
            {
                s += $"{Limitador}{i}{Limitador},";
            });

            return s.TrimEnd(',');
        }

        public virtual async Task EmiteDatosSesionIncorrectos(string Id= null, string Nombre = null)
        {
            this.IdEntidad = Id;
            this.NombreEntidad = Nombre;
            await RegistraEvento((int)EventosComunesAuditables.Leer,false,null,(int)CodigoComunesFalla.DatosSesionIncorrectos);
            throw new ExErrorDatosSesion();
        }

        public virtual async Task<EventoAuditoria> RegistraEventoPurgar(string Id, string Nombre = null)
        {
            this.NombreEntidad = Nombre;
            this.IdEntidad = Id;
            return await RegistraEvento((int)EventosComunesAuditables.Purgar);
        }

        public virtual async Task<EventoAuditoria> RegistraEventoEliminar(string Id, string Nombre = null)
        {
            this.NombreEntidad = Nombre;
            this.IdEntidad = Id;
            return await RegistraEvento((int)EventosComunesAuditables.Eliminar);
        }

        public virtual async Task<EventoAuditoria> RegistraEventoCrear(string Id, string Nombre = null)
        {
            this.NombreEntidad = Nombre;
            this.IdEntidad = Id;
            return await RegistraEvento((int)EventosComunesAuditables.Crear);
        }

        public virtual async Task<EventoAuditoria> RegistraEventoActualizar(string Id, string Nombre = null , string delta = null)
        {
            // Si el delta es nulo no hubo cambios
            if (!string.IsNullOrEmpty(delta))
            {
                this.NombreEntidad = Nombre;
                this.IdEntidad = Id;
                return await RegistraEvento((int)EventosComunesAuditables.Actualizar,  true, delta);

            }
            return null;
        }


        public virtual async Task<EventoAuditoria> RegistraEvento(int tipoEvento, bool Exitoso = true, string Delta = null, int? TipoFalla = null)
        {

            if (!EventosActivos.Any(e => e.TipoEvento == tipoEvento && e.AppId == this.APP_ID && e.ModuloId == this.MODULO_ID))
            {
                return null;
            }

            string fuente = "";
            if (MarcoLLanada != null)
            {
                fuente = $"{MarcoLLanada.GetMethod().DeclaringType.Name}.{MarcoLLanada.GetMethod().Name}";
            }

            EventoAuditoria ev = new EventoAuditoria()
            {
                DireccionRed = RegistroActividad.DireccionInternet,
                DominioId = RegistroActividad.DominioId,
                Exitoso = Exitoso,
                IdSesion = RegistroActividad.IdConexion,
                ModuloId = MODULO_ID,
                TipoEvento = tipoEvento,
                UAId = RegistroActividad.UnidadOrgId,
                UsuarioId = usuario.Id,
                TipoFalla = TipoFalla,
                IdEntidad = IdEntidad,
                Delta = Delta,
                TipoEntidad = TipoEntidad,
                Id = DateTime.UtcNow.Ticks,
                AppId = APP_ID,
                Fuente = fuente,
                NombreEntidad = NombreEntidad
            };

            ev = await registroAuditoria.InsertaEvento(ev);
            return ev;
        }

    }
}
