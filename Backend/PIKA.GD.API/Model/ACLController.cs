using Microsoft.AspNetCore.Mvc;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PIKA.GD.API
{
    public class ACLController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        protected string GetUserId()
        {
            if (!string.IsNullOrEmpty(this.UsuarioId)) return this.UsuarioId;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var valor = identity.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault();
            if (valor != null) return valor.Value;
            return "";
        }

        public string TenatId{ get; set; }
        public string UsuarioId { get; set; }
        public string DominioId { get; set; }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        protected List<FiltroConsulta> ObtieneFiltrosIdentidad() {
            List<FiltroConsulta> l = new List<FiltroConsulta>();
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "UsuarioId", Valor = UsuarioId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "TenatId", Valor = TenatId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "DominioId", Valor = DominioId });

            return l;
        }

    } 

}
