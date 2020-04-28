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
    public class DbConfPluginInstalado : IEntityTypeConfiguration<PluginInstalado>
    {
        public void Configure(EntityTypeBuilder<PluginInstalado> builder)
        {
            builder.ToTable(DbContextAplicacionPlugin.TablaPluginInstalado);
            builder.HasKey(x => new { x.PLuginId, x.VersionPLuginId });

            builder.Property(x => x.PLuginId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.VersionPLuginId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Activo).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.FechaInstalacion).HasDefaultValue(DateTime.Now).IsRequired();
            

            builder.HasOne(x => x.Plugin).WithMany(y=>y.PluginInstalados).HasForeignKey(z=>z.PLuginId);
            builder.HasOne(x=>x.VersionPlugin).WithMany(y=>y.PluginInstalados).HasForeignKey(z=>z.VersionPLuginId);

        }


    }
}
