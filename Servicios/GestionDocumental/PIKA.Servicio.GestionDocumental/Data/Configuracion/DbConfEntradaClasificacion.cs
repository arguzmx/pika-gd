using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEntradaClasificacion : IEntityTypeConfiguration<EntradaClasificacion>
    {
        public void Configure(EntityTypeBuilder<EntradaClasificacion> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEntradaClasificacion);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ElementoClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Ignore(x => x.TipoValoracionDocumentalId);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Clave).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Posicion).IsRequired();
            builder.Property(x => x.VigenciaTramite).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.VigenciaConcentracion).IsRequired();

            builder.Ignore(x => x.NombreCompleto);

            builder.HasMany(x => x.Activos).WithOne(y => y.EntradaClasificacion).HasForeignKey(z => z.EntradaClasificacionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.ValoracionesEntrada).WithOne(y => y.EntradaClasificacion).HasForeignKey(z => z.EntradaClasificacionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.DisposicionEntrada).WithMany(y => y.EntradaClasificacion).HasForeignKey(z=>z.TipoDisposicionDocumentalId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}