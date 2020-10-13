
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfigUsuariosDominio : IEntityTypeConfiguration<UsuarioDominio>
    {
        public void Configure(EntityTypeBuilder<UsuarioDominio> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaUsuariosOominio);
            builder.HasKey(x => new  { x.ApplicationUserId, x.DominioId, x.UnidadOrganizacionalId });

            builder.Property(x => x.ApplicationUserId).HasMaxLength(255).IsRequired();
            builder.Property(x => x.DominioId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.UnidadOrganizacionalId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EsAdmin).IsRequired();

        }
    }
}
