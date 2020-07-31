using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
   public class DbConfValoracionEntradaClasificacion: IEntityTypeConfiguration<ValoracionEntradaClasificacion>
    {
        public void Configure(EntityTypeBuilder<ValoracionEntradaClasificacion> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaValoracionEntradaClasificacion);
            builder.HasKey(x => new { x.EntradaClasificacionId, x.TipoValoracionDocumentalId });
           
            builder.Property(x => x.EntradaClasificacionId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoValoracionDocumentalId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();

            builder.HasOne(x => x.TipoValoracionDocumental).WithMany(y => y.ValoracionEntradas).HasForeignKey(z=>z.TipoValoracionDocumentalId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}