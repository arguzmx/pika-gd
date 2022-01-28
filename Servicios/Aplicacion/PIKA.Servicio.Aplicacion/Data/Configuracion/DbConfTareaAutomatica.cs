using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Aplicacion.Tareas;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.AplicacionPlugin.Data.Configuracion
{
    public class DbConfTareaAutomatica : IEntityTypeConfiguration<TareaAutomatica>
    {
        public void Configure(EntityTypeBuilder<TareaAutomatica> builder)
        {
            builder.ToTable(DbContextAplicacion.TablaTareaAutomatica);
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoOrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.OrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).IsRequired(true).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.CodigoError).IsRequired(false).HasMaxLength(250);
            builder.Property(x => x.Exito).IsRequired(false);
            builder.Property(x => x.UltimaEjecucion).IsRequired(false);
            builder.Property(x => x.ProximaEjecucion).IsRequired(false);
            builder.Property(x => x.Duracion).IsRequired(false);
            builder.Property(x => x.Periodo).IsRequired(true);
            builder.Property(x => x.FechaHoraEjecucion).IsRequired(false);
            builder.Property(x => x.Intervalo).IsRequired(false);
            builder.Property(x => x.TareaEjecucionContinua).IsRequired(true);
            builder.Property(x => x.TareaEjecucionContinuaMinutos).IsRequired(true);
            builder.Property(x => x.Estado).IsRequired(true);

            builder.HasMany(x=>x.Bitacora).WithOne(y=>y.Tarea).HasForeignKey(z=>z.TareaId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }



    public class DbConfBitacoraTarea : IEntityTypeConfiguration<BitacoraTarea>
    {
        public void Configure(EntityTypeBuilder<BitacoraTarea> builder)
        {
            

            builder.ToTable(DbContextAplicacion.TablaBitacoraTarea);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TareaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.CodigoError).IsRequired(false).HasMaxLength(250);
            builder.Property(x => x.Exito).IsRequired(true);
            builder.Property(x => x.FechaEjecucion).HasDefaultValue(new DateTime(2000,1,1,0,0,0)).IsRequired(true);
            builder.Property(x => x.Duracion).IsRequired(true);

            builder.HasIndex(x => x.TareaId);
        }
    }
}
