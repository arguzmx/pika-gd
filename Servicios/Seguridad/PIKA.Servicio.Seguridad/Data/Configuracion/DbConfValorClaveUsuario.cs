using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Seguridad;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    class DbConfValorClaveUsuario : IEntityTypeConfiguration<ValorClaveUsuario>
    {
        public void Configure(EntityTypeBuilder<ValorClaveUsuario> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaValorClaveUsuario);
            builder.HasKey(x => new { x.Id });
            builder.Property(x => x.Id).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.UsuarioId).HasMaxLength(255).IsRequired();
            builder.Property(x => x.Clave).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.Valor).IsRequired(false).HasColumnType("TEXT");
            builder.HasIndex(x => x.UsuarioId);
            builder.HasOne(x => x.Usuario).WithMany(y => y.ValoresClave).HasForeignKey(z => z.UsuarioId);
        }
    }
}
