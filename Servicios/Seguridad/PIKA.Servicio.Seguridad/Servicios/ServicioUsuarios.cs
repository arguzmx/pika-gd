using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using PIKA.Modelo.Seguridad.Validadores;
using PIKA.Servicio.Seguridad.Interfaces;
using RepositorioEntidades;
using TimeZoneConverter;

namespace PIKA.Servicio.Seguridad.Servicios
{

    public class ServicioUsuarios : ContextoServicioSeguridad
        , IServicioInyectable, IServicioUsuarios

    {
        private const string DEFAULT_SORT_COL = "username";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<PropiedadesUsuario> repo;
        private IRepositorioAsync<ApplicationUser> repoAppUser;
        private IRepositorioAsync<UsuarioDominio> repoUsuarioDominio;
        private IRepositorioAsync<UserClaim> repoClaims;

        public ServicioUsuarios(
              IAppCache cache,
              IRegistroAuditoria registroAuditoria,
              IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
              ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_USUARIOS)
        {
            this.repo = UDT.ObtenerRepositoryAsync<PropiedadesUsuario>(new QueryComposer<PropiedadesUsuario>());
            this.repoAppUser = UDT.ObtenerRepositoryAsync<ApplicationUser>(new QueryComposer<ApplicationUser>());
            this.repoClaims = UDT.ObtenerRepositoryAsync<UserClaim>(new QueryComposer<UserClaim>());
            this.repoUsuarioDominio = UDT.ObtenerRepositoryAsync<UsuarioDominio>(new QueryComposer<UsuarioDominio>());
        }

        public async Task RegistroLogin(string Usaurio, bool Valido, string DireccionRed)
        {
            Type t = typeof(ApplicationUser);
            var u = UDT.Context.Usuarios.Where(x => x.UserName == Usaurio).FirstOrDefault();
            if(u!=null)
            {
                seguridad = new Seguridad(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_USUARIOS, null, null, null, this.registroAuditoria, this.cache, this.UDT, null);

                List<UsuarioDominio> l = UDT.Context.UsuariosDominio.Where(x => x.ApplicationUserId == u.Id).ToList();
                foreach(var d in l)
                {
                    
                    EventoAuditoria ev = new EventoAuditoria()
                    {
                        DireccionRed = DireccionRed,
                        DominioId = d.DominioId,
                        Exitoso = Valido,
                        IdSesion = "",
                        ModuloId = ConstantesAppSeguridad.MODULO_USUARIOS,
                        TipoEvento = AplicacionSeguridad.EventosAdicionales.AccesoAlSistema.GetHashCode(),
                        UAId = d.UnidadOrganizacionalId,
                        UsuarioId = "",
                        TipoFalla = null,
                        IdEntidad = Usaurio,
                        Delta = $"{Valido}",
                        TipoEntidad = t.Name,
                        Id = DateTime.UtcNow.Ticks,
                        AppId = ConstantesAppSeguridad.APP_ID,
                        Fuente = "Login",
                        NombreEntidad = Usaurio
                    };
                    await seguridad.RegistraEvento(ev);
                }
            }

           
        }



        public async Task<int> ActualizarContrasena(string UsuarioId, string nueva)
        {
            seguridad.EstableceDatosProceso<ApplicationUser>();

            bool valido = await seguridad.UsuarioEnDominio(UsuarioId, RegistroActividad.DominioId);
            if(!valido)
            {
                return 400;
            }
            
            var u = await this.UDT.Context.Usuarios.Where(x => x.Id == UsuarioId).FirstAsync();
            
            if (u != null)
            {
                PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
                if (ValidadorUsuario.ContrasenaValida(nueva, this.logger))
                {
                    u.PasswordHash = hasher.HashPassword(u, nueva);
                    u.SecurityStamp = Guid.NewGuid().ToString();
                    u.ConcurrencyStamp = Guid.NewGuid().ToString();
                    await this.UDT.Context.SaveChangesAsync();

                    seguridad.IdEntidad = u.Id;
                    seguridad.NombreEntidad = u.Email;
                    await seguridad.RegistraEvento(AplicacionSeguridad.EventosAdicionales.CambioContrasenaAdmin.GetHashCode());

                    return 200;

                }
                else
                {
                    return 400;
                }

            }
            return 404;
        }

        public async Task<int> ActutalizarContrasena(string UsuarioId, string Actual, string nueva)
        {
            seguridad.EstableceDatosProceso<ApplicationUser>();

            if (UsuarioId!= RegistroActividad.UsuarioId)
            {
                return 400;
            }

            bool valido = await seguridad.UsuarioEnDominio(UsuarioId, RegistroActividad.DominioId);
            if (!valido)
            {
                return 400;
            }

            var u = await this.UDT.Context.Usuarios.Where(x => x.Id == UsuarioId).FirstAsync();
            if (u != null)
            {
                PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
                var result = hasher.VerifyHashedPassword(u, u.PasswordHash, Actual);
                if(result == PasswordVerificationResult.Success)
                {
                    if (ValidadorUsuario.ContrasenaValida(nueva, this.logger))
                    {
                        u.PasswordHash = hasher.HashPassword(u, nueva);
                        u.SecurityStamp = Guid.NewGuid().ToString();
                        u.ConcurrencyStamp = Guid.NewGuid().ToString();
                        await this.UDT.Context.SaveChangesAsync();

                        seguridad.IdEntidad = u.Id;
                        seguridad.NombreEntidad = u.Email;
                        await seguridad.RegistraEvento(AplicacionSeguridad.EventosAdicionales.CambioContrasenaUsuario.GetHashCode());

                        return 200;
                    
                    } else
                    {
                        return 400;
                    }

                    
                } else
                {
                    return 409;
                }
            }
            return 404;
        }

        public async Task<bool> Existe(Expression<Func<PropiedadesUsuario, bool>> predicado)
        {
            List<PropiedadesUsuario> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<bool> EsAdmin(string dominioId, string UnidadOrgId, string Id) {

            var a = await this.repoAppUser.UnicoAsync(x => x.Id == Id);
            var u = await this.repoUsuarioDominio.UnicoAsync(x => x.DominioId == dominioId && x.UnidadOrganizacionalId == UnidadOrgId
            && x.ApplicationUserId == Id);

            if (a == null) a = new ApplicationUser() { GlobalAdmin = false };
            if (u == null) u = new UsuarioDominio() { EsAdmin = false };

            return a.GlobalAdmin || a.GlobalAdmin;
        }

        public async Task<PropiedadesUsuario> CrearAsync(string dominioId, string UnidadOrgId, PropiedadesUsuario entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PropiedadesUsuario>();
            if (this.RegistroActividad.DominioId != dominioId)
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }


            if (string.IsNullOrEmpty(dominioId) || string.IsNullOrEmpty(UnidadOrgId))
            {
                throw new ExDatosNoValidos("Dominio/OU");
            }

            if (!ValidadorUsuario.ContrasenaValida(entity.password, this.logger))
            {
                throw new ExLoginInfoInvalida($"p:{entity.password}");
            }

            if (!ValidadorUsuario.IsValidEmail(entity.username))
            {
                throw new ExLoginInfoInvalida($"e:{entity.username}");
            }

            if (!ValidadorUsuario.UsernameValido(entity.username))
            {
                throw new ExLoginInfoInvalida($"n:{entity.username}");
            }

            ApplicationUser tmp = await UDT.Context.Usuarios.FirstOrDefaultAsync(x => x.NormalizedUserName == entity.username.ToUpper() || x.NormalizedEmail == entity.username.ToUpper());
            if (tmp == null)
            {
                PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
                tmp = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = entity.username,
                    Email = entity.username,
                    AccessFailedCount = 0,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = false,
                    LockoutEnabled = true,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    NormalizedUserName = entity.username.ToUpper(),
                    NormalizedEmail = entity.username.ToUpper(),
                    Inactiva = false,
                    Eliminada = false
                };

                tmp.PasswordHash = hasher.HashPassword(tmp, entity.password);

                // Añade el suaurio
                UDT.Context.Usuarios.Add(tmp);

                // Añade sus propiedades
                entity.UsuarioId = tmp.Id;
                entity.Inactiva = false;
                entity.Eliminada = false;
                

                UsuarioDominio ud = new UsuarioDominio()
                {
                    ApplicationUserId = tmp.Id,
                    DominioId = dominioId,
                    UnidadOrganizacionalId = UnidadOrgId,
                    EsAdmin = false
                };
                UDT.Context.UsuariosDominio.Add(ud);
                UDT.SaveChanges();

                await UpdateClaims(entity);

            } else
            {

                entity.UsuarioId = tmp.Id;
                entity.Inactiva = false;
                entity.Eliminada = false;

                var u = UDT.Context.PropiedadesUsuario.FirstOrDefault(x=>x.UsuarioId == tmp.Id);

                if (u == null)
                {
                    UDT.Context.PropiedadesUsuario.Add(entity);
                    UDT.SaveChanges();
                }

                
                var d = await UDT.Context.UsuariosDominio.FirstOrDefaultAsync(x => x.ApplicationUserId == tmp.Id && x.DominioId == RegistroActividad.DominioId);
                if (d == null)
                {
                    UsuarioDominio ud = new UsuarioDominio()
                    {
                        ApplicationUserId = tmp.Id,
                        DominioId = dominioId,
                        UnidadOrganizacionalId = UnidadOrgId,
                        EsAdmin = false
                    };
                    UDT.Context.UsuariosDominio.Add(ud);
                    UDT.SaveChanges();
                }
            }

            await seguridad.RegistraEventoCrear(entity.UsuarioId, entity.email);

            return entity.Copia();

        }


