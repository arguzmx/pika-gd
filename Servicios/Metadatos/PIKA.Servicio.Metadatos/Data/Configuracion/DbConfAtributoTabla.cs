using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
  public  class DbConfAtributoTabla : IEntityTypeConfiguration<AtributoTabla>
    {
        public void Configure(EntityTypeBuilder<AtributoTabla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaAtributoTabla);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.PropiedadId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Incluir).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.Visible).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.Alternable).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.IndiceOrdebnamiento).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.IdTablaCliente).HasMaxLength(LongitudDatos.GUID).IsRequired();

            //builder.HasOne(x => x.propiedadplantilla).WithOne(y => y.AtributoTabla).HasForeignKey<AtributoTabla>(z => z.Id);

        }
    }
}
