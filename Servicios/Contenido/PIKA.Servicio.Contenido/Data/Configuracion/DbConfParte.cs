using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
    public class DbConfParte : IEntityTypeConfiguration<Parte>
    {
        public void Configure(EntityTypeBuilder<Parte> builder)
        {
            builder.ToTable(DbContextContenido.TablaParte);
            builder.HasKey(x =>new { x.ElementoId,x.VersionId});

            builder.Property(x => x.ElementoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.VersionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x=>x.NombreOriginal).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x=>x.Indice).HasDefaultValue(0).IsRequired();
            builder.Property(x=>x.ConsecutivoVolumen).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.TipoMime).HasMaxLength(LongitudDatos.MIME).IsRequired();
            builder.Property(x=>x.LongitudBytes).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();

            builder.HasOne(x => x.Elemento).WithOne(y=>y.Parte);
            builder.HasOne(x=>x.Version).WithMany(y=>y.Partes).HasForeignKey(z=>z.VersionId);

        }
    }

}
