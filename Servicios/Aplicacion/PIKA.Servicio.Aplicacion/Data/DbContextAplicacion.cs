
using Microsoft.EntityFrameworkCore;
using RepositorioEntidades;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Servicio.AplicacionPlugin.Data;
using PIKA.Servicio.AplicacionPlugin.Data.Configuracion;
using PIKA.Modelo.Aplicacion.Tareas;
using System;

namespace PIKA.Servicio.AplicacionPlugin
{
    public class DbContextAplicacionFactory : IFabricaContexto<DbContextAplicacion>
    {

        private IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones;
        public DbContextAplicacionFactory(IProveedorOpcionesContexto<DbContextAplicacion> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextAplicacion Crear()
        {
            return new DbContextAplicacion(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextAplicacion : DbContext, IRepositorioInicializable
    {
        public DbContextAplicacion(DbContextOptions options)
       : base(options)
        {
        }


        #region Contantes de configutación

        private const string prefijo = "aplicacion";

        /// <summary>
        /// Nombre de la tabla para las entidades de plugin
        /// </summary>
        public static string TablaPlugin { get => $"{prefijo}$plugin"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Plugin Instalado
        /// </summary>
        public static string TablaPluginInstalado { get => $"{prefijo}$plugininstalado"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del VersionPlugin
        /// </summary>
        public static string TablaVersionPlugin { get => $"{prefijo}$versionplugin"; }


        public static string TablaTareaAutomatica { get => $"{prefijo}$autotarea"; }
        public static string TablaBitacoraTarea { get => $"{prefijo}$bitacoratarea"; }

        public static string TablaTareasEnDemanda { get => $"{prefijo}$ondemandtarea"; }

        #endregion


        /// <summary>
        /// Plugin disponibles para la appp
        /// </summary>
        public DbSet<Plugin> Plugin { get; set; }


        /// <summary>
        /// Plugins instalados en la APP
        /// </summary>
        public DbSet<PluginInstalado> PluginInstalado { get; set; }


        /// <summary>
        /// VErsiones del plugin existentes en la aplicación
        /// </summary>
        public DbSet<VersionPlugin> VersionPlugin { get; set; }

        /// <summary>
        /// Tareas automáticas realizadas por el sistema y su programación
        /// </summary>
        public DbSet<TareaAutomatica> TareasAutomaticas { get; set; }
        
        /// <summary>
        /// Historial de ejecución de las tareas automáticas
        /// </summary>
        public DbSet<BitacoraTarea> BitacoraTareas { get; set; }


        public DbSet<ColaTareaEnDemanda> TareasEnDemanda { get; set; }

        public void AplicarMigraciones()
        {
            this.Database.Migrate();
        }

        public void Inicializar(string ContentPath, bool generarDatosdemo)
        {
            InicializarDatos.Inicializar(this, ContentPath);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<Plugin>(new DbConfPlugin());
            builder.ApplyConfiguration<PluginInstalado>(new DbConfPluginInstalado());
            builder.ApplyConfiguration<VersionPlugin>(new DbConfVersionPlugin());
            builder.ApplyConfiguration(new DbConfTareaAutomatica());
            builder.ApplyConfiguration(new DbConfBitacoraTarea());
            builder.ApplyConfiguration(new DbConfTareaEnDemanda());
        }

    }

}

