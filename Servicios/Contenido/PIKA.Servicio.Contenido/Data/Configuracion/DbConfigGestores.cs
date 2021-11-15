using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
    public class DbConfigGestorSMB : IEntityTypeConfiguration<GestorSMBConfig>
    {
        public void Configure(EntityTypeBuilder<GestorSMBConfig> builder)
        {
            builder.ToTable(DbContextContenido.TablaGestorSMB);
            builder.HasKey(x => x.VolumenId);

            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Contrasena).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Usuario).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Dominio).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Ruta).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();
                    
        }
    }
    public class DbConfigGestorLaserfiche : IEntityTypeConfiguration<GestorLaserficheConfig>
    {
        public void Configure(EntityTypeBuilder<GestorLaserficheConfig> builder)
        {
            builder.ToTable(DbContextContenido.TablaGestorLaserfiche);
            builder.HasKey(x => x.VolumenId);
            builder.Property(x => x.ConvertirTiff).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.FormatoConversion).IsRequired().HasDefaultValue("JPG").HasMaxLength(10);
            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Ruta).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();

        }
    }

    public class DbConfigGestorLocal : IEntityTypeConfiguration<GestorLocalConfig>
    {
        public void Configure(EntityTypeBuilder<GestorLocalConfig> builder)
        {
            builder.ToTable(DbContextContenido.TablaGestorLocal);
            builder.HasKey(x => x.VolumenId);

            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Ruta).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();

        }
    }


    public class DbConfigGestorAzure : IEntityTypeConfiguration<GestorAzureConfig>
    {
        public void Configure(EntityTypeBuilder<GestorAzureConfig> builder)
        {
            builder.ToTable(DbContextContenido.TablaGestorAzure);
            builder.HasKey(x => x.VolumenId);

            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Contrasena).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Usuario).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Endpoint).HasMaxLength(LongitudDatos.NombreLargo).IsRequired();

        }
    }
}
