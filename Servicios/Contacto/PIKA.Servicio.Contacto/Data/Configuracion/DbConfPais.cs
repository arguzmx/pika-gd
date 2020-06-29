using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;

namespace PIKA.Servicio.Contacto.Data.Configuracion
{
    public class DbConfPais :
       IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {

            builder.ToTable(DbContextContacto.TablaPais);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasMany(x => x.Estados).WithOne(y => y.Pais).HasForeignKey(z=>z.PaisId) ;
            builder.HasMany(x => x.Direcciones).WithOne(y => y.Pais).HasForeignKey(z => z.PaisId);

        }
    }

}
