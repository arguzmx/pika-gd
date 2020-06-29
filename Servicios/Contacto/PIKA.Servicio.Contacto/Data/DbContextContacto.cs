
using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Contacto;
using PIKA.Servicio.Contacto.Data.Configuracion;
using RepositorioEntidades;
using System;

namespace PIKA.Servicio.Contacto
{
    public class DbContextContactoFactory : IFabricaContexto<DbContextContacto>
    {

        private IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones;
        public DbContextContactoFactory(IProveedorOpcionesContexto<DbContextContacto> proveedorOpciones)
        {
            this.proveedorOpciones = proveedorOpciones;
        }

        public DbContextContacto Crear()
        {
            return new DbContextContacto(proveedorOpciones.ObtieneOpciones());
        }
    }

    public class DbContextContacto : DbContext, IRepositorioInicializable
    {

        #region  nombres de tablas
        public static string TablaBase { get => "contacto"; }
        public static string SeparadosEsquema { get => "$"; }

        public static string TablaPais { get => $"{TablaBase}{SeparadosEsquema}pais"; }
        public static string TablaDireccionesPostales { get => $"{TablaBase}{SeparadosEsquema}direcciones"; }
        public static string TablaEstados { get => $"{TablaBase}{SeparadosEsquema}estados"; }
        public static string TablaHorariosMedioContacto { get => $"{TablaBase}{SeparadosEsquema}horariomediocontacto"; }
        public static string TablaMedioContacto { get => $"{TablaBase}{SeparadosEsquema}mediocontacto"; }
        public static string TablaTipoMedioContacto { get => $"{TablaBase}{SeparadosEsquema}tipomediocontacto"; }
        public static string TablaTipoFuenteContacto { get => $"{TablaBase}{SeparadosEsquema}tipofuentecontacto"; }

        #endregion

        public DbContextContacto(DbContextOptions options) : base(options)
        {
        }

        
        /// <summary>
        /// Catálogo de paises
        /// </summary>
        public DbSet<Pais> Paises { get; set; }


        /// <summary>
        /// Direcciones posatles
        /// </summary>
        public DbSet<DireccionPostal> Direcciones { get; set; }

        /// <summary>
        /// Direcciones posatles
        /// </summary>
        public DbSet<Estado> Estados { get; set; }

        /// <summary>
        /// JHorarios de los medios de conatcto
        /// </summary>
        public DbSet<HorarioMedioContacto> HorariosMedioContacto { get; set; }


        /// <summary>
        /// MEdios de contacto 
        /// </summary>
        public DbSet<MedioContacto> MediosContacto { get; set; }


        public DbSet<TipoMedio> TiposMedio { get; set; }

        public DbSet<TipoFuenteContacto> TiposFuentesContacto { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration<DireccionPostal>(new DbConfDpostales());
            builder.ApplyConfiguration<Estado>(new DbConfEstado());
            builder.ApplyConfiguration<Pais>(new DbConfPais());
            builder.ApplyConfiguration<TipoFuenteContacto>(new DbConfTipoFuenteContacto());
            builder.ApplyConfiguration<TipoMedio>(new DbConfTipoMedioContacto());
            builder.ApplyConfiguration<MedioContacto>(new DbConfMedioContacto());
            builder.ApplyConfiguration<HorarioMedioContacto>(new DbConfHorarioMedioContacto());



        }

        public void AplicarMigraciones()
        {
            this.Database.Migrate();
        }

        public void Inicializar(string contentPath, bool generarDatosdemo)
        {
            Console.WriteLine("Inicializando datos des repositorio de contacto");
            InicializarDatos.Inicializar(this, contentPath, generarDatosdemo);
        }
    }
}
