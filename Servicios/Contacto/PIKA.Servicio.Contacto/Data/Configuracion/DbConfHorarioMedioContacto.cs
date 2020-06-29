using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.Contacto;
using RepositorioEntidades;

namespace PIKA.Servicio.Contacto.Data.Configuracion
{
    public class DbConfHorarioMedioContacto :
       IEntityTypeConfiguration<HorarioMedioContacto>
    {
        public void Configure(EntityTypeBuilder<HorarioMedioContacto> builder)
        {

            builder.ToTable(DbContextContacto.TablaHorariosMedioContacto);
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.SinHorario).IsRequired(true).HasDefaultValue(false);
            builder.Property(x => x.DiaSemana).IsRequired(true).HasDefaultValue(0);
            builder.Property(x => x.Inicio).IsRequired(false);
            builder.Property(x => x.Fin).IsRequired(false);

            builder.HasOne(x => x.MedioContacto).WithMany(y => y.Horarios).HasForeignKey(z => z.MedioContactoId);
        }
    }

}
