using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;

namespace PIKA.Servicio.Contacto.Data.Configuracion
{
    public class DbConfMedioContacto :
       IEntityTypeConfiguration<MedioContacto>
    {
        public void Configure(EntityTypeBuilder<MedioContacto> builder)
        {

            builder.ToTable(DbContextContacto.TablaMedioContacto);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired(true).HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.TipoOrigenId).IsRequired(true).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.OrigenId).IsRequired(true).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoMedioId).IsRequired(true).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoFuenteContactoId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Principal).IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.Activo).IsRequired(true).HasDefaultValue(true);
            builder.Property(x => x.Medio).IsRequired(true).HasMaxLength(500);
            builder.Property(x => x.Prefijo).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.Sufijo).IsRequired(false).HasMaxLength(100);
            builder.Property(x => x.Notas).IsRequired(false).HasMaxLength(500);

            builder.HasIndex(x => new { x.TipoOrigenId, x.OrigenId });

            builder.Ignore(x => x.TipoOrigenDefault);
        }
    }

}
