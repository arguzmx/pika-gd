using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEventoTransferencia : IEntityTypeConfiguration<EventoTransferencia>
    {
        public void Configure(EntityTypeBuilder<EventoTransferencia> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEventosTransferencia);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TransferenciaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EstadoTransferenciaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.UsuarioId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Comentario).HasMaxLength(2048).IsRequired(false);

            builder.HasOne(x => x.Estado).WithMany(y => y.Eventos).HasForeignKey(z => z.EstadoTransferenciaId);
        }
    }
}
