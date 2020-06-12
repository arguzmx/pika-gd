using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfValorListaPropiedad :
       IEntityTypeConfiguration<ValorListaPlantilla>
    {
        public void Configure(EntityTypeBuilder<ValorListaPlantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaValoresListaPropiedad);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Texto).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Indice).IsRequired().HasDefaultValue(0);
            builder.HasOne(x => x.Propiedad).WithMany(y => y.ValoresLista).HasForeignKey(z => z.PropiedadId);
            builder.HasIndex(x => x.PropiedadId);

        }
    }

}
