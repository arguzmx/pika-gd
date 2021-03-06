﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfValidadorNumero : IEntityTypeConfiguration<ValidadorNumero>
    {
        public void Configure(EntityTypeBuilder<ValidadorNumero> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaValidadorNumero);
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.PropiedadId);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PropiedadId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.min).IsRequired();
            builder.Property(x => x.max).IsRequired();
            builder.Property(x => x.valordefault).IsRequired();

        }
    }
}
