using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
   public class DbConfCatalogoVinculado : IEntityTypeConfiguration<CatalogoVinculado>
    {
        public void Configure(EntityTypeBuilder<CatalogoVinculado> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaCatalogoVinculado);
            builder.HasKey(x => new { x.IdCatalogo, x.IdEntidad });

            builder.Property(x => x.IdCatalogo).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.IdCatalogoMap).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.IdEntidad).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.IdEntidadMap).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EntidadVinculo).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.PropiedadReceptora).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EntidadCatalogo).HasMaxLength(LongitudDatos.GUID).IsRequired();

        }
    }
}
