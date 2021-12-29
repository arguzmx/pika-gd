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
            builder.Property(x => x.CuadroClasifiacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Ignore(x => x.TipoValoracionDocumentalId);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Clave).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.AbreCon).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CierraCon).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.InstruccionFinal).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.Contiene).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.Posicion).IsRequired();
            builder.Property(x => x.VigenciaTramite).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.VigenciaConcentracion).IsRequired();
            builder.Property(x=>x.Descripcion).IsRequired(false);

            builder.Ignore(x => x.NombreCompleto);

            builder.HasMany(x => x.Activos).WithOne(y => y.EntradaClasificacion).HasForeignKey(z => z.EntradaClasificacionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.ValoracionesEntrada).WithOne(y => y.EntradaClasificacion).HasForeignKey(z => z.EntradaClasificacionId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.DisposicionEntrada).WithMany(y => y.EntradaClasificacion).HasForeignKey(z=>z.TipoDisposicionDocumentalId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}