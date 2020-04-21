
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfComentarioPrestamo : IEntityTypeConfiguration<ComentarioPrestamo>
    {
        public void Configure(EntityTypeBuilder<ComentarioPrestamo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaComentariosPrestamo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Comentario).HasMaxLength(2048).IsRequired();
            builder.Property(x => x.Fecha).IsRequired();

        }
    }
}