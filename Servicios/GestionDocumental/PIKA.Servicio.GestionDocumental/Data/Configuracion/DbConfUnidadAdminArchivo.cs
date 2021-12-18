using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfUnidadAdminArchivo : IEntityTypeConfiguration<UnidadAdministrativaArchivo>
    {
        public void Configure(EntityTypeBuilder<UnidadAdministrativaArchivo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaUnidadAdministrativaArchivo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.UnidadAdministrativa).HasMaxLength(LongitudDatos.Nombre).IsRequired();
            builder.Property(x => x.AreaProcedenciaArchivo).HasMaxLength(LongitudDatos.Nombre).IsRequired(false); 
            builder.Property(x => x.Responsable).HasMaxLength(LongitudDatos.Nombre).IsRequired(false); 
            builder.Property(x => x.Cargo).HasMaxLength(LongitudDatos.Nombre).IsRequired(false); 
            builder.Property(x => x.Domicilio).HasMaxLength(LongitudDatos.Nombre).IsRequired(false); 
            builder.Property(x => x.Telefono).HasMaxLength(LongitudDatos.Nombre).IsRequired(false); 
            builder.Property(x => x.Email).HasMaxLength(LongitudDatos.Nombre).IsRequired(false);
            builder.Property(x => x.OrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.TipoOrigenId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.ArchivoConcentracionId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.ArchivoHistoricoId).HasMaxLength(LongitudDatos.GUID).IsRequired(false);
            builder.Property(x => x.ArchivoTramiteId).HasMaxLength(LongitudDatos.GUID).IsRequired(true);
            builder.Property(x => x.UbicacionFisica).HasMaxLength(LongitudDatos.NombreLargo).IsRequired(false);

            builder.HasMany(x => x.Activos).WithOne(y => y.UnidadAdministrativa).HasForeignKey(z => z.UnidadAdministrativaArchivoId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(x => x.Permisos).WithOne(y => y.UnidadAdministrativaArchivo).HasForeignKey(z => z.UnidadAdministrativaArchivoId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
