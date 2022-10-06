using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfTransferencia : IEntityTypeConfiguration<Transferencia>
    {
        public void Configure(EntityTypeBuilder<Transferencia> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaTransferencias);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EstadoTransferenciaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ArchivoOrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ArchivoDestinoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.UsuarioId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.FechaCreacion).IsRequired();
            builder.Property(x => x.CantidadActivos).IsRequired();
            builder.Property(x => x.EntradaClasificacionId).IsRequired(false);
            builder.Property(x => x.CuadroClasificacionId).IsRequired(false);
            builder.Property(x => x.Folio).IsRequired(false).HasMaxLength(LongitudDatos.NombreLargo);

            builder.HasOne(x => x.Estado).WithMany(y => y.Transferencias).HasForeignKey(z => z.EstadoTransferenciaId);
            builder.HasMany(x => x.Eventos).WithOne(y => y.Transferencia).HasForeignKey(z => z.TransferenciaId);
            builder.HasMany(x => x.Comentarios).WithOne(y => y.Transferencia).HasForeignKey(z => z.TransferenciaId);
            builder.HasMany(x => x.ActivosIncluidos).WithOne(y => y.Transferencia).HasForeignKey(z => z.TransferenciaId);
            builder.HasOne(x => x.ArchivoOrigen).WithMany(y => y.TransferenciasOrigen).HasForeignKey(z => z.ArchivoOrigenId);
            builder.HasOne(x => x.ArchivoDestino).WithMany(y => y.TransferenciasDestino).HasForeignKey(z => z.ArchivoDestinoId);
        }
    }
}
