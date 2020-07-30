using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Topologia;
using PIKA.Servicio.GestionDocumental.Data.Configuracion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data
{
    public class DbContextGestionDocumentalFactory : IFabricaContexto<DBContextGestionDocumental>
    {

        private IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones;
        public DbContextGestionDocumentalFactory(IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DBContextGestionDocumental Crear()
        {
            return new DBContextGestionDocumental(proveedorOpciones.ObtieneOpciones());
        }
    }
    public class DBContextGestionDocumental : DbContext, IRepositorioInicializable
    {
        public DBContextGestionDocumental(DbContextOptions options)
     : base(options)
        {
        }


        #region Constantes de configuración


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Cuadro clasificacion
        /// </summary>
        public static string TablaCuadrosClasificacion { get => "gd$cuadroclasificacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades Elemento clasificacion
        /// </summary>
        public static string TablaElementosClasificacion { get => "gd$elementoclasificacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Estado cuadro clasificacion
        /// </summary>
        public static string TablaEstadosCuadro { get => "gd$estadocuadroclasificacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Archivo
        /// </summary>
        public static string TablaArchivos { get => "gd$archivo"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Tipo archivo
        /// </summary>
        public static string TablaTiposArchivo { get => "gd$tipoarchivo"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Fase ciclo vital
        /// </summary>
        public static string TablaFasesCicloVital { get => "gd$faseciclovital"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Activo
        /// </summary>
        public static string TablaActivos { get => "gd$activo"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Ampliación
        /// </summary>
        public static string TablaAmpliacion { get => "gd$ampliacion"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Tipo Ampliación
        /// </summary>
        public static string TablaTipoAmpliacion { get => "gd$tipoampliacion"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaAsuntos { get => "gd$asunto"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaHistorialArchivoActivos { get => "gd$historialarchivoactivo"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaPrestamos { get => "gd$prestamo"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaActivosPrestamo { get => "gd$activoprestamo"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaComentariosPrestamo { get => "gd$comentarioprestamo"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaAlmacenesArchivo { get => "gd$almacen"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaEstantes { get => "gd$estantes"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Asunto 
        /// </summary>
        public static string TablaEspaciosEstante { get => "gd$espacioestante"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Transferencia 
        /// </summary>
        public static string TablaTransferencias { get => "gd$transferencia"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo EstadoTransferencia 
        /// </summary>
        public static string TablaEstadosTransferencia { get => "gd$estadotransferencia"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo EventoTransferencia 
        /// </summary>
        public static string TablaEventosTransferencia { get => "gd$eventotransferencia"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo ComentarioTransferencia 
        /// </summary>
        public static string TablaComentariosTransferencia { get => "gd$comentariotransferencia"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo ActivoTransferencia 
        /// </summary>
        public static string TablaActivosTransferencia { get => "gd$activotransferencia"; }

        /// <summary>
        /// Nombre de la tabla para las entidades del tipo ActivoDeclinado
        /// </summary>
        public static string TablaActivosDeclinados { get => "gd$activodeclinado"; }
        /// <summary>
        /// Nombre de  la entrada de clasificacion
        /// </summary>
        public static string TablaEntradaClasificacion { get => "gd$entradaclasificacion"; }
        
        /// <summary>
        /// Nombre del tipo de disposicion documental 
        /// </summary>
       public static string TablaTipoDisposicionDocumental { get=> "gd$tipodisposiciondocumental"; }
        /// <summary>
        /// Nombre del  Valoracion Entrada de clasificacion
        /// </summary>
        public static string TablaValoracionEntradaClasificacion {get=> "gd$valoracionentradaclasificacion";}
        /// <summary>
        /// Nombre del tipo de Valoracion Entrada de clasificacion
        /// </summary>
        public static string TablaTipoValoracionDocumental { get => "gd$tipovaloraciondocumental"; }


        #endregion

        /// <summary>
        /// Cuadros clasificacion existentes en la aplicación
        /// </summary>
        public DbSet<ElementoClasificacion> CuadrosClasificacion { get; set; }

        /// <summary>
        /// Elementos de clasificacion en la aplicación
        /// </summary>
        public DbSet<ElementoClasificacion> ElementosClasificacion { get; set; }

        /// <summary>
        /// Estados cuadro clasfificacion en la aplicación
        /// </summary>
        public DbSet<EstadoCuadroClasificacion> EstadosCuadroClasificacion { get; set; }

        /// <summary>
        /// Archivos existentes en la aplicación
        /// </summary>
        public DbSet<Archivo> Archivos { get; set; }

        /// <summary>
        /// Tipos de archivo existentes en la aplicación
        /// </summary>
        public DbSet<TipoArchivo> TiposArchivo { get; set; }


        /// <summary>
        /// Activos existentes en la aplicación
        /// </summary>
        public DbSet<Activo> Activos { get; set; }

        /// <summary>
        /// asuntos existentes en la aplicación
        /// </summary>
        public DbSet<Asunto> Asuntos { get; set; }

        /// <summary>
        /// Activos existentes en la amplacion
        /// </summary>
        public DbSet<Ampliacion> Ampliaciones { get; set; }

        /// <summary>
        /// Activos existentes en la amplacion
        /// </summary>
        public DbSet<TipoAmpliacion> TiposAmpliaciones { get; set; }


        /// <summary>
        /// HistorialArchivosActivo existentes en la aplicación
        /// </summary>
        public DbSet<HistorialArchivoActivo> HistorialArchivosActivo { get; set; }

        /// <summary>
        /// Almacenes de archivo existentes en la aplicación
        /// </summary>
        public DbSet<AlmacenArchivo> AlmacenesArchivo { get; set; }
        /// <summary>
        /// Estantes existentes en la aplicación
        /// </summary>
        public DbSet<Estante> Estantes { get; set; }
        /// <summary>
        /// Espacios de estante de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<EspacioEstante> EspaciosEstante { get; set; }

        /// <summary>
        /// Prestamos existentes en la aplicación
        /// </summary>
        public DbSet<Prestamo> Prestamos { get; set; }

        /// <summary>
        /// Activos del prestamo existentes en la aplicación
        /// </summary>
        public DbSet<ActivoPrestamo> ActivosPrestamo { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<ComentarioPrestamo> ComentariosPrestamo { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<Transferencia> Transferencias { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<EstadoTransferencia> EstadosTransferencia { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<EventoTransferencia> EventosTransferencia { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<ComentarioTransferencia> ComentariosTransferencia { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<ActivoTransferencia> ActivosTransferencia { get; set; }

        /// <summary>
        /// Comentarios de prestamo existentes en la aplicación
        /// </summary>
        public DbSet<ActivoDeclinado> ActivosDeclinados { get; set; }

        /// <summary>
        /// Variable de Entrada Clasificacion
        /// </summary>
        public DbSet<EntradaClasificacion> EntradaClasificacion{ get; set; }
        /// <summary>
        /// Variable de Tipo disposicion documental
        /// </summary>
        public DbSet<TipoDisposicionDocumental> TipoDisposicionDocumental { get; set; }

        /// <summary>
        /// Variable de Valoracion entrada de clasificacion
        /// </summary>
        public DbSet<ValoracionEntradaClasificacion> ValoracionEntradaClasificacion { get; set; }
        /// <summary>
        /// Variable de Tipo de Valoracion Documental
        /// </summary>
        public DbSet<TipoValoracionDocumental> TipoValoracionDocumental { get; set; }

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
            //base.OnModelCreating(builder);

            builder.ApplyConfiguration<CuadroClasificacion>(new DbConfCuadroClasificacion());
            builder.ApplyConfiguration<ElementoClasificacion>(new DbConfElementoClasificacion());
            builder.ApplyConfiguration<EstadoCuadroClasificacion>(new DbConfEstadoCuadroClasificacion());

            builder.ApplyConfiguration<Archivo>(new DbConfArchivo());
            builder.ApplyConfiguration<TipoArchivo>(new DbConfTipoArchivo());

            builder.ApplyConfiguration<Activo>(new DbConfActivo());
            builder.ApplyConfiguration<Asunto>(new DbConfAsunto());

            builder.ApplyConfiguration<Ampliacion>(new DbConfAmpliacion());
            builder.ApplyConfiguration<TipoAmpliacion>(new DbConfTipoAmpliacion());
            builder.ApplyConfiguration<HistorialArchivoActivo>(new DbConfHistorialArchivoActivo());

            builder.ApplyConfiguration<AlmacenArchivo>(new DbConfAlmacenArchivo());
            builder.ApplyConfiguration<Estante>(new DbConfEstante());
            builder.ApplyConfiguration<EspacioEstante>(new DbConfEspacioEstante());

            builder.ApplyConfiguration<Prestamo>(new DbConfPrestamo());
            builder.ApplyConfiguration<ActivoPrestamo>(new DbConfActivoPrestamo());
            builder.ApplyConfiguration<ComentarioPrestamo>(new DbConfComentarioPrestamo());

            builder.ApplyConfiguration<Transferencia>(new DbConfTransferencia());
            builder.ApplyConfiguration<EstadoTransferencia>(new DbConfEstadoTransferencia());
            builder.ApplyConfiguration<EventoTransferencia>(new DbConfEventoTransferencia());
            builder.ApplyConfiguration<ComentarioTransferencia>(new DbConfComentarioTransferencia());
            builder.ApplyConfiguration<ActivoTransferencia>(new DbConfActivoTransferencia());
            builder.ApplyConfiguration<ActivoDeclinado>(new DbConfActivoDeclinado());
            builder.ApplyConfiguration<EntradaClasificacion>(new DbConfEntradaClasificacion());
            builder.ApplyConfiguration<TipoDisposicionDocumental>(new DbConfTipoDisposicionDocumental());
            builder.ApplyConfiguration<ValoracionEntradaClasificacion>(new DbConfValoracionEntradaClasificacion());
            builder.ApplyConfiguration<TipoValoracionDocumental>(new DbConfTipoValoracionDocumental());

        }

    }
}
