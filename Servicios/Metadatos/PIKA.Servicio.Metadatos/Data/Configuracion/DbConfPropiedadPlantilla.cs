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
            builder.Property(x=>x.Buscable).IsRequired();
            builder.Property(x=>x.Ordenable).IsRequired();
            builder.Property(x=>x.Visible).IsRequired();
            builder.Property(x=>x.EsIdClaveExterna).IsRequired();
            builder.Property(x=>x.EsIdRegistro).IsRequired();
            builder.Property(x=>x.EsIdJerarquia).IsRequired();
            builder.Property(x=>x.EsTextoJerarquia).IsRequired();
            builder.Property(x=>x.EsIdRaizJerarquia).IsRequired();
            builder.Property(x=>x.EsFiltroJerarquia).IsRequired();
            builder.Property(x=>x.Requerido).IsRequired();
            builder.Property(x=>x.Autogenerado).IsRequired();
            builder.Property(x=>x.EsIndice).IsRequired();
            builder.Property(x=>x.ControlHTML).HasMaxLength(LongitudDatos.ControlHTML).IsRequired();

            builder.Ignore(x => x.AtributoLista);
            builder.Ignore(x => x.AtributosEvento);
            builder.Ignore(x => x.AtributosVistaUI);
            builder.Ignore(x => x.AtributoTabla);
            builder.Ignore(x => x.ParametroLinkVista);


            builder.HasOne(x => x.ValidadorTexto).WithOne(y => y.PropiedadPlantilla).HasForeignKey<ValidadorTexto>(z => z.PropiedadId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ValidadorNumero).WithOne(y => y.PropiedadPlantilla).HasForeignKey<ValidadorNumero>(z => z.PropiedadId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
