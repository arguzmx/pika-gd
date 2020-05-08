using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfTipoAmpliacion : IEntityTypeConfiguration<TipoAmpliacion>
    {
        public void Configure(EntityTypeBuilder<TipoAmpliacion> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaTipoAmpliacion);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasMany(x => x.Ampliaciones).WithOne(y => y.TipoAmpliacion).HasForeignKey(z => z.TipoAmpliacionId);
        }
    }
}
