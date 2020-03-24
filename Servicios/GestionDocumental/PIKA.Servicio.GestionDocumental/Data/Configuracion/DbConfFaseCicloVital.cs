using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfFaseCicloVital : IEntityTypeConfiguration<FaseCicloVital>
    {
        public void Configure(EntityTypeBuilder<FaseCicloVital> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaFasesCicloVital);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
        }
    }
}
