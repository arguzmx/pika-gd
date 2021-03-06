﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data.Configuracion
{
    public class DbConfPlantilla : IEntityTypeConfiguration<Plantilla>
    {
        public void Configure(EntityTypeBuilder<Plantilla> builder)
        {
            builder.ToTable(DbContextMetadatos.TablaPlantilla);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Eliminada).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x=>x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Ignore(x => x.Asociaciones);

            builder.HasMany(x => x.Propiedades).WithOne(y => y.Plantilla).HasForeignKey(z => z.PlantillaId).OnDelete(DeleteBehavior.Restrict);


        }
    }
}
