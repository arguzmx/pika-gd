using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion.Data
{
    public class DbConfOUs :
       IEntityTypeConfiguration<UnidadOrganizacional>
    {
        public void Configure(EntityTypeBuilder<UnidadOrganizacional> builder)
        {


            builder.ToTable(DbContextOrganizacion.TablaOU);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.DominioId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Eliminada).IsRequired().HasDefaultValue(false);
            builder.HasIndex(x => x.DominioId);

        }
    }

}
