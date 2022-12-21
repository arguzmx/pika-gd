using LazyCache;
using Microsoft.EntityFrameworkCore;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{

    public class SeguridadGestionDocumental : SevicioAuditableBase
    {
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public SeguridadGestionDocumental(
            string APP_ID, string MODULO_ID,
            UsuarioAPI usuario,
            ContextoRegistroActividad RegistroActividad,
            List<EventoAuditoriaActivo> EventosActivos,
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            UnidadDeTrabajo<DBContextGestionDocumental> UDT,
            Dictionary<string, string> Tablas) :
            base(
                APP_ID, MODULO_ID,
                usuario, RegistroActividad, EventosActivos, registroAuditoria,
                cache, Tablas, UDT.Context)
        {
            this.UDT = UDT;
        }

        public enum EventosAdicionales { 
            EntregarPrestamo =100, DevolverPrestamo = 101
        }

        public async Task<bool> AccesoCacheArchivo(string ArchivoId)
        {
            var cached = await cache.GetAsync<List<string>>($"archivo-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(ArchivoId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{ArchivoId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.Archivos.FirstOrDefault(x => x.Id == ArchivoId);
                if (ca != null)
                {
                    if (usuario.Accesos.Any(x => x.OU.Equals(ca.OrigenId)))
                    {
                        cached.Add(ArchivoId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{ArchivoId}");
                    }
                }
                else
                {
                    cached.Add($"~{ArchivoId}");
                }
                cache.Add($"archivo-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


        public async Task<bool> AccesoCacheContenedorAlmacen(string ContenedorAlmacenId)
        {
            var ArchivosUsuario = await CreaCacheArchivos();
            var cached = await cache.GetAsync<List<string>>($"contenedoresAlmacen-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(ContenedorAlmacenId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{ContenedorAlmacenId}") >= 0)
                {
                    valid = false;
                } else
                {
                    search = true;
                }
            } else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.ContenedoresAlmacen.FirstOrDefault(x => x.Id == ContenedorAlmacenId);
                if (ca != null)
                {
                    if (ArchivosUsuario.Any(x => x.Equals(ca.ArchivoId)))
                    {
                        cached.Add(ContenedorAlmacenId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{ContenedorAlmacenId}");
                    }
                }
                else
                {
                    cached.Add($"~{ContenedorAlmacenId}");
                }
                cache.Add($"contenedoresAlmacen-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


        public async Task<bool> AccesoCacheAlmacenArchivo(string AlmacenArchivoId)
        {
            var ArchivosUsuario = await CreaCacheArchivos();

            var cached = await cache.GetAsync<List<string>>($"almacenArchivo-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(AlmacenArchivoId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{AlmacenArchivoId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.AlmacenesArchivo.FirstOrDefault(x => x.Id == AlmacenArchivoId);
                if (ca != null)
                {
                    if (ArchivosUsuario.Any(x => x.Equals(ca.ArchivoId)))
                    {
                        cached.Add(AlmacenArchivoId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{AlmacenArchivoId}");
                    }
                }
                else
                {
                    cached.Add($"~{AlmacenArchivoId}");
                }
                cache.Add($"almacenArchivo-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


        public async Task<bool> AccesoCacheTransferencia(string TransferenciaId)
        {
            var ArchivosUsuario = await CreaCacheArchivos();

            var cached = await cache.GetAsync<List<string>>($"transferencia-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(TransferenciaId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{TransferenciaId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.Transferencias.FirstOrDefault(x => x.Id == TransferenciaId);
                if (ca != null)
                {
                    if (ArchivosUsuario.Any(x => x.Equals(ca.ArchivoOrigenId)) || ArchivosUsuario.Any(x => x.Equals(ca.ArchivoDestinoId)))
                    {
                        cached.Add(TransferenciaId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{TransferenciaId}");
                    }
                }
                else
                {
                    cached.Add($"~{TransferenciaId}");
                }
                cache.Add($"transferencia-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


        public async Task<bool> AccesoCachePrestamo(string PrestamoId)
        {
            var ArchivosUsuario = await CreaCacheArchivos();

            var cached = await cache.GetAsync<List<string>>($"prestamo-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(PrestamoId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{PrestamoId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.Prestamos.FirstOrDefault(x => x.Id == PrestamoId);
                if (ca != null)
                {
                    if (ArchivosUsuario.Any(x => x.Equals(ca.ArchivoId)))
                    {
                        cached.Add(PrestamoId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{PrestamoId}");
                    }
                }
                else
                {
                    cached.Add($"~{PrestamoId}");
                }
                cache.Add($"prestamo-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }

        public async Task<bool> AccesoCacheZonasAlmacen(string ZonaId)
        {
            var ArchivosUsuario = await CreaCacheArchivos();
            var cached = await cache.GetAsync<List<string>>($"zonasAlmacen-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(ZonaId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{ZonaId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.ZonasAlmacen.FirstOrDefault(x => x.Id == ZonaId);
                if (ca != null)
                {
                    if (ArchivosUsuario.Any(x => x.Equals(ca.ArchivoId)))
                    {
                        cached.Add(ZonaId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{ZonaId}");
                    }
                }
                else
                {
                    cached.Add($"~{ZonaId}");
                }
                cache.Add($"zonasAlmacen-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }


        public async Task<bool> AccesoCacheUnidadesAdministrativas(string UAId)
        {
            var cached = await cache.GetAsync<List<string>>($"uas-{usuario.Id}");
            bool valid = false;
            bool search = false;

            if (cached != null)
            {
                if (cached.IndexOf(UAId) >= 0)
                {
                    valid = true;
                }
                else if (cached.IndexOf($"~{UAId}") >= 0)
                {
                    valid = false;
                }
                else
                {
                    search = true;
                }
            }
            else
            {
                search = true;
                cached = new List<string>();
            }

            if (search)
            {
                var ca = UDT.Context.UnidadesAdministrativasArchivo.FirstOrDefault(x => x.Id == UAId);
                if (ca != null)
                {
                    if (usuario.Accesos.Any(x => x.OU == ca.OrigenId ))
                    {
                        cached.Add(UAId);
                        valid = true;
                    }
                    else
                    {
                        cached.Add($"~{UAId}");
                    }
                }
                else
                {
                    cached.Add($"~{UAId}");
                }
                cache.Add($"uas-{usuario.Id}", cached, DateTimeOffset.Now.AddMinutes(5));
            }

            return valid;
        }

        public async Task<bool> AccesoValidoArchivo(string ArchivoId, bool EmitirExcepcion = true)
        {
            if (!string.IsNullOrEmpty(ArchivoId))
            {
                var ArchivosUsuario = await CreaCacheArchivos();

                if (ArchivosUsuario.IndexOf(ArchivoId) < 0)
                {
                    if(!EmitirExcepcion)
                    {
                        return false;
                    }
                    await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                    throw new ExErrorDatosSesion();
                }
            }

            return true;
        }

        public async Task<bool> AccesoValidoUnidadAdministrativa(string UAId, bool EmitirExcepcion = true)
        {
            var uas =await CreaCacheUnidadesAdministrativas();
            if (!string.IsNullOrEmpty(UAId))
            {
                if (uas.IndexOf(UAId) < 0)
                {
                    if(!EmitirExcepcion)
                    {
                        return false;
                    }
                    await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                    throw new ExErrorDatosSesion();
                }
            }
            return true;
        }

        public async Task<bool> AccesoValidoPermisosArchivo(PermisosArchivo o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = "*";
            var ArchivosUsuario = await CreaCacheArchivos();

            // Verifica que la UO este en la lista de las del usuario
            if (ArchivosUsuario.IndexOf(o.ArchivoId) < 0)
            {
                if(!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }
            return true;
        }

        public async Task<bool> AccesoValidoPosicionAlmacen(PosicionAlmacen o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            var ArchivosUsuario = await CreaCacheArchivos();
            // Verifica que el dominio este en la lista de las del usuario
            if (!ArchivosUsuario.Any(x => x.Equals(o.ArchivoId)))
            {
                if(!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return true;
        }

        public async Task<bool> AccesoValidoActivoPrestamo(ActivoPrestamo o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = "*";
            var ArchivosUsuario = await CreaCacheArchivos();
            var okPrestamo = await AccesoCachePrestamo(o.PrestamoId);
            var activo = UDT.Context.Activos.FirstOrDefault(x => x.Id == o.ActivoId);

            if (!okPrestamo || !ArchivosUsuario.Contains(activo.ArchivoId))
            {
                if (!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return false;
        }

        public async Task<bool> AccesoValidoPrestamo(Prestamo o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Folio;
            var ArchivosUsuario = await CreaCacheArchivos();
            
            if (!ArchivosUsuario.Any(x => x.Equals(o.ArchivoId)))
            {
                if (!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return true;
        }


        public async Task<bool> AccesoValidoElementoClasificacion(ElementoClasificacion o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            var CuadrosUsuario = await CacheIdEntidadPorDominio<CuadroClasificacion>();

            // Verifica que el dominio este en la lista de las del usuario
            if (CuadrosUsuario.IndexOf(o.CuadroClasifiacionId) < 0)
            {
                if (!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return true;
        }


        public async Task<bool> AccesoValidoEntradaClasificacion(EntradaClasificacion o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            var CuadrosUsuario = await CacheIdEntidadPorDominio<CuadroClasificacion>();

            // Verifica que el dominio este en la lista de las del usuario
            if (CuadrosUsuario.IndexOf(o.CuadroClasifiacionId) < 0)
            {
                if (!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return true;
        }


        public async Task<bool> AccesoValidoZonaAlmacen(ZonaAlmacen o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            var ArchivosUsuario = await CreaCacheArchivos();
            // Verifica que el dominio este en la lista de las del usuario
            if (!ArchivosUsuario.Any(x => x.Equals(o.ArchivoId)))
            {
                if(!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }
            return true;
        }


        public async Task<bool> AccesoValidoArchivo(Archivo o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            if (!usuario.Accesos.Any(x => x.OU.Contains(o.OrigenId)))
            {
                if(!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }
            return true;
        }


        public async Task<List<string>> CreaCacheUnidadesAdministrativas()
        {
            var unidades = new List<string>();
            if (usuario.Accesos.Count > 0)
            {
                unidades = await cache.GetAsync<List<string>>($"uaaUsuario-{usuario.Id}");
                if (unidades == null)
                {
                    string sqls = $@"select distinct ua.*  from gd$unidadadministrativaarchivo ua
inner join gd$permunidadadministrativa a on ua.Id = a.UnidadAdministrativaArchivoId
inner join org$usuarios_rol  r on r.RolId = a.DestinatarioId
inner join aspnetusers u on r.ApplicationUserId = u.Id
where u.Id = '{usuario.Id}' and ua.OrigenId in ({ASQLList(usuario.Accesos.Select(a => a.OU).ToList())})";

                    List<UnidadAdministrativaArchivo> l = UDT.Context.UnidadesAdministrativasArchivo.FromSqlRaw(sqls).ToList();

                    if (l.Count > 0)
                    {
                        unidades = l.Select(x => x.Id).ToList();
                        cache.Add($"uaaUsuario-{usuario.Id}", unidades, DateTimeOffset.Now.AddMinutes(5));
                    }
                }
            }

            return unidades == null ? new List<string>() : unidades;
        }


        public async Task<List<string>> CreaCacheArchivos()
        {
            List<string> ArchivosUsuario = new List<string>();
            if (usuario.Accesos.Count > 0)
            {
                ArchivosUsuario = await cache.GetAsync<List<string>>($"archivosUsuario-{usuario.Id}");
                if (ArchivosUsuario == null)
                {
                    string sqls = $@"SELECT DISTINCT a.*  FROM gd$archivo a 
INNER JOIN  gd$permisosrchivo pa ON a.Id = pa.ArchivoId  
INNER JOIN org$usuarios_rol  r ON r.RolId = pa.DestinatarioId
INNER JOIN aspnetusers u ON r.ApplicationUserId = u.Id
WHERE u.Id = '{usuario.Id}' AND a.OrigenId in ({ASQLList(usuario.Accesos.Select(a => a.OU).ToList())})";

                    List<Archivo> l = UDT.Context.Archivos.FromSqlRaw(sqls).ToList();

                    if (l.Count > 0)
                    {
                        ArchivosUsuario = l.Select(x => x.Id).ToList();
                        cache.Add($"archivosUsuario-{usuario.Id}", ArchivosUsuario, DateTimeOffset.Now.AddMinutes(5));
                    }
                }
            }

            return ArchivosUsuario == null ? new List<string>() : ArchivosUsuario;
        }



        public async Task<bool> AccesoValidoContenedorAlmacen(ContenedorAlmacen o, bool EmitirExcepcion = true)
        {
            var archivos = await CreaCacheArchivos();
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            if (!archivos.Any(x => x.Equals(o.ArchivoId)))
            {
                if (EmitirExcepcion)
                {
                    await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                    throw new ExErrorDatosSesion();

                }
                return false;
            }

            return true;
        }


        public async Task<bool> AccesoValidoActivo(Activo o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;

            var archivos = await CreaCacheArchivos();
            var cuadros =  await CacheIdEntidadPorDominio<CuadroClasificacion>();
            var uas = await CreaCacheUnidadesAdministrativas();

            // Verifica que la UO este en la lista de las del usuario
            if (archivos.IndexOf(o.ArchivoId) < 0 ||
                cuadros.IndexOf(o.CuadroClasificacionId) < 0 ||
                uas.IndexOf(o.UnidadAdministrativaArchivoId) < 0)
            {
                if(!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return true;
        }


        public async Task<bool> AccesoValidoAlmacen(AlmacenArchivo o, bool EmitirExcepcion = true)
        {
            IdEntidad = o.Id;
            NombreEntidad = o.Nombre;
            var ArchivosUsuario = await CreaCacheArchivos();
            
            if (!ArchivosUsuario.Any(x => x.Equals(o.ArchivoId)))
            {
                if(!EmitirExcepcion)
                {
                    return false;
                }
                await RegistraEvento((int)EventosComunesAuditables.Leer, false, null, (int)CodigoComunesFalla.DatosSesionIncorrectos);
                throw new ExErrorDatosSesion();
            }

            return true;
        }

    }
}
