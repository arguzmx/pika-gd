using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
    public class DbConfVolumen : IEntityTypeConfiguration<Volumen>
    {
        public void Configure(EntityTypeBuilder<Volumen> builder)
        {
            builder.ToTable(DbContextContenido.TablaVolumen);
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.TipoOrigenDefault);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
           
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.TipoGestorESId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Activo).IsRequired();
            builder.Property(x=>x.EscrituraHabilitada).IsRequired();
            builder.Property(x => x.ConfiguracionValida).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).IsRequired();
            builder.Property(x=>x.ConsecutivoVolumen).HasDefaultValue(0).IsRequired();
            builder.Property(x=>x.CanidadPartes).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.CanidadElementos).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.Tamano).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.TamanoMaximo).HasDefaultValue(0).IsRequired();

            builder.HasMany(x => x.PuntosMontajeVolumen).WithOne(y => y.Volumen).HasForeignKey(z => z.VolumenId).OnDelete(DeleteBehavior.Restrict).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.TipoGestorES).WithMany(y => y.Volumenes).HasForeignKey(z => z.TipoGestorESId).OnDelete(DeleteBehavior.Restrict).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.SMBConfig).WithOne(y => y.Volumen).HasForeignKey<GestorSMBConfig>(z => z.VolumenId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.AxureConfig).WithOne(y => y.Volumen).HasForeignKey<GestorAzureConfig>(z => z.VolumenId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.LocalConfig).WithOne(y => y.Volumen).HasForeignKey<GestorLocalConfig>(z => z.VolumenId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.LaserficheConfig).WithOne(y => y.Volumen).HasForeignKey<GestorLaserficheConfig>(z => z.VolumenId).OnDelete(DeleteBehavior.Cascade);
        }

     
    }
}
