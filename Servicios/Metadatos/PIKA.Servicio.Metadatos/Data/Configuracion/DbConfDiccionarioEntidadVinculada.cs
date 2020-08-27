using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
  public class DbConfDiccionarioEntidadVinculada : IEntityTypeConfiguration<DiccionarioEntidadVinculada>
    {
        public void Configure(EntityTypeBuilder<DiccionarioEntidadVinculada> builder)
        {
            //builder.ToTable(DbContextMetadatos.TablaDiccionarioEntidadVinculada);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Enidad).HasMaxLength(LongitudDatos.Nombre).IsRequired();

        }
    }
}
