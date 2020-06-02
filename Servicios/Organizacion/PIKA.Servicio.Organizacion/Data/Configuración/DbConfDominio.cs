using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion.Data
{
    public class DbConfDominio :
       IEntityTypeConfiguration<Dominio>
    {
        public void Configure(EntityTypeBuilder<Dominio> builder)
        {

            builder.ToTable(DbContextOrganizacion.TablaDominio);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasIndex(x => new { x.TipoOrigenId, x.OrigenId });

            builder.HasMany(x => x.UnidadesOrganizacionales).WithOne(y => y.Dominio).HasForeignKey(z => z.DominioId);

            builder.Ignore(x => x.TipoOrigenDefault);

        }
    }

}
