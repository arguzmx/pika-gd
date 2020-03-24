using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEstadoCuadroClasificacion : IEntityTypeConfiguration<EstadoCuadroClasificacion>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<EstadoCuadroClasificacion> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEstadosCuadro);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
        }
    }
}
