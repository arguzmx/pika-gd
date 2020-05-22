using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfAlmacenDatos : IEntityTypeConfiguration<AlmacenDatos>
    {
        public void Configure(EntityTypeBuilder<AlmacenDatos> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaAlmacenDatos);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.Protocolo).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Direccion).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Usuario).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Contrasena).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.Puerto).HasMaxLength(50).IsRequired(false);
            builder.Property(x => x.TipoAlmacenMetadatosId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasMany(x => x.Plantillas).WithOne(y => y.Almacen).HasForeignKey(z => z.AlmacenDatosId);

        }
    }
}
