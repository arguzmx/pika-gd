using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfTipoArchivo : IEntityTypeConfiguration<TipoArchivo>
    {
        public void Configure(EntityTypeBuilder<TipoArchivo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaTiposArchivo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);

            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();

            

        }
    }
}
