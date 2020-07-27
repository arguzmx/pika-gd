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
    public class DbConfDestinatarioPermiso : IEntityTypeConfiguration<DestinatarioPermiso>
    {
        public void Configure(EntityTypeBuilder<DestinatarioPermiso> builder)
        {
            builder.ToTable(DbContextContenido.TablaDestinatariosPermisos);
            builder.HasKey(x => new {x.PermisoId, x.DestinatarioId } );

            builder.Property(x => x.PermisoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.DestinatarioId).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.EsGrupo).IsRequired().HasDefaultValue(false);
            builder.HasOne(x => x.Permiso).WithMany(y => y.Destinatarios).HasForeignKey(z => z.PermisoId);

        }


    }
}
