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
            builder.HasKey(x =>new { x.ArchivoId,x.CuadroClasificacionId,x.UnidadAdministrativaArchivoId, x.EntradaClasificacionId});
            builder.Property(x => x.ArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.UnidadAdministrativaArchivoId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.CuadroClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.EntradaClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ConteoActivos).IsRequired();
            builder.Property(x => x.ConteoActivosEliminados).IsRequired();
            builder.Property(x => x.FechaMinApertura).IsRequired(false);
            builder.Property(x => x.FechaMaxCierre).IsRequired(false);

            builder.HasOne(x=>x.Archivo).WithMany(y=>y.EstadisticasClasificacionAcervo).HasForeignKey(z=>z.ArchivoId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.CuadroClasificacion).WithMany(y => y.EstadisticasClasificacionAcervo).HasForeignKey(z => z.CuadroClasificacionId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.EntradaClasificacion).WithMany(y => y.EstadisticasClasificacionAcervo).HasForeignKey(z => z.EntradaClasificacionId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.UnidadAdministrativaArchivo).WithMany(y => y.EstadisticasClasificacionAcervo).HasForeignKey(z => z.UnidadAdministrativaArchivoId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}