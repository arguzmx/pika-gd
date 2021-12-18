using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Reportes;
using PIKA.Servicio.Reportes.Data.Configuracion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Reportes.Data
{

    public class DbContextReportesFactory : IFabricaContexto<DbContextReportes>
    {

        private IProveedorOpcionesContexto<DbContextReportes> proveedorOpciones;
        public DbContextReportesFactory(IProveedorOpcionesContexto<DbContextReportes> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextReportes Crear()
        {
            return new DbContextReportes(proveedorOpciones.ObtieneOpciones());
        }
    }
    public class DbContextReportes : DbContext, IRepositorioInicializable
    {
        public DbContextReportes(DbContextOptions options)
     : base(options)
        {
        }

        #region Constantes de configuración
        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Cuadro clasificacion
        /// </summary>
        public static string TablaReporteEntidad { get => "repo$reporteentidad"; }

        #endregion

        /// <summary>
        /// Reportes 
        /// </summary>
        public DbSet<ReporteEntidad> ReporteEntidades { get; set; }

        public void AplicarMigraciones()
        {
            this.Database.Migrate();
        }

        public void Inicializar(string contentPath, bool generarDatosdemo)
        {
            InicializarDatos d = new InicializarDatos();
            d.Inicializar(this, contentPath, null);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration<ReporteEntidad>(new DbConfReporteEntidad());
        }

    }
}
