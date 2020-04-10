using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfAtributoMetadato : IEntityTypeConfiguration<AtributoMetadato>
    {
        public void Configure(EntityTypeBuilder<AtributoMetadato> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaAtributoMetadato);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PropiedadId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Valor).HasMaxLength(LongitudDatos.ValorDefault);

            builder.HasOne(x => x.Propiedad).WithMany(y => y.Atributos).HasForeignKey(z => z.Id);

        }
    }
}
