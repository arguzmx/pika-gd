using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfComentarioTransferencia : IEntityTypeConfiguration<ComentarioTransferencia>
    {
        public void Configure(EntityTypeBuilder<ComentarioTransferencia> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaComentariosTransferencia);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TransferenciaId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.UsuarioId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.Comentario).HasMaxLength(2048).IsRequired();
            builder.Property(x => x.Publico).HasDefaultValue(false).IsRequired();
        }
    }
}
