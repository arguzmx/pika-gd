﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Reportes;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Reportes.Data.Configuracion
{
  public  class DbConfReporteEntidad : IEntityTypeConfiguration<ReporteEntidad>
    {
        public void Configure(EntityTypeBuilder<ReporteEntidad> builder)
        {
            builder.ToTable(DbContextReportes.TablaReporteEntidad);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Entidad).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.GrupoReportes).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.SubReporte).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Bloqueado).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Descripcion).HasMaxLength(LongitudDatos.Descripcion).IsRequired();
            builder.Property(x=>x.Plantilla).HasColumnType("LONGTEXT").IsRequired();
            builder.Property(x => x.ExtensionSalida).IsRequired(true).HasMaxLength(LongitudDatos.Tipo);
            builder.HasIndex(x => x.Entidad);
            builder.HasIndex(x => x.OrigenId);
            builder.HasIndex(x => x.TipoOrigenId);
        }
    }
}