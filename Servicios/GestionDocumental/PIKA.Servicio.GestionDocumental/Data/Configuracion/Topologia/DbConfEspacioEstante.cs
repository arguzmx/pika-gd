using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental.Topologia;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEspacioEstante : IEntityTypeConfiguration<EspacioEstante>
    {
        public void Configure(EntityTypeBuilder<EspacioEstante> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEspaciosEstante);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EstanteId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.CodigoOptico).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.Posicion).IsRequired();

            builder.HasOne(x => x.Estante).WithMany(y => y.Espacios).HasForeignKey(x => x.EstanteId);
        }
    }
}
