using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfElementoClasificacion : IEntityTypeConfiguration<ElementoClasificacion>
    {
        public void Configure(EntityTypeBuilder<ElementoClasificacion> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaElementosClasificacion);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.Clave).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Posicion).HasMaxLength(LongitudDatos.CodigoPostal).IsRequired();

            builder.Property(x => x.CuadroClasifiacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.HasMany(x => x.Hijos).WithOne(y => y.Padre).HasForeignKey(z => z.ElementoClasificacionId);

        }
    }
}
