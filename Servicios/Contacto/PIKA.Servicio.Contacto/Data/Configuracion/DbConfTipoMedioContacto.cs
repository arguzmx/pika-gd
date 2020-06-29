using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;

namespace PIKA.Servicio.Contacto.Data.Configuracion
{
    public class DbConfTipoMedioContacto :
       IEntityTypeConfiguration<TipoMedio>
    {
        public void Configure(EntityTypeBuilder<TipoMedio> builder)
        {

            builder.ToTable(DbContextContacto.TablaTipoMedioContacto);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasMany(x => x.MediosContacto).WithOne(y => y.TipoMedio).HasForeignKey(z => z.TipoMedioId);

        }
    }

}
