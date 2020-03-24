using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data.Configuracion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data
{
    public class DBContextGestionDocumental : DbContext
    {
        public DBContextGestionDocumental(DbContextOptions<DBContextGestionDocumental> options)
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
        public static string TablaElementosClasificacion{ get => "gd$elementoclasificacion"; }


        /// <summary>
        /// Nombre de la tabla para las entidades del tipo Estado cuadro clasificacion
        /// </summary>
        public static string TablaEstadosCuadro{ get => "gd$estadocuadroclasificacion"; }


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
        public static string TablaFasesCicloVital{ get => "gd$faseciclovital"; }


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
        /// Fases del ciclo vital existentes en la aplicación
        /// </summary>
        public DbSet<FaseCicloVital> FasesCicloVital { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration<CuadroClasificacion>(new DbConfCuadroClasificacion());
            builder.ApplyConfiguration<ElementoClasificacion>(new DbConfElementoClasificacion());
            builder.ApplyConfiguration<EstadoCuadroClasificacion>(new DbConfEstadoCuadroClasificacion());

            builder.ApplyConfiguration<Archivo>(new DbConfArchivo());
            builder.ApplyConfiguration<TipoArchivo>(new DbConfTipoArchivo());
            builder.ApplyConfiguration<FaseCicloVital>(new DbConfFaseCicloVital());

        }

    }
}
