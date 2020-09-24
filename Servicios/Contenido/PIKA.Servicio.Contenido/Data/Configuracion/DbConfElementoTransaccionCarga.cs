using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
    public class DbConfElementoTransaccionCarga : IEntityTypeConfiguration<ElementoTransaccionCarga>
    {
        public void Configure(EntityTypeBuilder<ElementoTransaccionCarga> builder)
        {
            builder.ToTable(DbContextContenido.TablaElementoTransaccionContenido);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TransaccionId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.PuntoMontajeId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ElementoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.VersionId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Indice).IsRequired();
            builder.Property(x => x.NombreOriginal).IsRequired().HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.TamanoBytes).IsRequired();
            builder.Property(x => x.FechaCarga).IsRequired();
            builder.Property(x => x.Procesado).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Error).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Info).IsRequired(false).HasMaxLength(LongitudDatos.Descripcion);

            builder.HasIndex(x => x.TransaccionId);
        }


    }
}
