using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
   public class DbConfValidadorTexto : IEntityTypeConfiguration<ValidadorTexto>
    {
        public void Configure(EntityTypeBuilder<ValidadorTexto> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaValidadorTexto);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PropiedadId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.longmin).IsRequired();
            builder.Property(x => x.longmax).IsRequired();
            builder.Property(x => x.valordefaulr).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.regexp).IsRequired();

            builder.HasOne(x => x.PropiedadPlantilla).WithOne(y => y.ValTexto).HasForeignKey<ValidadorTexto>(z => z.Id);

        }
    }
}
