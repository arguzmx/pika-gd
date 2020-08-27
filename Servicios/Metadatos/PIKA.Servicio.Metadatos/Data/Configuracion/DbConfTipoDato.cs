using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
  public class DbConfTipoDato : IEntityTypeConfiguration<TipoDato>
    {
        public void Configure(EntityTypeBuilder<TipoDato> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaTipoDato);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasMany(x => x.PropiedadesPlantilla).WithOne(y => y.TipoDato).HasForeignKey(z => z.TipoDatoId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
