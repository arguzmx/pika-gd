using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion.Data
{
    public class DbConfPais :
       IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {

            builder.ToTable(DbContextOrganizacion.TablaPais);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Valor).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasMany(x => x.Estados).WithOne(y => y.Pais).HasForeignKey(z=>z.PaisId) ;

        }
    }

}
