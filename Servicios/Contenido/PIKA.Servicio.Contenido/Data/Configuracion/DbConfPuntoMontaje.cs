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
    public class DbConfPuntoMontaje : IEntityTypeConfiguration<PuntoMontaje>
    {
        public void Configure(EntityTypeBuilder<PuntoMontaje> builder)
        {
            builder.ToTable(DbContextContenido.TablaPuntoMontaje);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.CreadorId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.VolumenDefaultId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Nombre).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();

            builder.HasMany(x => x.Elementos).WithOne(y => y.PuntoMontaje).HasForeignKey(z => z.PuntoMontajeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Carpetas).WithOne(y => y.PuntoMontaje).HasForeignKey(z => z.PuntoMontajeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.VolumenDefault).WithMany(y => y.PuntosMontaje).HasForeignKey(z => z.VolumenDefaultId);
            builder.HasMany(x => x.VolumenesPuntoMontaje).WithOne(y => y.PuntoMontaje).HasForeignKey(z => z.VolumenId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.PermisosPuntoMontaje).WithOne(y => y.PuntoMontaje).HasForeignKey(z => z.PuntoMontajeId).OnDelete(DeleteBehavior.Restrict);
            builder.Ignore(x => x.TipoOrigenDefault);
        }


    }

    public class DbConfPermisosPuntoMontaje : IEntityTypeConfiguration<PermisosPuntoMontaje>
    {
        public void Configure(EntityTypeBuilder<PermisosPuntoMontaje> builder)
        {
            builder.ToTable(DbContextContenido.TablaPermisosPuntoMontaje);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.PuntoMontajeId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.DestinatarioId).IsRequired().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Actualizar).IsRequired();
            builder.Property(x => x.Crear).IsRequired();
            builder.Property(x => x.Elminar).IsRequired();
            builder.Property(x => x.Leer).IsRequired();
            builder.Property(x => x.GestionContenido ).IsRequired();
            builder.Property(x => x.GestionMetadatos).IsRequired();
        }


    }
}
