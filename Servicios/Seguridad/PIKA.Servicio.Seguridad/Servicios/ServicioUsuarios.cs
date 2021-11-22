using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
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
        private UnidadDeTrabajo<DbContextSeguridad> UDT;

        public ServicioUsuarios(
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioUsuarios> Logger) :
            base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<PropiedadesUsuario>(new QueryComposer<PropiedadesUsuario>());
            this.repoAppUser = UDT.ObtenerRepositoryAsync<ApplicationUser>(new QueryComposer<ApplicationUser>());
            this.repoClaims = UDT.ObtenerRepositoryAsync<UserClaim>(new QueryComposer<UserClaim>());
            this.repoUsuarioDominio = UDT.ObtenerRepositoryAsync<UsuarioDominio>(new QueryComposer<UsuarioDominio>());
        }




        public async Task<int> ActutalizarContrasena(string UsuarioId, string Actual, string nueva)
        {
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

        public async Task<Boolean> EsAdmin(string dominioId, string UnidadOrgId, string Id) {

            var a = await this.repoAppUser.UnicoAsync(x => x.Id == Id);
            var u = await this.repoUsuarioDominio.UnicoAsync(x => x.DominioId == dominioId && x.UnidadOrganizacionalId == UnidadOrgId
            && x.ApplicationUserId == Id);

            if (a == null) a = new ApplicationUser() { GlobalAdmin = false };
            if (u == null) u = new UsuarioDominio() { EsAdmin = false };

            return a.GlobalAdmin || a.GlobalAdmin;
        }

        public async Task<PropiedadesUsuario> CrearAsync(string dominioId, string UnidadOrgId, PropiedadesUsuario entity, CancellationToken cancellationToken = default)
        {
            try
            {

                

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

                ApplicationUser tmp = await repoAppUser.UnicoAsync(
                    x => x.NormalizedUserName == entity.username.ToUpper() ||
                    x.NormalizedEmail == entity.username.ToUpper());
                if (tmp != null)
                {
                    throw new ExElementoExistente(entity.username);
                }

                IRepositorioAsync<UsuarioDominio> repoud = UDT.ObtenerRepositoryAsync<UsuarioDominio>(new QueryComposer<UsuarioDominio>());
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
                await this.repoAppUser.CrearAsync(tmp);
                UDT.SaveChanges();

                // Añade sus propiedades
                entity.UsuarioId = tmp.Id;
                entity.Inactiva = false;
                entity.Eliminada = false;
                await repo.CrearAsync(entity);
                UDT.SaveChanges();

                UsuarioDominio ud = new UsuarioDominio()
                {
                    ApplicationUserId = tmp.Id,
                    DominioId = dominioId,
                    UnidadOrganizacionalId = UnidadOrgId,
                    EsAdmin = false
                };
                await repoud.CrearAsync(ud);
                UDT.SaveChanges();

                // actualiza los claims
                await UpdateClaims(entity);

                return entity.Copia();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw ex;
            }
        }


        /// <summary>
        ///  Actializa los claims de OIDC en la base de datos
        /// </summary>
        /// <param name="p"></param>
        private async Task UpdateClaims(PropiedadesUsuario p)
        {
            List<UserClaim> claims = await this.repoClaims.ObtenerAsync(x => x.UserId == p.UsuarioId);

            string val = string.IsNullOrEmpty(p.name) ? "" : p.name;
            if (claims.Where(x => x.ClaimType == "name").Count() == 0)
            {
                await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "name").First().ClaimValue = val;
                UDT.Context.Entry(claims.Where(x => x.ClaimType == "name").First()).State = EntityState.Modified;
            }

            val = string.IsNullOrEmpty(p.family_name) ? "" : p.family_name;
            if (claims.Where(x => x.ClaimType == "family_name").Count() == 0)
            {
                await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "family_name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "family_name").First().ClaimValue = val;
                UDT.Context.Entry(claims.Where(x => x.ClaimType == "family_name").First()).State = EntityState.Modified;
            }

            val = string.IsNullOrEmpty(p.given_name) ? "" : p.given_name;
            if (claims.Where(x => x.ClaimType == "given_name").Count() == 0)
            {
                await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "given_name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "given_name").First().ClaimValue = val;
                UDT.Context.Entry(claims.Where(x => x.ClaimType == "given_name").First()).State = EntityState.Modified;
            }


            val = string.IsNullOrEmpty(p.middle_name) ? "" : p.middle_name;
            if (claims.Where(x => x.ClaimType == "middle_name").Count() == 0)
            {
                await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "middle_name", ClaimValue = val, UserId = p.UsuarioId });
            }
            else
            {
                claims.Where(x => x.ClaimType == "middle_name").First().ClaimValue = val;
                UDT.Context.Entry(claims.Where(x => x.ClaimType == "middle_name").First()).State = EntityState.Modified;
            }


            val = string.IsNullOrEmpty(p.nickname) ? "" : p.nickname;

            if (val != "")
                if (claims.Where(x => x.ClaimType == "nickname").Count() == 0)
                {
                    await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "nickname", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "nickname").First().ClaimValue = val;
                    UDT.Context.Entry(claims.Where(x => x.ClaimType == "nickname").First()).State = EntityState.Modified;
                }


            val = string.IsNullOrEmpty(p.picture) ? "" : p.picture;
            if (val != "")
                if (claims.Where(x => x.ClaimType == "picture").Count() == 0)
                {
                    await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "picture", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "picture").First().ClaimValue = val;
                    UDT.Context.Entry(claims.Where(x => x.ClaimType == "picture").First()).State = EntityState.Modified;
                }



            val = p.updated_at.HasValue ? p.updated_at.ToString() : "";
            if (val != "")
                if (claims.Where(x => x.ClaimType == "picture").Count() == 0)
                {
                    await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "picture", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "picture").First().ClaimValue = val;
                    UDT.Context.Entry(claims.Where(x => x.ClaimType == "picture").First()).State = EntityState.Modified;
                }

            val = p.email_verified.HasValue ? p.email_verified.ToString() : "false";
            if (val != "")
                if (claims.Where(x => x.ClaimType == "email_verified").Count() == 0)
                {
                    await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "email_verified", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "email_verified").First().ClaimValue = val;
                    UDT.Context.Entry(claims.Where(x => x.ClaimType == "email_verified").First()).State = EntityState.Modified;
                }


            val = string.IsNullOrEmpty(p.email) ? "" : p.email;
            if (val != "")
                if (claims.Where(x => x.ClaimType == "email").Count() == 0)
                {
                    await this.repoClaims.CrearAsync(new UserClaim() { ClaimType = "email", ClaimValue = val, UserId = p.UsuarioId });
                }
                else
                {
                    claims.Where(x => x.ClaimType == "email").First().ClaimValue = val;
                    UDT.Context.Entry(claims.Where(x => x.ClaimType == "email").First()).State = EntityState.Modified;
                }


            UDT.SaveChanges();

        }

        public async Task ActualizarAsync(PropiedadesUsuario entity)
        {
            try
            {
                ApplicationUser o = await this.repoAppUser.UnicoAsync(x => x.Id == entity.UsuarioId);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.UsuarioId);
                }

                if (!string.IsNullOrEmpty(entity.email))
                {

                    if (!ValidadorUsuario.IsValidEmail(entity.email))
                    {
                        throw new ExLoginInfoInvalida($"e:{entity.email}");
                    }

                    ApplicationUser m = await this.repoAppUser.UnicoAsync(x => x.Id != entity.UsuarioId
                        && x.NormalizedEmail == entity.email.ToUpper());

                    if (m != null)
                    {
                        throw new ExElementoExistente(entity.email);
                    }

                    if (entity.email != o.Email)
                    {
                        o.Email = entity.email;
                        o.NormalizedEmail = entity.email.ToUpper();
                        UDT.Context.Entry(o).State = EntityState.Modified;
                    }
                }



                PropiedadesUsuario p = await this.repo.UnicoAsync(x => x.UsuarioId == entity.UsuarioId);

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

                if (p != null)
                {
                    p.nickname = entity.nickname;
                    p.middle_name = entity.middle_name;
                    p.given_name = entity.given_name;
                    p.family_name = entity.family_name;
                    p.name = entity.name;
                    p.UsuarioId = entity.UsuarioId;
                    if(!string.IsNullOrEmpty(entity.generoid))
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


                    UDT.Context.Entry(p).State = EntityState.Modified;
                }
                else
                {

                    PropiedadesUsuario prop = new PropiedadesUsuario()
                    {
                        nickname = entity.nickname,
                        middle_name = entity.middle_name,
                        given_name = entity.given_name,
                        family_name = entity.family_name,
                        UsuarioId = entity.UsuarioId,
                        generoid = entity.generoid,
                        paisid = entity.password,
                        estadoid = entity.estadoid,
                        updated_at = DateTime.UtcNow,
                        name = entity.name
                    };

                    p.gmt = gmt;
                    if ((gmt != null))
                    {
                        p.gmt_offset = (float)gmt_offset;
                    }
                    else
                    {
                        p.gmt_offset = null;
                    }
                    // Añade sus propiedades
                    await repo.CrearAsync(prop);

                }

                UDT.SaveChanges();
                await UpdateClaims(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

        public async Task<IPaginado<PropiedadesUsuario>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PropiedadesUsuario>, IIncludableQueryable<PropiedadesUsuario, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, include);
            return respuesta;
        }


        public async Task<IPaginado<PropiedadesUsuario>> ObtenerPaginadoIdsAsync(List<string> ids, Consulta Query, Func<IQueryable<PropiedadesUsuario>, IIncludableQueryable<PropiedadesUsuario, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(x => ids.Contains(x.UsuarioId), Query, include);
            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            ApplicationUser o;
            PropiedadesUsuario p;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {

                p = await this.repo.UnicoAsync(x => x.UsuarioId == Id);
                if (p != null)
                {
                    p.Eliminada = true;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                }

                o = await this.repoAppUser.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    o.Eliminada = true;
                    UDT.Context.Entry(o).State = EntityState.Modified;
                    listaEliminados.Add(Id);
                }
            }
            UDT.SaveChanges();


            return listaEliminados;

        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
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
                    UDT.Context.Entry(p).State = EntityState.Modified;
                }

                o = await this.repoAppUser.UnicoAsync(x => x.Id == Id);
                if (o != null)
                {
                    o.Eliminada = false;
                    UDT.Context.Entry(o).State = EntityState.Modified;
                    listaEliminados.Add(Id);
                }
            }
            UDT.SaveChanges();


            return listaEliminados;
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
                }
            }

            Query = GetDefaultQuery(Query);
            //Console.WriteLine($"--->>{nombre}");

            //var resultados = await this.repo.ObtenerAsync(x => ((x.name ?? "") + (x.nickname ?? "") + (x.email ?? "") +
            //(x.family_name ?? "") + (x.given_name ?? "") + (x.username ?? "")).Contains(nombre));

            var usuarios = UDT.Context.PropiedadesUsuario.Where(x=>x.username.Contains(nombre, StringComparison.InvariantCultureIgnoreCase)).ToList();
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




        #endregion


    }
}
