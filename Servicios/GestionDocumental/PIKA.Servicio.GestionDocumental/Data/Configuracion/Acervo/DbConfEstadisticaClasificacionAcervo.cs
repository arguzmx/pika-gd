using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
  public  class DbConfEstadisticaClasificacionAcervo : IEntityTypeConfiguration<EstadisticaClasificacionAcervo>
    {
        public void Configure(EntityTypeBuilder<EstadisticaClasificacionAcervo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaEstadisticaClasificacionAcervo);
            builder.HasKey(x =>new { x.ArchivoId,x.CuadroClasificacionId,x.EntradaClasificacionId});
            builder.Property(x => x.ArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.CuadroClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EntradaClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ConteoActivos).IsRequired();
            builder.Property(x => x.ConteoActivosEliminados).IsRequired();
            builder.Property(x => x.FechaMinApertura).IsRequired(false);
            builder.Property(x => x.FechaMaxCierre).IsRequired(false);


        }


    }
}