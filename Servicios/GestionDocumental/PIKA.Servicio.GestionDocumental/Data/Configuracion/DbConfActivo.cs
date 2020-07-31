using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivo : IEntityTypeConfiguration<Activo>
    {
        public void Configure(EntityTypeBuilder<Activo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            //¿Requerida?
            builder.Property(x => x.ElementoClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Asunto).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.FechaApertura).IsRequired();
            builder.Property(x => x.FechaCierre).IsRequired(false);
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.EsElectronio).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.CodigoOptico).HasMaxLength(1024).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(1024).IsRequired(false);
            builder.Property(x => x.EnPrestamo).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Reservado).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Confidencial).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Ampliado).HasDefaultValue(false).IsRequired();

            builder.HasMany(x => x.Ampliaciones).WithOne(y => y.activo).HasForeignKey(z => z.ActivoId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.HistorialArchivosActivo).WithOne(y => y.Activo).HasForeignKey(z => z.ActivoId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}