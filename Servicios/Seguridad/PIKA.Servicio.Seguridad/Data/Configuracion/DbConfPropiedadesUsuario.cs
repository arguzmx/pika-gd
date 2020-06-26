
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Data.Configuracion
{
    public class DbConfPropiedadesUsuario : IEntityTypeConfiguration<PropiedadesUsuario>
    {
        public void Configure(EntityTypeBuilder<PropiedadesUsuario> builder)
        {
            builder.ToTable(DbContextSeguridad.TablaPropiedadesUsuario);
            builder.HasKey(x => x.UsuarioId);

            builder.Property(x => x.username).IsRequired(true).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.email).IsRequired(false).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.name).IsRequired(false).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.family_name).IsRequired(false).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.given_name).IsRequired(false).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.middle_name).IsRequired(false).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.nickname).IsRequired(false).HasMaxLength(LongitudDatos.Nombre);
            builder.Property(x => x.updated_at).IsRequired(false);
            builder.Property(x => x.email_verified).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.UsuarioId).IsRequired().HasMaxLength(255).ValueGeneratedNever();
            builder.Property(x => x.generoid).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.paisid).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.estadoid).IsRequired(false).HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.gmt).IsRequired(false).HasMaxLength(255);
            builder.Property(x => x.gmt_offset).IsRequired(false);
            builder.Property(x => x.Ultimoacceso).IsRequired(false);

            builder.HasOne(x => x.Usuario).WithOne(y => y.Propiedades).HasForeignKey<PropiedadesUsuario>(z => z.UsuarioId);
            builder.HasOne(x => x.genero).WithMany(y => y.PropiedadesUsuario).HasForeignKey(z => z.generoid);

        }
    }
}
