using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivoDeclinado : IEntityTypeConfiguration<ActivoDeclinado>
    {
        public void Configure(EntityTypeBuilder<ActivoDeclinado> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivosDeclinados);
            builder.HasKey(x => new { x.ActivoId, x.TransferenciaId });
            builder.Property(x => x.ActivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TransferenciaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Motivo).HasMaxLength(2048).IsRequired();

            builder.HasOne(x => x.Activo).WithMany(y => y.DeclinadosTransferenciaRelacionados).HasForeignKey(x => x.ActivoId);
        }
    }
}
