using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
   public class DbConfAsociacionPlantilla : IEntityTypeConfiguration<AsociacionPlantilla>
    {
        public void Configure(EntityTypeBuilder<AsociacionPlantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaAsociacionPlantilla);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PlantillaId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoAlmacenMetadatosId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.IdentificadorAlmacenamiento).HasMaxLength(LongitudDatos.GUID).IsRequired();
         
            
            builder.HasOne(x => x.TipoAlmacen).WithMany(y => y.AsociacionesPlantilla).HasForeignKey(z => z.TipoAlmacenMetadatosId);
        }
    }
}
