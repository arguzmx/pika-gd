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
    public class DbConfElemento : IEntityTypeConfiguration<Elemento>
    {
        public void Configure(EntityTypeBuilder<Elemento> builder)
        {
            builder.ToTable(DbContextContenido.TablaElemento);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.PuntoMontajeId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.CreadorId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.CarpetaId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PermisoId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.Versionado).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.VersionId).IsRequired();

            builder.HasOne(x => x.Permiso).WithMany(y => y.Elementos).HasForeignKey(z => z.PermisoId);
            builder.HasOne(x => x.Volumen).WithMany(y => y.Elementos).HasForeignKey(z => z.VolumenId);
        }


    }
}
