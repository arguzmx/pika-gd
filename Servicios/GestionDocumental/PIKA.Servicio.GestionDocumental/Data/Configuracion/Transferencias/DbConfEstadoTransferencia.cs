using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEstadoTransferencia : IEntityTypeConfiguration<EstadoTransferencia>
    {
        public void Configure(EntityTypeBuilder<EstadoTransferencia> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEstadosTransferencia);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

        }
    }
}
