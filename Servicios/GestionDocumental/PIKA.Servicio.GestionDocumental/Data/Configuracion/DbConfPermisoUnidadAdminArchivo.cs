using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Configuracion
{
    public class DbConfPermisoUnidadAdminArchivo : IEntityTypeConfiguration<PermisosUnidadAdministrativaArchivo>
    {
        public void Configure(EntityTypeBuilder<PermisosUnidadAdministrativaArchivo> builder)
        {
            builder.ToTable(DBContextGestionDocumental.TablaPermisoUnidadAdministrativaArchivo);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever().HasMaxLength(LongitudDatos.GUID);
            builder.Property(x => x.UnidadAdministrativaArchivoId).HasMaxLength(LongitudDatos.GUID).IsRequired();
            builder.Property(x => x.DestinatarioId).HasMaxLength(LongitudDatos.GUID).IsRequired(); 
            builder.Property(x => x.CrearAcervo).IsRequired(true);
            builder.Property(x => x.ActualizarAcervo).IsRequired(true);
            builder.Property(x => x.ElminarAcervo).IsRequired(true);
            builder.Property(x => x.ElminarAcervo).IsRequired(true);
            builder.Property(x => x.LeerAcervo).IsRequired(true);

        }
    }
}
