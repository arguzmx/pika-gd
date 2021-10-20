using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace PIKA.Identity.Server.Services
{
    using IdentityServer4;
    using PIKA.Modelo.Seguridad;
    using PIKA.Servicio.Usuarios;

    public class PikaProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServicioPerfilUsuario perfil;
        public PikaProfileService(UserManager<ApplicationUser> userManager,
            IServicioPerfilUsuario perfil,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            this.perfil = perfil;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
            

            List<string> roles = await perfil.ObtieneRoles(sub);
            roles.ForEach(r => {
                claims.Add(new Claim(JwtClaimTypes.Role, r));
            });

            if(await perfil.EsAdmin(sub,""))
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "adminglobal"));
            }

            var dominios = await perfil.Dominios(sub);
            dominios.ForEach(d =>
            {
                d.UnidadesOrganizacionales.ForEach(u =>
                {
                    var value = u.DominioId + '-' + u.Id + (d.EsAdmin ? "-admin" : "-user");
                    claims.Add(new Claim(JwtClaimTypes.Role, value));

                });
                
            });

            //    claims.Add(new Claim(JwtClaimTypes.Scope, "dataEventRecords"));

            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
