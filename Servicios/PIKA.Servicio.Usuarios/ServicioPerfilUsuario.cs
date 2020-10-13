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

                foreach(var d in dominios)
                {
                    DominioActivo da = l.Where(x => x.Id == d.Id).First();
                    foreach(var ud in d.UnidadesOrganizacionales)
                    {
                        da.UnidadesOrganizacionales.Add(new UnidadOrganizacionalActiva()
                        {
                            DominioId = da.Id,
                            EsAdmin = da.EsAdmin,
                            Id = ud.Id,
                            Nombre = ud.Nombre
                        });
                    }
                }

            }
            else {
                var suscripciones = await repoUsuariosDominio.ObtenerAsync(x => x.ApplicationUserId == UsuarioId);
                foreach (var s in suscripciones) {
                    Dominio d = dominios.Where(x => x.Id == s.DominioId).SingleOrDefault();
                    if (d != null)
                    {

                        // Añade el dominio si no ha sido incluido
                        if (l.Where(x=>x.Id == d.Id).Count() == 0)
                        {
                            l.Add(new DominioActivo()
                            {
                                EsAdmin = s.EsAdmin,
                                Id = s.DominioId,
                                Nombre = d.Nombre
                            });
                        }

                        // verifica si la unidad organizacion ya esta incluida en el dominuio para ls multitenant
                        DominioActivo da = l.Where(x => x.Id == d.Id).First();
                        if (da.UnidadesOrganizacionales.Where(x => x.Id == s.UnidadOrganizacionalId).Count() == 0) {
                            UnidadOrganizacional ud = d.UnidadesOrganizacionales.Where(x => x.Id == s.UnidadOrganizacionalId).SingleOrDefault();
                            if (ud != null)
                            {
                                da.UnidadesOrganizacionales.Add(new UnidadOrganizacionalActiva()
                                {
                                    DominioId = da.Id,
                                    EsAdmin = da.EsAdmin,
                                    Id = ud.Id,
                                    Nombre = ud.Nombre
                                });
                            }
                        }
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
