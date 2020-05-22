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
            builder.HasIndex(x => x.PropiedadId);

            
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PropiedadId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.longmin).IsRequired();
            builder.Property(x => x.longmax).IsRequired();
            builder.Property(x => x.valordefault).HasMaxLength(512).IsRequired(false);
            builder.Property(x => x.regexp).HasMaxLength(LongitudDatos.RegExp).IsRequired(false);

        }
    }
}
