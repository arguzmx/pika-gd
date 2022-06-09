using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfPosicionalmacen : IEntityTypeConfiguration<PosicionAlmacen>
    {
        public void Configure(EntityTypeBuilder<PosicionAlmacen> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaPosicionAlmacen);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.AlmacenArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ZonaAlmacenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.PosicionPadreId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Indice).IsRequired();
            builder.Property(x => x.Localizacion).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.CodigoBarras).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.Ocupacion).IsRequired();
            builder.Property(x => x.IncrementoContenedor).IsRequired();

            builder.HasIndex(x => x.ArchivoId);
            builder.HasIndex(x => x.AlmacenArchivoId);
            builder.HasIndex(x => x.ZonaAlmacenId);
            builder.HasIndex(x => x.PosicionPadreId);
            builder.HasIndex(x => x.Nombre);
            builder.HasIndex(x => x.CodigoBarras);
            builder.HasIndex(x => x.CodigoElectronico);

            builder.HasMany(x => x.Posiciones).WithOne(y => y.PosicionPadre).HasForeignKey(z => z.PosicionPadreId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Contenedores).WithOne(y => y.Posicion).HasForeignKey(z => z.PosicionAlmacenId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
