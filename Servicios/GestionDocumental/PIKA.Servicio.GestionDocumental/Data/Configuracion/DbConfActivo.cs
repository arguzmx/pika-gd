using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivo : IEntityTypeConfiguration<Activo>
    {
        public void Configure(EntityTypeBuilder<Activo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            //¿Requerida?
            builder.Property(x => x.EntradaClasificacionId).IsRequired().ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.IDunico).HasMaxLength(LongitudDatos.IDunico);
            builder.Property(x => x.Asunto).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.FechaApertura).IsRequired();
            builder.Property(x => x.FechaCierre).IsRequired(false);
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.EsElectronico).IsRequired();
            builder.Property(x => x.CodigoOptico).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(LongitudDatos.TEXTO_INDEXABLE_LARGO).IsRequired(false);
            builder.Property(x => x.EnPrestamo).IsRequired();
            builder.Property(x => x.Reservado).IsRequired();
            builder.Property(x => x.Confidencial).IsRequired();
            builder.Property(x => x.Ampliado).IsRequired();
            builder.Property(x => x.ArchivoOrigenId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ArchivoId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoArchivoId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.FechaRetencionAC).IsRequired(false);
            builder.Property(x => x.FechaRetencionAT).IsRequired(false);
            

            builder.HasIndex(i => new { i.ArchivoOrigenId });
            builder.HasIndex(i => new { i.TipoArchivoId });
            builder.HasIndex(i => new { i.EnPrestamo });
            builder.HasIndex(i => new { i.Ampliado });
            builder.HasIndex(i => new { i.FechaRetencionAC });
            builder.HasIndex(i => new { i.FechaRetencionAT });
            builder.HasIndex(i => new { i.FechaCierre });
            builder.HasIndex(i => new { i.Nombre });
            builder.HasIndex(i => new { i.CodigoElectronico });
            builder.HasIndex(i => new { i.CodigoOptico });
            builder.HasIndex(i => new { i.ArchivoId });


            builder.HasMany(x => x.HistorialArchivosActivo).WithOne(y => y.Activo).HasForeignKey(z => z.ActivoId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Asuntos).WithOne(y => y.Activo).HasForeignKey<Asunto>(z=>z.ActivoId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}