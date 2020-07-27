using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
    public class DbConfVolumenPuntoMontaje : IEntityTypeConfiguration<VolumenPuntoMontaje>
    {
        public void Configure(EntityTypeBuilder<VolumenPuntoMontaje> builder)
        {
            builder.ToTable(DbContextContenido.TablaVolumenPuntoMontaje);
            builder.HasKey(x =>  new { x.VolumenId, x.PuntoMontajeId } );

            builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.PuntoMontajeId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasOne(x => x.PuntoMontaje).WithMany(y => y.VolumenesPuntoMontaje).HasForeignKey(z => z.PuntoMontajeId);
            builder.HasOne(x => x.Volumen).WithMany(y => y.PuntosMontajeVolumen).HasForeignKey(z => z.VolumenId);
        }


    }
}
