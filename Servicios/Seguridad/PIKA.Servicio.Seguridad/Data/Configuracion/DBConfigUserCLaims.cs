
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfigUserClaims : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaApplicationUserClaims);
            builder.HasKey(x => new  { x.Id });

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.UserId).HasMaxLength(255).IsRequired();
            builder.Property(x => x.ClaimType).HasColumnType("longtext");
            builder.Property(x => x.ClaimValue).HasColumnType("longtext");

            builder.HasOne(x => x.User).WithMany(x => x.Claims).HasForeignKey(z => z.UserId);

        }
    }
}
