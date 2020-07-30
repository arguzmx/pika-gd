using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfCuadroClasificacion : IEntityTypeConfiguration<CuadroClasificacion>
    {
        public void Configure(EntityTypeBuilder<CuadroClasificacion> builder)
        {

            builder.ToTable(DBContextGestionDocumental.TablaCuadrosClasificacion);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();

            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EstadoCuadroClasificacionId).HasMaxLength(LongitudDatos.GUID)
                .HasDefaultValue(ConstantesEstado.ESTADO_ACTIVO).IsRequired();

            builder.HasMany(x => x.Elementos).WithOne(y=>y.CuadroClasificacion).HasForeignKey(z=>z.CuadroClasifiacionId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Estado).WithMany(x => x.Cuadros).HasForeignKey(x => x.EstadoCuadroClasificacionId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
