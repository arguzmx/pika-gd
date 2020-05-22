using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
 public   class DbConfTipoAlmacenMetadatos : IEntityTypeConfiguration<TipoAlmacenMetadatos>
    {
        public void Configure(EntityTypeBuilder<TipoAlmacenMetadatos> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaTipoAlmacenMetadatos);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.HasMany(x => x.AlmacensDatos).WithOne(y => y.TipoAlmacen).HasForeignKey(z => z.TipoAlmacenMetadatosId);

        }
    }
}
