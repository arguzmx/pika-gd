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
    public class DbConfPermiso : IEntityTypeConfiguration<Permiso>
    {
        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.ToTable(DbContextContenido.TablaPermisos);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Leer).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Escribir).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Crear).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Eliminar).IsRequired().HasDefaultValue(false);

            builder.HasMany(x => x.Destinatarios).WithOne(y => y.Permiso).HasForeignKey(x => x.PermisoId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Carpetas).WithOne(y => y.Permiso).HasForeignKey(x => x.PermisoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Elementos).WithOne(y => y.Permiso).HasForeignKey(x => x.PermisoId).OnDelete(DeleteBehavior.Restrict);

        }


    }
}
