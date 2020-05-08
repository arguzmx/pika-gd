using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfPrestamo : IEntityTypeConfiguration<Prestamo>
    {
        public void Configure(EntityTypeBuilder<Prestamo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaPrestamos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x=>x.Eliminada).HasDefaultValue(false);
            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.FechaProgramadaDevolucion).IsRequired();
            builder.Property(x => x.FechaDevolucion).IsRequired(false);
            builder.Property(x => x.TieneDevolucionesParciales).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Folio).HasMaxLength(100).IsRequired();

            
            builder.HasMany(x => x.Comentarios).WithOne(y => y.Prestamo).HasForeignKey(z => z.PrestamoId);
            




        }
    }
}