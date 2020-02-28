using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Data;

namespace PIKA.Servicio.Organizacion
{
    public class DbContextOrganizacion : DbContext
    {
        public DbContextOrganizacion(DbContextOptions<DbContextOrganizacion> options)
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
        /// Nombre de la tabla para las entidades del tipo Pais
        /// </summary>
        public static string TablaPais { get => "org$pais"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo estado
        /// </summary>
        public static string TablaEstado { get => "org$estado"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Dirección postal
        /// </summary>
        public static string TablaDireccionesPortales { get => "org$direccion_postal"; }


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
        /// Direcciones postales individuales existentes en la aplicación
        /// </summary>
        public DbSet<DireccionPostal> DireccionesPostales { get; set; }


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
   

        /// <summary>
        /// catálogo de paises en la aplicación
        /// </summary>
        public DbSet<Pais> Paises { get; set; }


        /// <summary>
        /// Catálogo de estados de un país existentes en la aplicación
        /// </summary>
        public DbSet<Estado> Estados { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<Dominio>(new DbConfDominio());
            builder.ApplyConfiguration<UnidadOrganizacional>(new DbConfOUs());
            builder.ApplyConfiguration<Rol>(new DbConfRoles());
            builder.ApplyConfiguration<UsuariosRol>(new DBConfUsuarioRol());
            builder.ApplyConfiguration<Pais>(new DbConfPais());
            builder.ApplyConfiguration<Estado>(new DbConfEstado());
            builder.ApplyConfiguration<DireccionPostal>(new DbConfDpostales());
            
        }

    }

}

