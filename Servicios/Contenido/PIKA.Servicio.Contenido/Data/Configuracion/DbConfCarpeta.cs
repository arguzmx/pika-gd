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
    public class DbConfCarpeta : IEntityTypeConfiguration<Carpeta>
    {
        public void Configure(EntityTypeBuilder<Carpeta> builder)
        {
            builder.ToTable(DbContextContenido.TablaCarpeta);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.CreadorId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Nombre).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();
            builder.Property(x => x.Eliminada).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.EsRaiz).IsRequired();
            builder.Property(x => x.CarpetaPadreId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PermisoId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);

            builder.HasMany(x => x.Subcarpetas).WithOne(x => x.CarpetaPadre).HasForeignKey(z => z.CarpetaPadreId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Permiso).WithMany(y => y.Carpetas).HasForeignKey(z => z.PermisoId);
        }


    }
}
