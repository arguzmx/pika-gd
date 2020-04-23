using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfTraduccionAplicacionModulo : IEntityTypeConfiguration<TraduccionAplicacionModulo>
    {
        public void Configure(EntityTypeBuilder<TraduccionAplicacionModulo> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaTraduccionAplicacionModulo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.AplicacionId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ModuloId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Descripcion).HasMaxLength(LongitudDatos.Descripcion).IsRequired();
            builder.Property(x => x.UICulture).HasMaxLength(LongitudDatos.UICulture).IsRequired();



        }


    }

}
