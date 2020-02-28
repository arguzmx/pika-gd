using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion.Data
{
    public class DbConfRoles :
       IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            

            builder.ToTable(DbContextOrganizacion.TablaRoles);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Descripcion).HasMaxLength(LongitudDatos.Descripcion).IsRequired(false);

            builder.Property(x => x.RolPadreId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);

            builder.HasOne(x => x.RolPadre).WithMany(y => y.SubRoles).HasForeignKey(z => z.RolPadreId);

            builder.HasIndex(x => new { x.TipoOrigenId, x.OrigenId });

        }
    }

}
