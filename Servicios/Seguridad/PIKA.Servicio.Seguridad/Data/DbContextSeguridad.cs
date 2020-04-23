
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Seguridad.Data;
using PIKA.Servicio.Seguridad.Data.Configuracion;

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
            return new DbContextSeguridad(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextSeguridad : DbContext, IRepositorioInicializable
    {
        public DbContextSeguridad(DbContextOptions options)
       : base(options)
        {
        }


        #region Contantes de configutación


        /// <summary>
        /// Nombre de la tabla para las entidades de Aplicaciones
        /// </summary>
        public static string TablaAplicacion { get => "seguridad$aplicacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Modulo de Aplicaciones
        /// </summary>
        public static string TablaModuloAplicacion { get => "seguridad$moduloaplicacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Tipo de Administrador de Modulo
        /// </summary>
        public static string TablaTipoAdministradorModulo { get => "seguridad$tipoadministradormodulo"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Traduccion Aplicacion Modulo
        /// </summary>
        public static string TablaTraduccionAplicacionModulo { get => "seguridad$traduccionaplicacionmodulo"; }




        #endregion


        /// <summary>
        /// Aplicacion existentes en la aplicación
        /// </summary>
        public DbSet<Aplicacion> Aplicacion { get; set; }


        /// <summary>
        /// Modulos Aplicaciones existentes en la aplicación
        /// </summary>
        public DbSet<ModuloAplicacion> ModuloAplicacion { get; set; }



        /// <summary>
        /// Traduccion Aplicaion modulo existentes en la aplicación
        /// </summary>
        public DbSet<TraduccionAplicacionModulo> TraduccionAplicacionModulo { get; set; }


        /// <summary>
        /// Tipo Administracion Modulo existentes en la aplicación
        /// </summary>
        public DbSet<TipoAdministradorModulo> TipoAdministradorModulo { get; set; }

        public void AplicarMigraciones()
        {
            this.Database.Migrate();
        }

        public void Inicializar(string ContentPath)
        {
            InicializarDatos.Inicializar(this, ContentPath);
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

