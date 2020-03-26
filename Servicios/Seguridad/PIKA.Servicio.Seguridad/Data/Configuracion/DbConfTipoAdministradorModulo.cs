using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
   public class DbConfTipoAdministradorModulo : IEntityTypeConfiguration<TipoAdministradorModulo>
    {
        public void Configure(EntityTypeBuilder<TipoAdministradorModulo> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaTipoAdministradorModulo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Ignore(x => x.TiposAdministrados);
            builder.Property(x => x.AplicacionId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ModuloId).HasMaxLength(LongitudDatos.GUID).IsRequired();


        }
    }
}
