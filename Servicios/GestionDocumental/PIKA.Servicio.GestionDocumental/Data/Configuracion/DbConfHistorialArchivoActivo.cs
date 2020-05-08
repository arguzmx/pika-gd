using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfHistorialArchivoActivo : IEntityTypeConfiguration<HistorialArchivoActivo>
    {
        public void Configure(EntityTypeBuilder<HistorialArchivoActivo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaHistorialArchivoActivos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.ArchivoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ActivoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.FechaIngreso).IsRequired();
            builder.Property(x => x.FechaEgreso).IsRequired(false);

        }
    }
}