using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivoTransferencia : IEntityTypeConfiguration<ActivoTransferencia>
    {
        public void Configure(EntityTypeBuilder<ActivoTransferencia> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivosTransferencia);
            builder.HasKey(x => new { x.Id});
            builder.Property(x => x.ActivoId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TransferenciaId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Declinado).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.Aceptado).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.FechaVoto).IsRequired(false);
            builder.Property(x => x.FechaRetencion).IsRequired(true);
            builder.Property(x => x.CuadroClasificacionId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.EntradaClasificacionId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Notas).IsRequired(false).HasMaxLength(LongitudDatos.Descripcion);
            builder.Property(x => x.MotivoDeclinado).IsRequired(false).HasMaxLength(LongitudDatos.Descripcion);
            builder.Property(x => x.UsuarioId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.UsuarioReceptorId).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.HasIndex(x => new { x.ActivoId, x.TransferenciaId, x.Declinado } );
            builder.HasIndex(x => x.UsuarioId);
            builder.HasIndex(x => x.UsuarioReceptorId);

            builder.HasOne(x => x.Activo).WithMany(y => y.TransferenciasRelacionados).HasForeignKey(x => x.ActivoId);
        }
    }
}
