using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
 public   class DbConfVersion : IEntityTypeConfiguration<Version>
    {
        public void Configure(EntityTypeBuilder<Version> builder)
        {
            builder.ToTable(DbContextContenido.TablaVersionElemento);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ElementoId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.FechaCreacion).HasDefaultValue(DateTime.Now).IsRequired();
            builder.Property(x => x.FechaActualizacion).HasDefaultValue(DateTime.Now).IsRequired();
            builder.Property(x => x.Activa).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();


        }
    }
}
