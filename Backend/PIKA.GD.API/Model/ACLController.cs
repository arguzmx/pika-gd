using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using PIKA.GD.API.Model;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
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

        /// <summary>
        ///  Estas propiedaes vienen del filtro AsyncACLActionFilter
        /// </summary>
        public string TenantId{ get; set; }
        public string UsuarioId { get; set; }
        public string DominioId { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Accesos { get; set; }
        public bool AdminGlobal { get; set; }
        public UsuarioAPI usuario { get; set; }
        public ContextoRegistroActividad contextoRegistro { get; set; }
        public List<string> Universos
        {
            get
            {

                List<string> us = new List<string>();

                Accesos.ForEach(a =>
                {
                    us.Add(a.Split('-')[0]);
                });
                return us;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {
            this.usuario = usuario;
            this.contextoRegistro = RegistroActividad;
            EmiteConfiguracionSeguridad(usuario, RegistroActividad);
        }

        [OpenApiIgnore]
        [NonAction]
        public virtual void EmiteConfiguracionSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {

        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        protected List<FiltroConsulta> ObtieneFiltrosIdentidad() {
            List<FiltroConsulta> l = new List<FiltroConsulta>();
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "UsuarioId", Valor = UsuarioId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "TenantId", Valor = TenantId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "DominioId", Valor = DominioId });

            return l;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        protected List<FiltroConsulta> ObtieneFiltrosIdentidadSinDominio()
        {
            List<FiltroConsulta> l = new List<FiltroConsulta>();
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "UsuarioId", Valor = UsuarioId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "TenantId", Valor = TenantId });

            return l;
        }

        protected FiltroConsulta FiltroDominio(string Propiedad)
        {
            return new FiltroConsulta() { Propiedad = Propiedad , Operador = FiltroConsulta.OP_EQ, Valor = DominioId };
        }

        protected FiltroConsulta FiltroUA(string Propiedad)
        {
            return new FiltroConsulta() { Propiedad = Propiedad, Operador = FiltroConsulta.OP_EQ, Valor = TenantId };
        }

    } 

}
