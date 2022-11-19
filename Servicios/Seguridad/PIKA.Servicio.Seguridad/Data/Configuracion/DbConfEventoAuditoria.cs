using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfEventoAuditoria : IEntityTypeConfiguration<EventoAuditoria>
    {
        public void Configure(EntityTypeBuilder<EventoAuditoria> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaEventoAuditoria);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.EsError).IsRequired();
            builder.Property(x => x.Exitoso).IsRequired();
            builder.Property(x => x.UsuarioId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.DireccionRed).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.IdSesion).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.DominioId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.UAId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.FuenteEventoId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.ModuloId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoEvento).IsRequired();
            builder.Property(x => x.Texto).IsRequired(false).HasMaxLength(LongitudDatos.PAYLOAD_EVENTO);

            builder.HasIndex(i => new { i.DominioId, i.UAId});
            builder.HasIndex(i => new { i.UsuarioId, i.Fecha, i.FuenteEventoId, i.ModuloId });


        }
    
    }

    public class DbConfEventoAuditoriaActivo : IEntityTypeConfiguration<EventoAuditoriaActivo>
    {
        public void Configure(EntityTypeBuilder<EventoAuditoriaActivo> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaEventoAuditoriaActivo);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.UAId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.FuenteEventoId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.TipoEvento).IsRequired();
            builder.Property(x => x.ModuloId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Auditable).IsRequired();
            builder.Property(x => x.DominioId).IsRequired();
            builder.Property(x => x.UAId).IsRequired();
            builder.HasIndex(i => new { i.DominioId, i.UAId });

        }

    }

    public class DbConfTipoEventoAuditoria : IEntityTypeConfiguration<TipoEventoAuditoria>
    {
        public void Configure(EntityTypeBuilder<TipoEventoAuditoria> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaTipoEventoAuditoria);
            builder.HasKey(x => new { x.FuenteEventoId, x.TipoEvento, x.ModuloId } );

            builder.Property(x => x.ModuloId).IsRequired().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.FuenteEventoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.PlantillaEvento).IsRequired(false);
            builder.Property(x => x.Desripción).IsRequired().HasMaxLength(LongitudDatos.Descripcion);
        }

    }
}
