using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfArchivo : IEntityTypeConfiguration<Archivo>
    {
        public void Configure(EntityTypeBuilder<Archivo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaArchivos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoArchivoId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasOne(x => x.Tipo).WithMany(y => y.Archivos).HasForeignKey(z => z.TipoArchivoId).OnDelete(DeleteBehavior.Restrict); 
            builder.HasMany(x => x.Activos).WithOne(z => z.ArchivoActual).HasForeignKey(y => y.ArchivoId).OnDelete(DeleteBehavior.Restrict); 
            builder.HasMany(x => x.HistorialArchivosActivo).WithOne(y => y.Archivo).HasForeignKey(z => z.ArchivoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Almacenes).WithOne(y => y.Archivo).HasForeignKey(z => z.ArchivoId);

        }
    }
}
