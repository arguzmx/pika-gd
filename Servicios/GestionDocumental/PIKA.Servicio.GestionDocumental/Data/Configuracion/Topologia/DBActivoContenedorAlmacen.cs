using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DBActivoContenedorAlmacen :
       IEntityTypeConfiguration<ActivoContenedorAlmacen>
    {
        public void Configure(EntityTypeBuilder<ActivoContenedorAlmacen> builder)
        {

            builder.ToTable(DBContextGestionDocumental.TablaActivoContenedorAlmacen);
            builder.HasKey(x => new { x.ContenedorAlmacenId, x.ActivoId });
            builder.HasOne(x => x.ContenedorAlmacen).WithMany(y => y.Activos).HasForeignKey(z => z.ContenedorAlmacenId);

        }
    }

}
