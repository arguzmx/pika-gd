using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    

    public class ServicioTokenSeguridad : IServicioTokenSeguridad
    {

        //private readonly OrganizationDbContext cx;
        private readonly ConfiguracionServidor Config;
        private readonly ILogger<ServicioTokenSeguridad> Logger;
        //public ServicioTokenSeguridad(OrganizationDbContext cx, IOptions<ConfiguracionServidor> Config,
        public ServicioTokenSeguridad(IOptions<ConfiguracionServidor> Config,
            ILogger<ServicioTokenSeguridad> Logger)
        {
            //this.cx = cx;
            this.Config = Config.Value;
            this.Logger = Logger;
        }

        public async Task<SecurityToken> ObtenerToken(string UserId, string DomainId, string AppId)
        {
            SecurityToken Token = null;

            await Task.Delay(1);
            Token = new SecurityToken() { AppId = AppId, 
                Content = "demo", 
                CreatedOn = DateTime.UtcNow, 
                DomainId = DomainId, 
                HostId = "x", 
                TicketId = "1", 
                UserId = UserId };

            //if (Config.alamcenar_cache_seguridad)
            //{
            //    Token = await cx.SecurityTokens.Where(x => x.UserId == UserId && x.DomainId == DomainId && x.AppId == AppId).FirstOrDefaultAsync();
            //    if (Token != null) return Token;
            //}

            //List<AppDomainGrant> l = await ObtenerUsuasioAppGrants(UserId, DomainId, AppId);
            //if (l.Count > 0)
            //{
            //    Token = new SecurityToken()
            //    {
            //        CreatedOn = DateTime.UtcNow,
            //        HostId = "",
            //        UserId = UserId,
            //        AppId = AppId,
            //        DomainId = DomainId,
            //    };

            //    Token.SerializeGrants(l);

            //    if (Config.alamcenar_cache_seguridad)
            //    {
            //        await cx.SecurityTokens.AddAsync(Token);
            //        await cx.SaveChangesAsync();
            //    }


            //}


            return Token;
        }

        public async Task<List<AppDomainGrant>> ObtenerGrants(string UserId, string DomainId, string AppId)
        {
            return await ObtenerUsuasioAppGrants(UserId, DomainId, AppId);
        }


        private async Task<List<AppDomainGrant>> ObtenerUsuasioAppGrants(string UserId, string DomainId, string AppId)
        {



            //string sqls = $@"
            //    select distinct *  from (                
            //    select g.* from DomainSecurityGrants g 
            //    inner join (
            //    select SecurityGroupId  from SecurityGroupUsers where DomainId ='{DomainId}' and UserId='{UserId}'
            //    union
            //    select SecurityGroupId   from SecurityGroupWorkRoles sgw 
            //    inner join UserWorkRoles w on sgw.WorkRoleId=w.WorkRoleId and w.DomainId = '{DomainId}' and w.UserId='{UserId}'
            //    ) as Groups on g.GrantedId=Groups.SecurityGroupId and g.GrantedType='{SecurityConstants.TYPE_GROUP}'
            //    where g.DomainId='{DomainId}'  and g.AppId='{AppId}' 
            //    union 
            //    select g.* from DomainSecurityGrants g 
            //    where g.DomainId='{DomainId}'  and g.AppId='{AppId}'  and g.GrantedType='{SecurityConstants.TYPE_USER}' and g.GrantedId='{UserId}'
            //    ) as Grants
            //    ";

            //string sqls = "";
            //List <AppDomainGrant> l = await cx.DomainSecurityGrants.FromSqlInterpolated($"{sqls}").ToListAsync();

            //return l;

            await Task.Delay(1);
            return new List<AppDomainGrant>();
        }

        
    }
}
