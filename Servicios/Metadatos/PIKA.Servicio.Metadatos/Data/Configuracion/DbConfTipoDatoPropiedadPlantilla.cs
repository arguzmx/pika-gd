using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
   public class DbConfTipoDatoPropiedadPlantilla: IEntityTypeConfiguration<TipoDatoPropiedadPlantilla>
    {
        public void Configure(EntityTypeBuilder<TipoDatoPropiedadPlantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaTipoDatoPropiedadPlantilla);
            builder.HasKey(x =>  new { x.TipoDatoId, x.PropiedadPlantillaId });
            builder.Property(x => x.TipoDatoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.PropiedadPlantillaId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.HasOne(x => x.Propiedad).WithOne(y => y.TipoDatoPropiedad);
            builder.HasOne(x => x.Tipo).WithMany(y => y.PropiedadesPlantilla).HasForeignKey(z => z.TipoDatoId);

        }
    }
}
