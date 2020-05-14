using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Data.Configuracion
{
  public  class DbConfTipoGestorES  : IEntityTypeConfiguration<TipoGestorES>
    {
        public void Configure(EntityTypeBuilder<TipoGestorES> builder)
        {
            builder.ToTable(DbContextContenido.TablaTipoGestorEs);
            builder.HasKey(x =>x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x=>x.Volumenid).HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Eliminada).HasDefaultValue(false).IsRequired();

            builder.HasMany(x => x.Volumenes).WithOne(y=>y.TipoGestorES).HasForeignKey(z=>z.TipoGestorESId);

        }
    }
}
