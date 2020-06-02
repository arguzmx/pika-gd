
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Aplicacion.Plugins;
using PIKA.Servicio.AplicacionPlugin.Data;
using PIKA.Servicio.AplicacionPlugin.Data.Configuracion;

namespace PIKA.Servicio.AplicacionPlugin
{
    public class DbContextAplicacionPluginFactory : IFabricaContexto<DbContextAplicacionPlugin>
    {

        private IProveedorOpcionesContexto<DbContextAplicacionPlugin> proveedorOpciones;
        public DbContextAplicacionPluginFactory(IProveedorOpcionesContexto<DbContextAplicacionPlugin> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextAplicacionPlugin Crear()
        {
            return new DbContextAplicacionPlugin(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextAplicacionPlugin : DbContext, IRepositorioInicializable
    {
        public DbContextAplicacionPlugin(DbContextOptions options)
       : base(options)
        {
        }


        #region Contantes de configutación


        /// <summary>
        /// Nombre de la tabla para las entidades de plugin
        /// </summary>
        public static string TablaPlugin { get => "aplicacion$plugin"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Plugin Instalado
        /// </summary>
        public static string TablaPluginInstalado { get => "aplicacion$plugininstalado"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del VersionPlugin
        /// </summary>
        public static string TablaVersionPlugin { get => "aplicacion$versionplugin"; }


       

        #endregion


        /// <summary>
        /// Aplicacion existentes en la aplicación
        /// </summary>
        public DbSet<Plugin> Plugin { get; set; }


        /// <summary>
        /// Modulos Aplicaciones existentes en la PluginInstalado
        /// </summary>
        public DbSet<PluginInstalado> PluginInstalado { get; set; }



        /// <summary>
        /// VersionPlugin existentes en la aplicación
        /// </summary>
        public DbSet<VersionPlugin> VersionPlugin { get; set; }



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
            builder.ApplyConfiguration<VersionPlugin>  (new DbConfVersionPlugin());

        }

    }

}

