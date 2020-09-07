using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfPermisosAplicacion : IEntityTypeConfiguration<PermisoAplicacion>
    {
        public void Configure(EntityTypeBuilder<PermisoAplicacion> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaPermisosApp);
            builder.HasKey(x =>  new { x.DominioId, x.AplicacionId, x.ModuloId, x.EntidadAccesoId, x.TipoEntidadAcceso } );

            builder.Property(x => x.DominioId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.AplicacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ModuloId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.EntidadAccesoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoEntidadAcceso).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Leer).IsRequired();
            builder.Property(x => x.Escribir).IsRequired();
            builder.Property(x => x.NegarAcceso).IsRequired();
            builder.Property(x => x.Admin).IsRequired();
            builder.Property(x => x.Eliminar).IsRequired();
            builder.Property(x => x.Ejecutar).IsRequired();

            builder.Ignore(x => x.Mascara);

        }
    }
}
