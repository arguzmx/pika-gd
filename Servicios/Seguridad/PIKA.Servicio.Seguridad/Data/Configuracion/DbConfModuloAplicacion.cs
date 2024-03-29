﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfModuloAplicacion : IEntityTypeConfiguration<ModuloAplicacion>
    {
        public void Configure(EntityTypeBuilder<ModuloAplicacion> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaModuloAplicacion);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.AplicacionId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ModuloPadreId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Descripcion).HasMaxLength(LongitudDatos.Descripcion).IsRequired();
            builder.Property(x => x.UICulture).HasMaxLength(LongitudDatos.UICulture).IsRequired();
            builder.Property(x => x.Asegurable).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Icono).HasMaxLength(LongitudDatos.Icono).IsRequired();
            builder.Property(x=>x.PermisosDisponibles).IsRequired();

            builder.HasMany(x => x.Modulos).WithOne(y => y.ModuloPadre).HasForeignKey(z => z.ModuloPadreId);
            builder.HasMany(x => x.Traducciones).WithOne(y => y.ModuloApp).HasForeignKey(z => z.ModuloId);
            builder.HasMany(x => x.TiposAdministrados).WithOne(y => y.ModuloApp).HasForeignKey(z => z.ModuloId);

        }


    }
}
