using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;

namespace PIKA.Servicio.Contacto.Data.Configuracion
{
    public class DbConfEstado :
       IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {

            builder.ToTable(DbContextContacto.TablaEstados);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.PaisId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasMany(x => x.Direcciones).WithOne(y => y.Estado).HasForeignKey(z => z.EstadoId);

        }
    }

}
