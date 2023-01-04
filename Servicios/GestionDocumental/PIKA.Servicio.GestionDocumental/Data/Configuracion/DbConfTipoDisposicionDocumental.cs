using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
   public  class DbConfTipoDisposicionDocumental: IEntityTypeConfiguration<TipoDisposicionDocumental>
    {
        public void Configure(EntityTypeBuilder<TipoDisposicionDocumental> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaTipoDisposicionDocumental);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.DominioId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.UOId).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.Nombre).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.HasIndex(x => x.DominioId);
        }
    }
}