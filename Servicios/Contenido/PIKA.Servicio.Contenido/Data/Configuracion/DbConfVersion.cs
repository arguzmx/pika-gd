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

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ElementoId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.CreadorId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Activa).IsRequired();
            builder.Property(x => x.Eliminada).IsRequired();
            builder.Property(x => x.ConteoPartes).IsRequired();
            builder.Property(x => x.MaxIndicePartes).IsRequired();
            builder.Property(x => x.TamanoBytes).IsRequired();

            builder.HasOne(x => x.Elemento).WithMany(y => y.Versiones).HasForeignKey(z => z.ElementoId);
            builder.HasMany(x => x.Partes).WithOne(y => y.Version).HasForeignKey(z => z.VersionId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
