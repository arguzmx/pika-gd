
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Seguridad;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DBConfigApplicationUser : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaApplicationUser);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(255);
            builder.Property(x => x.UserName).HasMaxLength(256).IsRequired(false);
            builder.Property(x => x.NormalizedUserName).HasMaxLength(256).IsRequired(false);
            builder.Property(x => x.Email).HasMaxLength(256).IsRequired(false);
            builder.Property(x => x.NormalizedEmail).HasMaxLength(256).IsRequired(false);
            builder.Property(x => x.EmailConfirmed).IsRequired();
            builder.Property(x => x.PasswordHash).HasColumnType("longtext").IsRequired(false);
            builder.Property(x => x.SecurityStamp).HasColumnType("longtext").IsRequired(false);
            builder.Property(x => x.PhoneNumber).HasColumnType("longtext").IsRequired(false);
            builder.Property(x => x.PhoneNumberConfirmed).IsRequired();
            builder.Property(x => x.TwoFactorEnabled).IsRequired();
            builder.Property(x => x.LockoutEnd).IsRequired(false).HasColumnType("datetime(6)");
            builder.Property(x => x.LockoutEnabled).IsRequired();
            builder.Property(x => x.AccessFailedCount).IsRequired();
            builder.Property(x => x.Inactiva).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Eliminada).IsRequired().HasDefaultValue(false);

            builder.HasMany(x => x.UsuariosDominio).WithOne(y => y.ApplicationUser).HasForeignKey(z => z.ApplicationUserId);


        }
    }
}
