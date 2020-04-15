using Microsoft.EntityFrameworkCore;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Seguridad.Data.Configuracion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Seguridad
{
    public class DbContextSeguridadFactory : IFabricaContexto<DbContextSeguridad>
    {

        private IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones;
        public DbContextSeguridadFactory(IProveedorOpcionesContexto<DbContextSeguridad> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextSeguridad Crear()
        {
            //var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(t);
            //var optionsBuilder = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType);
            //optionsBuilder.UseMySql(Configuration.GetConnectionString("pika-gd"));
            //var dbContext = (DbContext)Activator.CreateInstance(t, optionsBuilder.Options);

            return new DbContextSeguridad(proveedorOpciones.ObtieneOpciones());
        }
    }
    public class DbContextSeguridad : DbContext, IRepositorioInicializable
    {
        public DbContextSeguridad(DbContextOptions options)
        : base(options)
        {
        }



        #region Constantes de configuracion

        /// <summary>
        /// Nombre de la tabla para las entidades del Aplicacion
        /// </summary>
        public static string TablaAplicacion{ get => "seguridad$aplicacion"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del ModuloAplicacion
        /// </summary>
        public static string TablaModuloAplicacion { get => "seguridad$moduloaplicacion"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del TipoAdministradorModulo
        /// </summary>
        public static string TablaTipoAdministradorModulo { get => "seguridad$tipoadministradormodulo"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del TraduccionAplicacionModulo
        /// </summary>
        public static string TablaTraduccionAplicacionModulo { get => "seguridad$traduccionaplicacionmodulo"; }


        #endregion

        /// <summary>
        /// Dominios existentes en la aplicación
        /// </summary>
        public DbSet<Aplicacion> Aplicaciones { get; set; }
        /// <summary>
        /// Dominios existentes en la ModuloAplicacion
        /// </summary>
        public DbSet<ModuloAplicacion> ModuloAplicacion { get; set; }
        /// <summary>
        /// Dominios existentes en la TipoAdministradorModulo
        /// </summary>
        public DbSet<TipoAdministradorModulo> TipoAdministradorModulo { get; set; }
        /// <summary>
        /// Dominios existentes en la TraduccionAplicacionModulo
        /// </summary>
        public DbSet<TraduccionAplicacionModulo> TraduccionAplicacionModulo { get; set; }

        public void AplicarMigraciones()
        {
            this.Database.Migrate();
        }

        public void Inicializar(string ContentPath)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<Aplicacion>(new DbConfAplicacion());
            builder.ApplyConfiguration<ModuloAplicacion>(new DbConfModuloAplicacion());
            builder.ApplyConfiguration<TipoAdministradorModulo>(new DbConfTipoAdministradorModulo());
            builder.ApplyConfiguration<TraduccionAplicacionModulo>(new DbConfTraduccionAplicacionModulo());

        }
       
    }
}
