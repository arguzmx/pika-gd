using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Temas;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfActivoSeleccionado : IEntityTypeConfiguration<ActivoSeleccionado>
    {
        public void Configure(EntityTypeBuilder<ActivoSeleccionado> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaActivoSelecionado);
            builder.HasKey(x => new { x.Id, x.UsuarioId, x.TemaId });
            builder.Property(x => x.TemaId).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.UsuarioId).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID);
            builder.HasOne(x => x.Activo).WithMany(y => y.ActivosSeleccionados).
                HasForeignKey(z => z.Id).OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class DbConfTemaActivoSeleccionado : IEntityTypeConfiguration<TemaActivos>
    {
        public void Configure(EntityTypeBuilder<TemaActivos> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaTemasActivos);
            builder.HasKey(x => new { x.Id });
            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.UsuarioId).IsRequired(true).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.Nombre).IsRequired(true).HasMaxLength(LongitudDatos.Nombre);
            builder.HasIndex(x => x.UsuarioId);
            builder.HasMany(x => x.ActivosSeleccionados).WithOne(x => x.TemaActivos)
                .HasForeignKey(z => z.TemaId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}