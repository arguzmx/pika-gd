using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using PIKA.Servicio.Seguridad.Data.Configuracion;

namespace PIKA.Identity.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<ApplicationUser>(new DBConfigApplicationUser());
            builder.ApplyConfiguration<UserClaim>(new DbConfigUserClaims());
            builder.ApplyConfiguration<UsuarioDominio>(new DbConfigUsuariosDominio());
            builder.ApplyConfiguration<PropiedadesUsuario>(new DbConfPropiedadesUsuario());
        }
    }
}
