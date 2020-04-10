using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfAtributoTablaPropiedadPlantilla : IEntityTypeConfiguration<AtributoTablaPropiedadPlantilla>
    {
        public void Configure(EntityTypeBuilder<AtributoTablaPropiedadPlantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaAtributoTablaPropiedadPlantilla);
            builder.HasKey(x => new { x.PropiedadPlantillaid, x.Atributotablaid});
            builder.Property(x => x.PropiedadPlantillaid).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Atributotablaid).HasMaxLength(LongitudDatos.GUID).IsRequired();
          
            //builder.HasOne(x => x.PropiedadPlantilla).WithOne(y => y.Atributotablapropiedad).HasForeignKey(z => z);
        }
    }
}
