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
            builder.HasKey(x =>x.Id);

            builder.Property(x => x.ElementoId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.VersionId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.VolumenId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Indice).IsRequired();
            builder.Property(x => x.ConsecutivoVolumen).IsRequired();
            builder.Property(x => x.TipoMime).HasMaxLength(LongitudDatos.MIME);
            builder.Property(x => x.Extension).HasMaxLength(LongitudDatos.MIME).IsRequired();
            builder.Property(x => x.LongitudBytes).IsRequired();
            builder.Property(x=>x.NombreOriginal).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();

            builder.Property(x => x.EsImagen).IsRequired();
            builder.Property(x => x.EsAudio).IsRequired();
            builder.Property(x => x.EsVideo).IsRequired();
            builder.Property(x => x.EsPDF).IsRequired();

            builder.Property(x => x.TieneMiniatura).IsRequired();
            builder.Property(x => x.Indexada).IsRequired();
            
            builder.HasIndex(x => x.Indexada);

            builder.HasOne(x => x.Elemento).WithMany(y => y.Partes).HasForeignKey(z => z.ElementoId);
            builder.HasOne(x => x.Version).WithMany(y => y.Partes).HasForeignKey(z => z.VersionId);

        }
    }

}