        /// <summary>
        ///  Actializa los claims de OIDC en la base de datos
        /// </summary>
        /// <param name="p"></param>
        private async Task UpdateClaims(PropiedadesUsuario p)
        {
            List<UserClaim> claims = UDT.Context.ClaimsUsuario.Where(x => x.UserId == p.UsuarioId).ToList();

            string val = string.IsNullOrEmpty(p.name) ? "" : p.name;
            if (claims.Where(x => x.ClaimType == "name").Count() == 0)
            {
                UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "name").First().ClaimValue = val;
                UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "name").First());
            }

            val = string.IsNullOrEmpty(p.family_name) ? "" : p.family_name;
            if (claims.Where(x => x.ClaimType == "family_name").Count() == 0)
            {
                UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "family_name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "family_name").First().ClaimValue = val;
                UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "family_name").First());
            }

            val = string.IsNullOrEmpty(p.given_name) ? "" : p.given_name;
            if (claims.Where(x => x.ClaimType == "given_name").Count() == 0)
            {
                UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "given_name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "given_name").First().ClaimValue = val;
                UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "given_name").First());
            }


            val = string.IsNullOrEmpty(p.middle_name) ? "" : p.middle_name;
            if (claims.Where(x => x.ClaimType == "middle_name").Count() == 0)
            {
                UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "middle_name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "middle_name").First().ClaimValue = val;
                UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "middle_name").First());
            }


            val = string.IsNullOrEmpty(p.nickname) ? "" : p.nickname;

            if (val != "")
                if (claims.Where(x => x.ClaimType == "nickname").Count() == 0)
                {
                    UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "nickname", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "nickname").First().ClaimValue = val;
                    UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "nickname").First());
                }


            val = string.IsNullOrEmpty(p.picture) ? "" : p.picture;
            if (val != "")
                if (claims.Where(x => x.ClaimType == "picture").Count() == 0)
                {
                    UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "picture", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "picture").First().ClaimValue = val;
                    UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "picture").First());
                }



            val = p.updated_at.HasValue ? p.updated_at.ToString() : "";
            if (val != "")
                if (claims.Where(x => x.ClaimType == "picture").Count() == 0)
                {
                    UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "picture", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "picture").First().ClaimValue = val;
                    UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "picture").First());
                }

            val = p.email_verified.HasValue ? p.email_verified.ToString() : "false";
            if (val != "")
                if (claims.Where(x => x.ClaimType == "email_verified").Count() == 0)
                {
                    UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "email_verified", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "email_verified").First().ClaimValue = val;
                    UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "email_verified").First());
                }


            val = string.IsNullOrEmpty(p.email) ? "" : p.email;
            if (val != "")
                if (claims.Where(x => x.ClaimType == "email").Count() == 0)
                {
                    UDT.Context.ClaimsUsuario.Add(new UserClaim() { ClaimType = "email", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "email").First().ClaimValue = val;
                    UDT.Context.ClaimsUsuario.Update(claims.Where(x => x.ClaimType == "email").First());
                }

            UDT.SaveChanges();
        }

        public async Task ActualizarAsync(PropiedadesUsuario entity)
        {
            seguridad.EstableceDatosProceso<PropiedadesUsuario>();

            PropiedadesUsuario p = await UDT.Context.PropiedadesUsuario.FirstOrDefaultAsync(x => x.UsuarioId == entity.UsuarioId);

            if (p == null)
            {
                throw new EXNoEncontrado(entity.UsuarioId);
            }

            if (!await seguridad.UsuarioEnDominio(p.UsuarioId, RegistroActividad.DominioId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(entity.UsuarioId);
            }

                    
            string gmt = null;
            double gmt_offset = 0;

            if (!string.IsNullOrEmpty(entity.gmt))
            {
                gmt = entity.gmt;
                try
                {
                    TimeZoneInfo tz = TZConvert.GetTimeZoneInfo(entity.gmt);
                    TimeSpan offset = tz.GetUtcOffset(DateTime.UtcNow);
                    gmt_offset = tz.BaseUtcOffset.TotalMinutes;
                }
                catch (Exception)
                {
                }
            }

            string original = p.Flat();
            p.nickname = entity.nickname;
            p.middle_name = entity.middle_name;
            p.given_name = entity.given_name;
            p.family_name = entity.family_name;
            p.name = entity.name;

            if (!string.IsNullOrEmpty(entity.generoid))
            {
                p.generoid = entity.generoid;
            }
            if (!string.IsNullOrEmpty(entity.paisid))
            {
                p.paisid = entity.paisid;
            }

            if (!string.IsNullOrEmpty(entity.estadoid))
            {
                p.estadoid = entity.estadoid;
            }

            p.updated_at = DateTime.UtcNow;
            p.gmt = gmt;

            if ((gmt != null))
            {
                p.gmt_offset = (float)gmt_offset;
            }
            else
            {
                p.gmt_offset = null;
            }

            UDT.Context.Entry(p).State= EntityState.Modified;
            UDT.SaveChanges();

           // await UpdateClaims(entity);

            await seguridad.RegistraEventoActualizar(p.UsuarioId, p.email, original.JsonDiff(p.Flat()));
                        
            

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

        public async Task<IPaginado<PropiedadesUsuario>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PropiedadesUsuario>, IIncludableQueryable<PropiedadesUsuario, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {

            seguridad.EstableceDatosProceso<PropiedadesUsuario>();
            Query = GetDefaultQuery(Query);

            long offset = Query.indice == 0 ? 0 : ((Query.indice) * Query.tamano) - 1;
            string sqls = @$"SELECT COUNT(*)  FROM  {DbContextSeguridad.TablaPropiedadesUsuario} up INNER JOIN {DbContextSeguridad.TablaUsuariosOominio} ud ON up.UsuarioId = ud.ApplicationUserId WHERE ud.DominioId = '{RegistroActividad.DominioId}' ";
            string sqlsCount = "";
            int total = 0;
            

            if (Query.Filtros != null)
            {
                List<string> condiciones = MySQLQueryComposer.Condiciones<PropiedadesUsuario>(Query, "");
                foreach (string s in condiciones)
                {
                    sqls += $" and ({s})";
                }
            }

            // Consulta de conteo sin ordenamiento ni limites
            sqlsCount = sqls;
            sqls += $" order by {Query.ord_columna} {Query.ord_direccion} ";
            sqls += $" LIMIT {offset},{Query.tamano}";
            sqls = sqls.Replace("COUNT(*)", "DISTINCT up.*");

            using (var command = UDT.Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlsCount;
                UDT.Context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    if (result.Read())
                    {
                        total = result.GetInt32(0);
                    }
                }
            }

            Paginado<PropiedadesUsuario> respuesta = new Paginado<PropiedadesUsuario>()
            {
                ConteoFiltrado = total,
                ConteoTotal = total,
                Desde = Query.indice * Query.tamano,
                Indice = Query.indice,
                Tamano = Query.tamano,
                Paginas = (int)Math.Ceiling(total / (double)Query.tamano)
            };
            respuesta.Elementos = this.UDT.Context.PropiedadesUsuario.FromSqlRaw(sqls).ToList();

            return respuesta;
        }


        public async Task<IPaginado<PropiedadesUsuario>> ObtenerPaginadoIdsAsync(List<string> ids, Consulta Query, Func<IQueryable<PropiedadesUsuario>, IIncludableQueryable<PropiedadesUsuario, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PropiedadesUsuario>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(x => ids.Contains(x.UsuarioId), Query, include);
            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<PropiedadesUsuario>();
            List<PropiedadesUsuario> listaEliminados = new List<PropiedadesUsuario>();
            foreach (var Id in ids)
            {

                PropiedadesUsuario p = await this.UDT.Context.PropiedadesUsuario.FirstOrDefaultAsync(x => x.UsuarioId == Id);
                if (p != null)
                {

                    if(!await seguridad.UsuarioEnDominio(Id, RegistroActividad.DominioId))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(Id);
                    }

                    listaEliminados.Add(p);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var p in listaEliminados)
                {
                    List<UsuarioDominio> d = UDT.Context.UsuariosDominio.Where(x => x.ApplicationUserId == p.UsuarioId && x.DominioId == RegistroActividad.DominioId).ToList();
                    if (d.Count > 0)
                    {
                        UDT.Context.UsuariosDominio.RemoveRange(d);
                        UDT.SaveChanges();

                        await seguridad.RegistraEventoEliminar(p.UsuarioId, p.email);
                    }
                }
                UDT.SaveChanges();
            }
            return listaEliminados.Select(x=>x.UsuarioId).ToList();

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<string>> Activar(string[] ids)
        {
            ApplicationUser o;
            PropiedadesUsuario p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {

                p = await this.repo.UnicoAsync(x => x.UsuarioId == Id);
                if (p != null)
                {
                    p.Eliminada = false;
                    p.Inactiva = false;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                }

                o = await this.repoAppUser.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    o.Eliminada = false;
                    o.Inactiva = false;
                    UDT.Context.Entry(o).State = EntityState.Modified;
                    listaEliminados.Add(Id);
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        public async Task<ICollection<string>> Inactivar(string[] ids)
        {
            ApplicationUser o;
            PropiedadesUsuario p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {

                p = await this.repo.UnicoAsync(x => x.UsuarioId == Id);
                if (p != null)
                {
                    p.Inactiva = true;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                }

                o = await this.repoAppUser.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    o.Inactiva = true;
                    UDT.Context.Entry(o).State = EntityState.Modified;
                    listaEliminados.Add(Id);
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }


        public Task<List<PropiedadesUsuario>> ObtenerAsync(Expression<Func<PropiedadesUsuario, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }


        public async Task<PropiedadesUsuario> UnicoAsync(Expression<Func<PropiedadesUsuario, bool>> predicado = null, Func<IQueryable<PropiedadesUsuario>, IOrderedQueryable<PropiedadesUsuario>> ordenarPor = null, Func<IQueryable<PropiedadesUsuario>, IIncludableQueryable<PropiedadesUsuario, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            PropiedadesUsuario d = await this.repo.UnicoAsync(predicado);
            return d;
        }



        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            string nombre = "";
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    nombre = Query.Filtros[i].Valor;
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }

            Query = GetDefaultQuery(Query);


            string sqls = $@"select p.*   from seguridad$usuarioprops p
 inner join seguridad$usuariosdominio d on p.UsuarioId = d.ApplicationUserId
 where d.DominioId='{RegistroActividad.DominioId}' and 
concat(COALESCE(p.username,''), COALESCE(p.name,''), COALESCE(p.family_name,''), COALESCE(p.given_name,'')) like '{nombre}%'";

            var usuarios = UDT.Context.PropiedadesUsuario.FromSqlRaw(sqls);
            List<ValorListaOrdenada> l = usuarios.Select(x => new ValorListaOrdenada()
            {
                Id = x.UsuarioId,
                Indice = 0,
                Texto =  $"{(x.name ?? "")} {(x.given_name ?? "")} {(x.family_name ?? "")} [{x.username}]"
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.UsuarioId));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.UsuarioId,
                Indice = 0,
                Texto = $"{(x.name ?? "")} {(x.given_name ?? "")} {(x.family_name ?? "")} [{x.username}]"
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        #region sin implementar


        public async Task<PropiedadesUsuario> CrearAsync(PropiedadesUsuario entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<PropiedadesUsuario>> CrearAsync(params PropiedadesUsuario[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PropiedadesUsuario>> CrearAsync(IEnumerable<PropiedadesUsuario> entities, CancellationToken cancellationToken = default)
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


        public Task<List<PropiedadesUsuario>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }


        public async Task<IPaginado<PropiedadesUsuario>> ObtenerPaginadoAsync(Expression<Func<PropiedadesUsuario, bool>> predicate = null, Func<IQueryable<PropiedadesUsuario>, IOrderedQueryable<PropiedadesUsuario>> orderBy = null, Func<IQueryable<PropiedadesUsuario>, IIncludableQueryable<PropiedadesUsuario, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PropiedadesUsuario> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }



        #endregion


    }
}
