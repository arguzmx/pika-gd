using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfZonaAlmacen : IEntityTypeConfiguration<ZonaAlmacen>
    {
        public void Configure(EntityTypeBuilder<ZonaAlmacen> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaZonasAlmacen);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.AlmacenArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Clave).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            builder.HasIndex(x => x.ArchivoId);
            builder.HasIndex(x => x.AlmacenArchivoId);
            builder.HasIndex(x => x.Nombre);
            builder.HasIndex(x => x.Clave);



            builder.HasMany(x => x.Posiciones).WithOne(y => y.Zona).HasForeignKey(z => z.ZonaAlmacenId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Contenedores).WithOne(y => y.Zona).HasForeignKey(z => z.ZonaAlmacenId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
