using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion.Data
{
    public class DbConfDpostales :
       IEntityTypeConfiguration<DireccionPostal>
    {
        public void Configure(EntityTypeBuilder<DireccionPostal> builder)
        {

            builder.ToTable(DbContextOrganizacion.TablaDireccionesPortales);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.Property(x => x.Calle).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            
            builder.Property(x => x.NoExterno).HasMaxLength(LongitudDatos.Nombre);

            builder.Property(x => x.NoInterno).HasMaxLength(LongitudDatos.Nombre);

            builder.Property(x => x.Colonia).HasMaxLength(LongitudDatos.Nombre);

            builder.Property(x => x.CP).HasMaxLength(LongitudDatos.CodigoPostal);
            
            builder.Property(x => x.Municipio).HasMaxLength(LongitudDatos.Nombre);

            builder.Property(x => x.EsDefault).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.EstadoId).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PaisId).HasMaxLength(LongitudDatos.GUID);

            builder.HasOne(x => x.Pais).WithMany(y => y.Direcciones).HasForeignKey(z => z.PaisId);
            builder.HasOne(x => x.Estado).WithMany(y => y.Direcciones).HasForeignKey(z => z.EstadoId);

            builder.HasIndex(x => new { x.TipoOrigenId, x.OrigenId });

            builder.Ignore(x => x.TipoOrigenDefault);

        }
    }

}
