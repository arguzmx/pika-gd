using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Aplicacion.Plugins;
using RepositorioEntidades;

namespace PIKA.Servicio.AplicacionPlugin.Data.Configuracion
{
    public class DbConfPlugin : IEntityTypeConfiguration<Plugin>
    {
        public void Configure(EntityTypeBuilder<Plugin> builder)
        {
            builder.ToTable(DbContextAplicacionPlugin.TablaPlugin);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

           builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();

            builder.Property(x=>x.Gratuito).HasDefaultValue(false).IsRequired(); 

            
        }
    }
}
