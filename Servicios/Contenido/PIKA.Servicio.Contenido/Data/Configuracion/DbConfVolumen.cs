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

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
           
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.TipoGestorESId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Elementoid).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Activo).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.EscrituraHabilitada).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.CadenaConexion).HasMaxLength(LongitudDatos.CadenaConexion).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x=>x.ConsecutivoVolumen).HasDefaultValue(0).IsRequired();
            builder.Property(x=>x.CanidadPartes).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.Tamano).HasDefaultValue(0).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();


        }

     
    }
}
