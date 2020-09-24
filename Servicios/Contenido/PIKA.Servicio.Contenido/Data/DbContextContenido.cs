
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using RepositorioEntidades;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Data;
using PIKA.Servicio.Contenido.Data.Configuracion;

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

        private const string esquema = "contenido$";

        #region Contantes de configutación

        public static string TablaGestorSMB { get => $"{esquema}gestorsmb"; }
        public static string TablaGestorLocal { get => $"{esquema}gestorlocal"; }

        public static string TablaGestorAzure { get => $"{esquema}gestorazure"; }


        public static string TablaPuntoMontaje { get => $"{esquema}pmontaje"; }

        public static string TablaPermisos { get => $"{esquema}permiso"; }
        
        public static string TablaDestinatariosPermisos { get => $"{esquema}destinatariopermiso"; }

        public static string TablaCarpeta { get => $"{esquema}carpeta"; }

        public static string TablaElemento { get => $"{esquema}elemento"; }
        public static string TablaParte { get => $"{esquema}versionpartes"; }

        public static string TablaTipoGestorEs { get => $"{esquema}tipogestores"; }

        public static string TablaVersionElemento { get => $"{esquema}versionelemento"; }

        public static string TablaVolumen { get => $"{esquema}volumen"; }

        public static string TablaVolumenPuntoMontaje { get => $"{esquema}puntomontajevolumen"; }

        public static string TablaElementoTransaccionContenido { get => $"{esquema}ElementoTransaccionContenido"; }

        #endregion

        /// <summary>
        /// Permisos de acceso sobre el contenido
        /// </summary>
        public DbSet<Permiso> Permisos { get; set; }

        /// <summary>
        /// Destinatarios asociados a un permiso de accesos
        /// </summary>
        public DbSet<DestinatarioPermiso> DestinatariosPermisos { get; set; }

        /// <summary>
        /// Tipos de ssistema de archivos para el almacenamiento en volumenes
        /// </summary>
        public DbSet<TipoGestorES> TipoGestorES { get; set; }

        /// <summary>
        /// Volumen de almacenamiento de contenidos en sistemas de archivos
        /// </summary>
        public DbSet<Volumen> Volumen { get; set; }

        /// <summary>
        /// Puntos de montaje disponibles para la creación de carpetas
        /// </summary>
        public DbSet<PuntoMontaje> PuntosMontaje { get; set; }

        public DbSet<VolumenPuntoMontaje> VolumenPuntosMontaje { get; set; }

        /// <summary>
        /// Elemento de estructura jerárquica parala organización del contenido
        /// </summary>
        public DbSet<Carpeta> Carpetas { get; set; }


        /// <summary>
        /// Elemento de contenido
        /// </summary>
        public DbSet<Elemento> Elemento { get; set; }
        
        /// <summary>
        /// Versiones de elemntos de contenido
        /// </summary>
        public DbSet<PIKA.Modelo.Contenido.Version> Version { get; set; }

        /// <summary>
        /// Partes que componen una versión para un  elemento de contenidp
        /// </summary>
        public DbSet<Parte> Partes { get; set; }



        /// <summary>
        /// Configuración del gestor de E/S Azure
        /// </summary>
        public DbSet<GestorAzureConfig> GestorAzureConfig { get; set; }

        /// <summary>
        /// Configuración del gestor de E/S Local
        /// </summary>
        public DbSet<GestorLocalConfig> GestorLocalConfig { get; set; }

        /// <summary>
        /// Configuración del gestor de E/S SMB
        /// </summary>
        public DbSet<GestorSMBConfig> GestorSMBConfig { get; set; }


        public DbSet<ElementoTransaccionCarga> ElementosTransaccionCarga { get; set; }


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
            builder.ApplyConfiguration<Permiso>(new DbConfPermiso());
            builder.ApplyConfiguration<DestinatarioPermiso>(new DbConfDestinatarioPermiso());
            builder.ApplyConfiguration<TipoGestorES>(new DbConfTipoGestorES());
            builder.ApplyConfiguration<Volumen>(new DbConfVolumen());
            builder.ApplyConfiguration<PuntoMontaje>(new DbConfPuntoMontaje());
            builder.ApplyConfiguration<Carpeta>(new DbConfCarpeta());
            builder.ApplyConfiguration<Elemento>(new DbConfElemento());
            builder.ApplyConfiguration<PIKA.Modelo.Contenido.Version>(new DbConfVersion());
            builder.ApplyConfiguration<Parte>(new DbConfParte());
            builder.ApplyConfiguration<VolumenPuntoMontaje>(new DbConfVolumenPuntoMontaje());
            builder.ApplyConfiguration<GestorLocalConfig>(new DbConfigGestorLocal());
            builder.ApplyConfiguration<GestorSMBConfig>(new DbConfigGestorSMB());
            builder.ApplyConfiguration<GestorAzureConfig>(new DbConfigGestorAzure());
            builder.ApplyConfiguration<ElementoTransaccionCarga>(new DbConfElementoTransaccionCarga());

        }

    }

}

