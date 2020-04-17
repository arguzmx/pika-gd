using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfPlantilla : IEntityTypeConfiguration<Plantilla>
    {
        public void Configure(EntityTypeBuilder<Plantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaPlantilla);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x=>x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Dato001).HasDefaultValue(false).IsRequired();

            builder.HasMany(x => x.Asociaciones).WithOne(y => y.Plantilla).HasForeignKey(z => z.Id);

        }
    }
}
