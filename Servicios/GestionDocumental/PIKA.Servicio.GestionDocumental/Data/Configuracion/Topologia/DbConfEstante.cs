using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental.Topologia;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfEstante : IEntityTypeConfiguration<Estante>
    {
        public void Configure(EntityTypeBuilder<Estante> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEstantes);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.AlmacenArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.CodigoOptico).HasMaxLength(2048).IsRequired(false);
            builder.Property(x => x.CodigoElectronico).HasMaxLength(2048).IsRequired(false);

            builder.HasOne(x => x.Almacen).WithMany(y => y.Estantes).HasForeignKey(z => z.AlmacenArchivoId);
            builder.HasMany(x => x.Espacios).WithOne(y => y.Estante).HasForeignKey(x => x.EstanteId);

        }
    }
}
