using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Servicios;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
   public class DbConfPropiedadPlantilla : IEntityTypeConfiguration<PropiedadPlantilla>
    {
        public void Configure(EntityTypeBuilder<PropiedadPlantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaPropiedadPlantilla);
            builder.HasKey(x => x.Id);
            
            builder.HasIndex(x => x.PlantillaId);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x=>x.PlantillaId).IsRequired().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.TipoDatoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ValorDefault).HasColumnType("TEXT").IsRequired(false);
            builder.Property(x=>x.IndiceOrdenamiento).IsRequired();
            builder.Property(x=>x.Buscable).HasDefaultValue(true).IsRequired();
            builder.Property(x=>x.Ordenable).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.Visible).HasDefaultValue(true).IsRequired();
            builder.Property(x=>x.EsIdClaveExterna).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EsIdRegistro).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EsIdJerarquia).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EsTextoJerarquia).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EsIdRaizJerarquia).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EsFiltroJerarquia).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.Requerido).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.Autogenerado).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EsIndice).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.ControlHTML).HasMaxLength(LongitudDatos.ControlHTML).IsRequired();

            builder.HasOne(x => x.Plantilla).WithMany(y => y.Propiedades).HasForeignKey(z => z.PlantillaId);
            builder.HasOne(x => x.AtributoTabla).WithOne(y => y.PropiedadPlantilla).HasForeignKey<AtributoTabla>(z=>z.PropiedadId);
            builder.HasOne(x => x.ValidadorTexto).WithOne(y => y.PropiedadPlantilla).HasForeignKey<ValidadorTexto>(z => z.PropiedadId);
            builder.HasOne(x => x.ValidadorNumero).WithOne(y => y.PropiedadPlantilla).HasForeignKey<ValidadorNumero>(z => z.PropiedadId);
           

        }
    }
}
