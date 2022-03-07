using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Aplicacion.Tareas;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.AplicacionPlugin.Data.Configuracion
{
    public class DbConfTareaEnDemanda : IEntityTypeConfiguration<ColaTareaEnDemanda>
    {
        public void Configure(EntityTypeBuilder<ColaTareaEnDemanda> builder)
        {
            builder.ToTable(DbContextAplicacion.TablaTareasEnDemanda);
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Completada).IsRequired(true);
            builder.Property(x => x.DominioId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Error).IsRequired(false).HasMaxLength(LongitudDatos.Descripcion);
            builder.Property(x => x.FechaCaducidad).IsRequired(false);
            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.FechaEjecucion).IsRequired(false);
            builder.Property(x => x.NombreEnsamblado).IsRequired(true).HasMaxLength(LongitudDatos.IDunico);
            builder.Property(x => x.InputPayload).IsRequired(false).HasMaxLength(2000);
            builder.Property(x => x.OutputPayload).IsRequired(false).HasMaxLength(2000);
            builder.Property(x => x.Prioridad).IsRequired(true);
            builder.Property(x => x.TareaProcesoId).IsRequired(true).HasMaxLength(LongitudDatos.IDunico); 
            builder.Property(x => x.TipoRespuesta).IsRequired(true);
            builder.Property(x => x.TenantId).IsRequired(true).HasMaxLength(LongitudDatos.GUID); 
            builder.Property(x => x.URLRecoleccion).IsRequired(true).HasMaxLength(LongitudDatos.IDunico);
            builder.Property(x => x.UsuarioId).IsRequired(true).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Recogida).IsRequired(true);
        }
    }

}
