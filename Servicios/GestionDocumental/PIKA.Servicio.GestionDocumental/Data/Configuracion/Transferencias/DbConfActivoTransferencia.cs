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
            builder.HasKey(x => new { x.ActivoId, x.TransferenciaId});
            builder.Property(x => x.ActivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TransferenciaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasOne(x => x.Activo).WithMany(y => y.TransferenciasRelacionados).HasForeignKey(x => x.ActivoId);
        }
    }
}
