using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
  public  class DbConfAtributoLista : IEntityTypeConfiguration<AtributoLista>
    {
        public void Configure(EntityTypeBuilder<AtributoLista> builder)
        {
            //builder.ToTable(DbContextMetadatos.TablaAtributoLista);
            builder.HasKey(x => x.PropiedadId);

            builder.Property(x => x.PropiedadId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Entidad).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Default).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.OrdenarAlfabetico).IsRequired();
            builder.Property(x => x.DatosRemotos).IsRequired();
            builder.Property(x => x.TypeAhead).IsRequired();


        }
    }
}