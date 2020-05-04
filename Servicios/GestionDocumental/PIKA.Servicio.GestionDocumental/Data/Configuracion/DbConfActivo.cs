using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivo : IEntityTypeConfiguration<Activo>
    {
        public void Configure(EntityTypeBuilder<Activo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivos);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Asunto).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.FechaApertura).IsRequired();
            builder.Property(x => x.FechaCierre).IsRequired(false);
            builder.Property(x => x.EsElectronio).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.CodigoOptico).HasMaxLength(1024).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(1024).IsRequired(false);
            builder.Property(x => x.EnPrestamo).HasDefaultValue(false).IsRequired();

            builder.HasOne(x => x.ElementoClasificacion).WithMany(y => y.Activos).HasForeignKey(z => z.ElementoClasificacionId).IsRequired();


            //Un archivo (es una unidad física) tiene muchos activos asociados asi que hay que rreemplazr por Archivo 1 ---> * Activos

            //builder.HasOne(x => x.ArchivoActual).WithOne(y => y.ActivoActual).HasForeignKey<Activo>(z=>z.ArchivoId).IsRequired();

        }
    }
}