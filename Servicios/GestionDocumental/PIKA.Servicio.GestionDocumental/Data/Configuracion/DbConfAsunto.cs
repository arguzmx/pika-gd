using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfAsunto : IEntityTypeConfiguration<Asunto>
    {
        public void Configure(EntityTypeBuilder<Asunto> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaAsuntos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Contenido).IsRequired();

            builder.HasOne(x => x.Activo).WithOne(y => y.oAsunto).HasForeignKey<Asunto>(z=>z.ActivoId).IsRequired();

        }
    }
}