using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Data;
using RepositorioEntidades;

namespace PIKA.Servicio.Organizacion
{
    public class DbContextOrganizacionFactory : IFabricaContexto<DbContextOrganizacion>
    {

        private IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones;
        public DbContextOrganizacionFactory(IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextOrganizacion Crear()
        {
            return new DbContextOrganizacion(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextOrganizacion : DbContext, IRepositorioInicializable
    {
        public DbContextOrganizacion(DbContextOptions options)
       : base(options)
        {
        }


        #region Contantes de configutación


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Dominio
        /// </summary>
        public static string TablaDominio { get => "org$dominio"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del unidad organizacional
        /// </summary>
        public static string TablaOU { get => "org$ou"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo rol
        /// </summary>
        public static string TablaRoles { get => "org$rol"; }


                /// <summary>
        /// Nombre de la tabla para las entidades de la relación usuarios-roles
        /// </summary>
        public static string TablaUsuariosRol { get => "org$usuarios_rol"; }

        #endregion
        

        /// <summary>
        /// Dominios existentes en la aplicación
        /// </summary>
        public DbSet<Dominio> Dominios { get; set; }


        /// <summary>
        /// Unidades organizacionales existentes en la aplicación
        /// </summary>
        public DbSet<UnidadOrganizacional> UnidadesOrganizacionales { get; set; }


        /// <summary>
        /// Roles existentes en la aplicación
        /// </summary>
        public DbSet<Rol> Roles { get; set; }


        /// <summary>
        /// Relación múltiple usuario-rol en la aplicación
        /// </summary>
        public DbSet<UsuariosRol> UsuariosRoles { get; set; }


        /// <summary>
        /// Usuarios existentes en la aplicación
        /// </summary>

        public void AplicarMigraciones()
        {
            this.Database.Migrate();
        }

        public void Inicializar(string ContentPath, bool generarDatosdemo)
        {
            InicializarDatos.Inicializar(this, ContentPath, generarDatosdemo);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<Dominio>(new DbConfDominio());
            builder.ApplyConfiguration<UnidadOrganizacional>(new DbConfOUs());
            builder.ApplyConfiguration<Rol>(new DbConfRoles());
            builder.ApplyConfiguration<UsuariosRol>(new DBConfUsuarioRol());
            
        }

    }

}

