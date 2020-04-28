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
    public class DbConfVersionPlugin : IEntityTypeConfiguration<VersionPlugin>
    {
        public void Configure(EntityTypeBuilder<VersionPlugin> builder)
        {
            builder.ToTable(DbContextAplicacionPlugin.TablaVersionPlugin);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x=>x.PluginId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.RequiereConfiguracion).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.URL).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Version).HasMaxLength(LongitudDatos.Version).IsRequired();

            builder.HasOne(x => x.Plugins).WithMany(y=>y.versionPlugins).HasForeignKey(z=>z.PluginId);

        }
    }

}
