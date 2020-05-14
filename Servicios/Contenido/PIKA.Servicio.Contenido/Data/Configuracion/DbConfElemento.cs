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
    public class DbConfElemento : IEntityTypeConfiguration<Elemento>
    {
        public void Configure(EntityTypeBuilder<Elemento> builder)
        {
            builder.ToTable(DbContextContenido.TablaElemento);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x=>x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.CreadorId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            //builder.Property(x => x.VolumenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.FechaCreacion).HasDefaultValue(DateTime.Now).IsRequired();


            builder.HasOne(x => x.Volumen).WithOne(y => y.Elemento);
            builder.HasMany(x=>x.Versiones).WithOne(y=>y.Elemento).HasForeignKey(z=>z.ElementoId);

        }


    }
}
