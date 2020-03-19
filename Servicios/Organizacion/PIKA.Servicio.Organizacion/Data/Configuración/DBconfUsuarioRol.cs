using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion.Data
{
    public class DBConfUsuarioRol :
       IEntityTypeConfiguration<UsuariosRol>
    {
        public void Configure(EntityTypeBuilder<UsuariosRol> builder)
        {

            builder.ToTable(DbContextOrganizacion.TablaUsuariosRol);
            builder.HasKey(x => new { x.ApplicationUserId, x.RolId });

            builder.HasOne(x => x.Rol).WithMany(y => y.UsuariosRol).HasForeignKey(z => z.RolId);
   

        }
    }

}
