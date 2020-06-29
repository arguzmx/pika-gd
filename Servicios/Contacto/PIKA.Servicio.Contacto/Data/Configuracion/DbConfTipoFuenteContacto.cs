using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;

namespace PIKA.Servicio.Contacto.Data.Configuracion
{
    public class DbConfTipoFuenteContacto :
       IEntityTypeConfiguration<TipoFuenteContacto>
    {
        public void Configure(EntityTypeBuilder<TipoFuenteContacto> builder)
        {

            builder.ToTable(DbContextContacto.TablaTipoFuenteContacto);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasMany(x => x.MediosContacto).WithOne(y => y.TipoFuenteContacto).HasForeignKey(z => z.TipoFuenteContactoId);

        }
    }

}
