
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Data;
using PIKA.Servicio.Contenido.Data.Configuracion;
using Version = PIKA.Modelo.Contenido.Version;
namespace PIKA.Servicio.Contenido
{
    public class DbContextContenidoFactory : IFabricaContexto<DbContextContenido>
    {

        private IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones;
        public DbContextContenidoFactory(IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextContenido Crear()
        {
            return new DbContextContenido(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextContenido : DbContext, IRepositorioInicializable
    {
        public DbContextContenido(DbContextOptions options)
       : base(options)
        {
        }


        #region Contantes de configutación


        /// <summary>
        /// 
        /// </summary>
        public static string TablaElemento { get => "Contenido$elemento"; }


        /// <summary>
        ///     
        /// </summary>
        public static string TablaParte { get => "Contenido$parte"; }


        /// <summary>
        ///     
        /// </summary>
        public static string TablaTipoGestorEs { get => "Contenido$tipogestores"; }


        /// <summary>
        /// 
        /// </summary>
        public static string TablaVersionElemento { get => "Contenido$versionelemento"; }


        /// <summary>
        ///     
        /// </summary>
        public static string TablaVolumen { get => "Contenido$volumen"; }


        #endregion


        /// <summary>
        ///     
        /// </summary>
        public DbSet<Elemento> Elemento { get; set; }


        /// <summary>
        ///     
        /// </summary>
        public DbSet<Parte> Parte { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public DbSet<TipoGestorES> TipoGestorES { get; set; }


        /// <summary>
        ///     
        /// </summary>
        public DbSet<Version> Version { get; set; }

        /// <summary>
        ///     
        /// </summary>
        public DbSet<Volumen> Volumen { get; set; }

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
            builder.ApplyConfiguration<Elemento>(new DbConfElemento());
            builder.ApplyConfiguration<Parte>(new DbConfParte());
            builder.ApplyConfiguration<TipoGestorES>  (new DbConfTipoGestorES());
            builder.ApplyConfiguration<Version>  (new DbConfVersion());
            builder.ApplyConfiguration<Volumen>  (new DbConfVolumen());

        }

    }

}

