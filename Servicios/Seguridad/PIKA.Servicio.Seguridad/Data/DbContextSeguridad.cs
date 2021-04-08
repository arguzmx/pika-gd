
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;
using PIKA.Infraestructura.Comun;
using PIKA.Servicio.Seguridad.Data;
using PIKA.Servicio.Seguridad.Data.Configuracion;
using PIKA.Modelo.Seguridad;
using PIKA.Modelo.Seguridad.Base;
using PIKA.Infraestructura.Comun.Seguridad;

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

        public static string TablaSeguridad= "seguridad$";

        public static string TablaApplicationUser { get => "AspNetUsers"; }

        public static string TablaApplicationUserClaims { get => "aspnetuserclaims"; }

        public static string TablaUsuariosOominio { get => $"{TablaSeguridad}usuariosdominio"; }

        public static string TablaPropiedadesUsuario { get => $"{TablaSeguridad}usuarioprops"; }

        public static string TablaGeneros{ get => $"{TablaSeguridad}generousuario"; }

        public static string TablaPermisosApp { get => $"{TablaSeguridad}permisosapl"; }



        /// <summary>
        /// Nombre de la tabla para las entidades de Aplicaciones
        /// </summary>
        public static string TablaAplicacion { get => $"{TablaSeguridad}aplicacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Modulo de Aplicaciones
        /// </summary>
        public static string TablaModuloAplicacion { get => $"{TablaSeguridad}moduloaplicacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Tipo de Administrador de Modulo
        /// </summary>
        public static string TablaTipoAdministradorModulo { get => $"{TablaSeguridad}tipoadministradormodulo"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del Traduccion Aplicacion Modulo
        /// </summary>
        public static string TablaTraduccionAplicacionModulo { get => $"{TablaSeguridad}traduccionaplicacionmodulo"; }




        #endregion


        /// <summary>
        /// Aplicacion existentes en la aplicación
        /// </summary>
        public DbSet<Aplicacion> Aplicacion { get; set; }


        /// <summary>
        /// usaurios de la aplciación
        /// </summary>
        public DbSet<ApplicationUser> Usuarios { get; set; }

        /// <summary>
        ///  erlacionas los claims para OIDC con el usuario
        /// </summary>
        public DbSet<UserClaim> ClaimsUsuario { get; set; }

        /// <summary>
        ///  Generos para los usaurios del sistema
        /// </summary>
        public DbSet<Genero> Generos { get; set; }

        /// <summary>
        /// Propieades de los usuarios del dominio
        /// </summary>
        public DbSet<PropiedadesUsuario> PropiedadesUsuario { get; set; }



        /// <summary>
        /// Registro de los usuarios exietntes en un dominio
        /// </summary>
        public DbSet<UsuarioDominio> UsuariosDominio { get; set; }

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

        public DbSet<PermisoAplicacion> PermisosAplicacion { get; set; }


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
            builder.ApplyConfiguration<Aplicacion>(new DbConfAplicacion());
            builder.ApplyConfiguration<PermisoAplicacion>(new DbConfPermisosAplicacion());
            builder.ApplyConfiguration<ApplicationUser>(new DBConfigApplicationUser());
            builder.ApplyConfiguration<UsuarioDominio>(new DbConfigUsuariosDominio());
            builder.ApplyConfiguration<UserClaim>(new DbConfigUserClaims());
            builder.ApplyConfiguration<Genero>(new DbConfiggeneros());
            builder.ApplyConfiguration<PropiedadesUsuario>(new DbConfPropiedadesUsuario());
            builder.ApplyConfiguration<ModuloAplicacion>(new DbConfModuloAplicacion());
            builder.ApplyConfiguration<TipoAdministradorModulo>(new DbConfTipoAdministradorModulo());
            builder.ApplyConfiguration<TraduccionAplicacionModulo>(new DbConfTraduccionAplicacionModulo());

        }

    }

}

