using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfAmpliacion : IEntityTypeConfiguration<Ampliacion>
    {
        public void Configure(EntityTypeBuilder<Ampliacion> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaAmpliacion);
            builder.HasKey(x => new { x.ActivoId, x.Vigente });

            builder.Property(x => x.ActivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Vigente).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.TipoAmpliacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.FechaFija).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.FundamentoLegal).HasMaxLength(2000).IsRequired();
            builder.Property(x=>x.Inicio);
            builder.Property(x => x.Fin);
            builder.Property(x => x.Dias);
            builder.Property(x => x.Meses);
            builder.Property(x => x.Anos);

            builder.HasOne(x => x.TipoAmpliacion).WithMany(x => x.Ampliaciones).HasForeignKey(x => x.TipoAmpliacionId);
        }
    }
}
