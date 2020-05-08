using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivoPrestamo : IEntityTypeConfiguration<ActivoPrestamo>
    {
        public void Configure(EntityTypeBuilder<ActivoPrestamo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivosPrestamo);
            builder.HasKey(x => new { x.PrestamoId, x.ActivoId });
            builder.Property(x => x.PrestamoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ActivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Devuelto).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.FechaDevolucion);

            builder.HasOne(x => x.Activo).WithMany(y => y.PrestamosRelacionados).HasForeignKey(z => z.ActivoId);
            builder.HasOne(x => x.Prestamo).WithMany(y => y.ActivosRelacionados).HasForeignKey(z => z.PrestamoId);

        }
    }
}