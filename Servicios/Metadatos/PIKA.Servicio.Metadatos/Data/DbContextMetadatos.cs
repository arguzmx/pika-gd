using FluentValidation.Internal;
using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Data.Configuracion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.Data
{
    public class DbContextMetadatosFactory : IFabricaContexto<DbContextMetadatos>
    {

        private IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones;
        public DbContextMetadatosFactory(IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextMetadatos Crear()
        {
            return new DbContextMetadatos(proveedorOpciones.ObtieneOpciones());
        }
    }
    public class DbContextMetadatos : DbContext, IRepositorioInicializable
    {
        public DbContextMetadatos(DbContextOptions options)
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

        /// <summary>
        /// Almacenes de datos disponibles en la aplicación
        /// </summary>
        public static string TablaAlmacenDatos { get => "metadatos$almacendatos"; }
        /// <summary>
        /// Valores Lista propiedad
        /// </summary>
        public static string TablaValoresListaPropiedad { get => "metadatos$ValoresListaPropiedad"; }

        /// <summary>
        /// Atributos eventos
        /// </summary>
        public static string TablaAtributoEvento { get => "metadatos$atributoevento"; }

        /// <summary>
        /// Atributos Listas
        /// </summary>
        public static string TablaAtributoLista { get => "metadatos$atributolista"; }
        /// <summary>
        /// AtributoVistaUI
        /// </summary>
        public static string TablaAtributoVistaUI { get => "metadatos$atributovistaUI"; }
        /// <summary>
        /// Catalogo Vinculado
        /// </summary>
        public static string TablaCatalogoVinculado { get => "metadatos$catalogovinculado"; }
        /// <summary>
        /// Diccionario Entidad Vinculada
        /// </summary>
        public static string TablaDiccionarioEntidadVinculada { get => "metadatos$diccionarioentidadvinculada"; }
        /// <summary>
        /// Entidad Vinculada
        /// </summary>
        public static string TablaEntidadVinculada { get => "metadatos$entidadvinculada"; }
        /// <summary>
        /// Metadata Info
        /// </summary>
        public static string TablaMetadataInfo { get => "metadatos$metadatainfo"; }
        /// <summary>
        /// Valor Lista
        /// </summary>
        public static string TablaValorLista { get => "metadatos$valorlista"; }
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
        /// Metadatos existentes en la PropiedadPlantilla
        /// </summary>
        public DbSet<ValorListaPlantilla> ValoresListaPropiedad { get; set; }


        /// <summary>
        /// Metadatos existentes en la TipoDato
        /// </summary>
        public DbSet<TipoDato> TipoDato { get; set; }

        // <summary>
        // Metadatos existentes en la AtributoMetadato
        // </summary>

        public DbSet<AtributoTabla> AtributoTabla { get; set; }


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
        /// <summary>
        /// Metadatos existentes en la Asociacion plantilla
        /// </summary>
        public DbSet<AlmacenDatos> AlmacenesDatos { get; set; }
        /// <summary>
        /// Atributos eventos
        /// </summary>
        public DbSet<AtributoEvento> AtributoEventos { get; set; }
        /// <summary>
        /// Atributos de listas
        /// </summary>
        public DbSet<AtributoLista> AtributoListas { get; set; }
        /// <summary>
        /// Atributos de vistas UI
        /// </summary>
        public DbSet<AtributoVistaUI> AtributoVistaUIs { get; set; }
        /// <summary>
        /// Catalogos
        /// </summary>
        public DbSet<CatalogoVinculado> CatalogoVinculados { get; set; }
        /// <summary>
        /// Diccionario de entidades
        /// </summary>
        public DbSet<DiccionarioEntidadVinculada> DiccionarioEntidadVinculadas { get; set; }


        /// <summary>
        /// Tipo de almacenes
        /// </summary>

        public DbSet<TipoAlmacenMetadatos> TipoAlmacenMetadatos { get; set; }
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
            builder.ApplyConfiguration<Plantilla>(new DbConfPlantilla());
            builder.ApplyConfiguration<AlmacenDatos>(new DbConfAlmacenDatos());
            builder.ApplyConfiguration<PropiedadPlantilla>(new DbConfPropiedadPlantilla());
            builder.ApplyConfiguration<ValorListaPlantilla>(new DbConfValorListaPropiedad());
            builder.ApplyConfiguration<TipoDato>(new DbConfTipoDato());
            builder.ApplyConfiguration<AtributoTabla>(new DbConfAtributoTabla());
            
            builder.ApplyConfiguration<ValidadorNumero>(new DbConfValidadorNumero());
            builder.ApplyConfiguration<ValidadorTexto>(new DbConfValidadorTexto());
            builder.ApplyConfiguration<AsociacionPlantilla>(new DbConfAsociacionPlantilla());
            builder.ApplyConfiguration<TipoAlmacenMetadatos>(new DbConfTipoAlmacenMetadatos());
            
            builder.ApplyConfiguration<AtributoEvento>(new DbConfAtributoEvento());
            builder.ApplyConfiguration<AtributoLista>(new DbConfAtributoLista());
            builder.ApplyConfiguration<AtributoVistaUI>(new DbConfAtributoVistaUI());
            builder.ApplyConfiguration<CatalogoVinculado>(new DbConfCatalogoVinculado());
            builder.ApplyConfiguration<DiccionarioEntidadVinculada>(new DbConfDiccionarioEntidadVinculada());
            builder.ApplyConfiguration<ValorLista>(new DbConfValorLista());

        }

    }
}
