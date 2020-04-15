using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data.Configuracion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data
{
   public class DbContextMetadatos : DbContext
    {
        public DbContextMetadatos(DbContextOptions<DbContextMetadatos> options)
        : base(options)
        {
        }


        #region Constantes de configuracion

        /// <summary>
        /// Nombre de la tabla para las entidades del Plantilla
        /// </summary>
        public static string TablaPlantilla { get => "metadatos$plantilla"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del Propiedad Plantilla
        /// </summary>
        public static string TablaPropiedadPlantilla { get => "metadatos$propiedadplantilla"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del TipoDato
        /// </summary>
        public static string TablaTipoDato { get => "metadatos$tipodato"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del Tipo Dato plantilla
        /// </summary>
        public static string TablaTipoDatoPropiedadPlantilla { get => "metadatos$tipodatopropiedadplantilla"; }


        /// <summary>
        ///  Nombre de la tabla para las entidades del AtributoMetadato
        /// </summary>
        public static string TablaAtributoMetadato { get => "metadatos$atributometadato"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del AtributoTabla
        /// </summary>
        public static string TablaAtributoTabla { get => "metadatos$atributotabla"; }
     
        /// <summary>
        ///  Nombre de la tabla para las entidades del ValidadorNumero
        /// </summary>
        public static string TablaValidadorNumero { get => "metadatos$validadornumero"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del ValidadorTexto
        /// </summary>
        public static string TablaValidadorTexto { get => "metadatos$validadortexto"; }

        /// <summary>
        ///  Nombre de la tabla para las entidades del Asociacion de Plantilla
        /// </summary>
        public static string TablaAsociacionPlantilla { get => "metadatos$asociacionplantilla"; }
        /// <summary>
        ///  Nombre de la tabla para las entidades del TipoAlmacen Metadatos
        /// </summary>
        public static string TablaTipoAlmacenMetadatos { get => "metadatos$tipoalmacenmetadatos"; }


        #endregion

        /// <summary>
        /// Metadatos existentes en la Plantilla
        /// </summary>
        public DbSet<Plantilla> Plantilla { get; set; }
        /// <summary>
        /// Metadatos existentes en la PropiedadPlantilla
        /// </summary>
        public DbSet<PropiedadPlantilla> PropiedadPlantilla { get; set; }

        /// <summary>
        /// Metadatos existentes en la TipoDato
        /// </summary>
        public DbSet<TipoDato> TipoDato { get; set; }
        // <summary>
        /// Contiene los tipos de datos asociados con las propeidades de las plantillas
        /// </summary>
         public DbSet<TipoDatoPropiedadPlantilla> TipoDatoPropiedadPlantilla { get; set; }


        // <summary>
        // Metadatos existentes en la AtributoMetadato
        // </summary>

        public DbSet<AtributoTabla> AtributoTabla { get; set; }

        // <summary>
        /// Metadatos existentes en la AtributoTabla
        /// </summary>

        public DbSet<AtributoMetadato> AtributoMetadato { get; set; }

        /// <summary>
        /// Metadatos existentes en la ValidadorNumero
        /// </summary>
        public DbSet<ValidadorNumero> ValidadorNumero { get; set; }

        /// <summary>
        /// Metadatos existentes en la ValidadorTexto
        /// </summary>
        public DbSet<ValidadorTexto> ValidadorTexto { get; set; }
        /// <summary>
        /// Metadatos existentes en la Asociacion plantilla
        /// </summary>

        public DbSet<AsociacionPlantilla> AsociacionPlantilla { get; set; }

        public DbSet<TipoAlmacenMetadatos> TipoAlmacenMetadatos { get; set; }
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
            builder.ApplyConfiguration<Plantilla>(new DbConfPlantilla());
            builder.ApplyConfiguration<PropiedadPlantilla>(new DbConfPropiedadPlantilla());
            builder.ApplyConfiguration<TipoDato>(new DbConfTipoDato());
            builder.ApplyConfiguration<TipoDatoPropiedadPlantilla>(new DbConfTipoDatoPropiedadPlantilla());
            builder.ApplyConfiguration<AtributoTabla>(new DbConfAtributoTabla());
            builder.ApplyConfiguration<AtributoMetadato>(new DbConfAtributoMetadato());
            builder.ApplyConfiguration<ValidadorNumero>(new DbConfValidadorNumero());
            builder.ApplyConfiguration<ValidadorTexto>(new DbConfValidadorTexto());
            builder.ApplyConfiguration<AsociacionPlantilla>(new DbConfAsociacionPlantilla());
            builder.ApplyConfiguration<TipoAlmacenMetadatos>(new DbConfTipoAlmacenMetadatos());
        }

    }
}
