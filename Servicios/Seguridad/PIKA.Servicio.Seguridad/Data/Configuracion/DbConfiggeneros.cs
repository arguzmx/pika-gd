
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using RepositorioEntidades;
using System.Security.Cryptography.X509Certificates;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfiggeneros : IEntityTypeConfiguration<Genero>
    {
        public void Configure(EntityTypeBuilder<Genero> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaGeneros);
            builder.HasKey(x => new  { x.Id });

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

        }
    }
}
