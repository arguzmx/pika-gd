using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfValorLista : IEntityTypeConfiguration<ValorLista>
    {
        public void Configure(EntityTypeBuilder<ValorLista> builder)
        {
            //builder.ToTable(DbContextMetadatos.TablaValorLista);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Texto).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Indice).IsRequired();


        }
    }
}