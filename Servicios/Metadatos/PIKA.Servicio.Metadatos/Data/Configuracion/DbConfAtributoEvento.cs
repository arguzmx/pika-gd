using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
  public  class DbConfAtributoEvento : IEntityTypeConfiguration<AtributoEvento>
    {
        public void Configure(EntityTypeBuilder<AtributoEvento> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaAtributoEvento);
            builder.HasKey(x => x.PropiedadId);

            builder.Property(x => x.PropiedadId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Entidad).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Parametro).HasMaxLength(LongitudDatos.Nombre).IsRequired();


        }
    }
}