using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfAplicacion : IEntityTypeConfiguration<Aplicacion>
    {
        public void Configure(EntityTypeBuilder<Aplicacion> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaAplicacion);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Descripcion).HasMaxLength(LongitudDatos.Descripcion).IsRequired();
            builder.Property(x => x.UICulture).HasMaxLength(LongitudDatos.UICulture).IsRequired();
            builder.Property(x => x.Version).HasMaxLength(LongitudDatos.Version).IsRequired();
            builder.Property(x => x.ReleaseIndex).HasDefaultValue(LongitudDatos.Version).IsRequired();

            builder.HasMany(x => x.Traducciones).WithOne(y => y.Aplicacion).HasForeignKey(z => z.AplicacionId);
            builder.HasMany(x => x.Modulos).WithOne(y => y.Aplicacion).HasForeignKey(z => z.AplicacionId);
        }
    }
}
