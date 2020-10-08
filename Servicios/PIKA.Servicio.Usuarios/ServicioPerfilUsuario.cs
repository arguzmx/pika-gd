using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Modelo.Organizacion;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using PIKA.Servicio.Organizacion;
using PIKA.Servicio.Seguridad;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Servicio.Usuarios
{
    public class ServicioPerfilUsuario: ContextoServicioUsuarios, IServicioPerfilUsuario
    {

        private IRepositorioAsync<UsuarioDominio> repoUsuariosDominio;
        private IRepositorioAsync<ApplicationUser> repoAppUser;
        private IRepositorioAsync<UnidadOrganizacional> repoUO;
        private UnidadDeTrabajo<DbContextSeguridad> UDT;
        private UnidadDeTrabajo<DbContextOrganizacion> UDTORG;
        private IRepositorioAsync<Dominio> repoDominio;

        public ServicioPerfilUsuario(
         IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpcionesOrg,
         IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones,
         ILogger<ServicioPerfilUsuario> Logger) :
            base(proveedorOpcionesOrg, proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextSeguridad>(contextoSeguridad);
            this.repoUsuariosDominio = UDT.ObtenerRepositoryAsync<UsuarioDominio>(new QueryComposer<UsuarioDominio>());
            this.repoAppUser = UDT.ObtenerRepositoryAsync<ApplicationUser>(new QueryComposer<ApplicationUser>());

            this.UDTORG = new UnidadDeTrabajo<DbContextOrganizacion>(contextoOrganizacion);
            this.repoDominio = UDTORG.ObtenerRepositoryAsync<Dominio>(new QueryComposer<Dominio>());
            this.repoUO = UDTORG.ObtenerRepositoryAsync<UnidadOrganizacional>(new QueryComposer<UnidadOrganizacional>());
        }


        public async Task<List<DominioActivo>> Dominios(string UsuarioId)
        {
            List<DominioActivo> l =  new List<DominioActivo>();
            List<Dominio> dominios = new List<Dominio>();

            var usr = await repoAppUser.UnicoAsync(x => x.Id == UsuarioId);
            if (usr == null) throw new  EXNoEncontrado(UsuarioId);
            dominios = await repoDominio.ObtenerAsync(x => x.Eliminada == false,null, 
                x => x.Include(y=> y.UnidadesOrganizacionales) );

            if (usr.GlobalAdmin)
            {
                l = dominios.Select(x => 
                new DominioActivo() { EsAdmin = true, Id = x.Id, Nombre = x.Nombre })
                    .ToList();

            }
            else {
                var suscripciones = await repoUsuariosDominio.ObtenerAsync(x => x.ApplicationUserId == UsuarioId);
                foreach (var s in suscripciones) {
                    Dominio d = dominios.Where(x => x.Id == s.OrigenId).SingleOrDefault();
                    if (d != null)
                    {
                        l.Add(new DominioActivo()
                        {
                            EsAdmin = s.EsAdmin,
                            Id = s.OrigenId,
                            Nombre = d.Nombre
                        });
                    }
                }
            }

            foreach(DominioActivo d in l)
            {
                Dominio tmp = dominios.Where(x => x.Id == d.Id).Single();
                if (tmp.UnidadesOrganizacionales != null)
                {
                    foreach(UnidadOrganizacional u in tmp.UnidadesOrganizacionales)
                    {
                        d.UnidadesOrganizacionales.Add(new UnidadOrganizacionalActiva()
                        {
                            DominioId = u.DominioId,
                            EsAdmin = d.EsAdmin,
                            Id = u.Id,
                            Nombre = u.Nombre
                        });
                    }
                }
            }

            return l;
        }

        public async Task<bool> EsAdmin(string UsuarioId, string DomainId)
        {
            var usr = await repoAppUser.UnicoAsync(x => x.Id == UsuarioId);
            return usr != null ? usr.GlobalAdmin : false;
        }
    }
}
