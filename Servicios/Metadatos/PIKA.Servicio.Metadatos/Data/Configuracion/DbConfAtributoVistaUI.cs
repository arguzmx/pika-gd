using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfAtributoVistaUI : IEntityTypeConfiguration<AtributoVistaUI>
    {
        public void Configure(EntityTypeBuilder<AtributoVistaUI> builder)
        {
            //builder.ToTable(DbContextMetadatos.TablaAtributoVistaUI);
            builder.HasKey(x => x.PropiedadId);

            builder.Property(x => x.PropiedadId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Control).IsRequired().HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x=>x.Plataforma).IsRequired().HasMaxLength(LongitudDatos.Nombre);

        }
    }
}
