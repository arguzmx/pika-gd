using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfAlmacenArchivo : IEntityTypeConfiguration<AlmacenArchivo>
    {
        public void Configure(EntityTypeBuilder<AlmacenArchivo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaAlmacenesArchivo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Clave).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.HabilitarFoliado).IsRequired();
            builder.Property(x => x.FolioActualContenedor).IsRequired();
            builder.Property(x => x.MacroFolioContenedor).HasMaxLength(250).IsRequired(false);
            builder.Property(x => x.Ubicacion).HasMaxLength(LongitudDatos.Nombre).IsRequired(false);

            builder.HasIndex(x => x.ArchivoId);
            builder.HasIndex(x => x.Nombre);
            builder.HasIndex(x => x.Clave);

            builder.HasMany(x=>x.Zonas).WithOne(y=>y.Almacen).HasForeignKey(z=>z.AlmacenArchivoId).OnDelete( DeleteBehavior.Restrict);
            builder.HasMany(x => x.Posiciones).WithOne(y => y.Almacen).HasForeignKey(z => z.AlmacenArchivoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Contenedores).WithOne(y => y.Almacen).HasForeignKey(z => z.AlmacenArchivoId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
