using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEventoContenedoAlmacen : IEntityTypeConfiguration<EventoContenedorAlmacen>
    {
        public void Configure(EntityTypeBuilder<EventoContenedorAlmacen> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEventoContenedorAlmacen);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.ProcesoId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.UsuarioId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.ContenedorAlmacenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EsAccionUsuario).IsRequired();

            builder.HasIndex(x => x.ContenedorAlmacenId);

            builder.Property(x => x.Payload).HasMaxLength(LongitudDatos.PAYLOAD_EVENTO).IsRequired();
        }
    }
}
