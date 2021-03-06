﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Metadatos.Data;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    [DbContext(typeof(DbContextMetadatos))]
    [Migration("20200825213236_ActalizarPropiedadPlantillaMetadatos")]
    partial class ActalizarPropiedadPlantillaMetadatos
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AlmacenDatos", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Contrasena")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Nombre")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Protocolo")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("Puerto")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("TipoAlmacenMetadatosId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Usuario")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("TipoAlmacenMetadatosId");

                    b.ToTable("metadatos$almacendatos");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AsociacionPlantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("IdentificadorAlmacenamiento")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PlantillaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("PlantillaId");

                    b.HasIndex("TipoOrigenId", "OrigenId");

                    b.ToTable("metadatos$asociacionplantilla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoEvento", b =>
                {
                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Entidad")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int>("Evento")
                        .HasColumnType("int");

                    b.Property<int>("Operacion")
                        .HasColumnType("int");

                    b.Property<string>("Parametro")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("PropiedadPlantillaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.HasKey("PropiedadId");

                    b.HasIndex("PropiedadPlantillaId");

                    b.ToTable("metadatos$atributoevento");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoLista", b =>
                {
                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("DatosRemotos")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Default")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Entidad")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<bool>("OrdenarAlfabetico")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("TypeAhead")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("PropiedadId");

                    b.ToTable("metadatos$atributolista");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoTabla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<bool>("Alternable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<string>("IdTablaCliente")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Incluir")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<int>("IndiceOrdebnamiento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Visible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.HasIndex("PropiedadId")
                        .IsUnique();

                    b.ToTable("metadatos$atributotabla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoVistaUI", b =>
                {
                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("Accion")
                        .HasColumnType("int");

                    b.Property<string>("Control")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Plataforma")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("PropiedadPlantillaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.HasKey("PropiedadId");

                    b.HasIndex("PropiedadPlantillaId");

                    b.ToTable("metadatos$atributovistaUI");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.CatalogoVinculado", b =>
                {
                    b.Property<string>("IdCatalogo")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("IdEntidad")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("Despliegue")
                        .HasColumnType("int");

                    b.Property<string>("EntidadCatalogo")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("EntidadVinculo")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("IdCatalogoMap")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("IdEntidadMap")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PropiedadReceptora")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("IdCatalogo", "IdEntidad");

                    b.ToTable("metadatos$catalogovinculado");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.DiccionarioEntidadVinculada", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Enidad")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("metadatos$diccionarioentidadvinculada");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.Plantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AlmacenDatosId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<bool>("Eliminada")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("AlmacenDatosId");

                    b.ToTable("metadatos$plantilla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.PropiedadPlantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AtributoListaPropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<bool>("Autogenerado")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Buscable")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ControlHTML")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("EsFiltroJerarquia")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EsIdClaveExterna")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EsIdJerarquia")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EsIdRaizJerarquia")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EsIdRegistro")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EsIndice")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EsTextoJerarquia")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Etiqueta")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("IndiceOrdenamiento")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<bool>("Ordenable")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("OrdenarValoresListaPorNombre")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PlantillaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Requerido")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TipoDatoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ValorDefault")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Visible")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("AtributoListaPropiedadId");

                    b.HasIndex("PlantillaId");

                    b.HasIndex("TipoDatoId");

                    b.ToTable("metadatos$propiedadplantilla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.TipoAlmacenMetadatos", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("metadatos$tipoalmacenmetadatos");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.TipoDato", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("metadatos$tipodato");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorNumero", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PropiedadId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("UtilizarMax")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("UtilizarMin")
                        .HasColumnType("tinyint(1)");

                    b.Property<float>("max")
                        .HasColumnType("float");

                    b.Property<float>("min")
                        .HasColumnType("float");

                    b.Property<float>("valordefault")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("PropiedadId")
                        .IsUnique();

                    b.ToTable("metadatos$validadornumero");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorTexto", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PropiedadId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("longmax")
                        .HasColumnType("int");

                    b.Property<int>("longmin")
                        .HasColumnType("int");

                    b.Property<string>("regexp")
                        .HasColumnType("varchar(1024) CHARACTER SET utf8mb4")
                        .HasMaxLength(1024);

                    b.Property<string>("valordefault")
                        .HasColumnType("varchar(512) CHARACTER SET utf8mb4")
                        .HasMaxLength(512);

                    b.HasKey("Id");

                    b.HasIndex("PropiedadId")
                        .IsUnique();

                    b.ToTable("metadatos$validadortexto");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValorLista", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("Indice")
                        .HasColumnType("int");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("metadatos$valorlista");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValorListaPlantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("Indice")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("PropiedadId");

                    b.ToTable("metadatos$valorespropiedad");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AlmacenDatos", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.TipoAlmacenMetadatos", "TipoAlmacen")
                        .WithMany("AlmacensDatos")
                        .HasForeignKey("TipoAlmacenMetadatosId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AsociacionPlantilla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.Plantilla", "Plantilla")
                        .WithMany("Asociaciones")
                        .HasForeignKey("PlantillaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoEvento", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", null)
                        .WithMany("AtributosEvento")
                        .HasForeignKey("PropiedadPlantillaId");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoTabla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "PropiedadPlantilla")
                        .WithOne("AtributoTabla")
                        .HasForeignKey("PIKA.Modelo.Metadatos.AtributoTabla", "PropiedadId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoVistaUI", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", null)
                        .WithMany("AtributosVistaUI")
                        .HasForeignKey("PropiedadPlantillaId");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.Plantilla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.AlmacenDatos", "Almacen")
                        .WithMany("Plantillas")
                        .HasForeignKey("AlmacenDatosId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.PropiedadPlantilla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.AtributoLista", "AtributoLista")
                        .WithMany()
                        .HasForeignKey("AtributoListaPropiedadId");

                    b.HasOne("PIKA.Modelo.Metadatos.Plantilla", "Plantilla")
                        .WithMany("Propiedades")
                        .HasForeignKey("PlantillaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Metadatos.TipoDato", "TipoDato")
                        .WithMany("PropiedadesPlantilla")
                        .HasForeignKey("TipoDatoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorNumero", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "PropiedadPlantilla")
                        .WithOne("ValidadorNumero")
                        .HasForeignKey("PIKA.Modelo.Metadatos.ValidadorNumero", "PropiedadId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorTexto", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "PropiedadPlantilla")
                        .WithOne("ValidadorTexto")
                        .HasForeignKey("PIKA.Modelo.Metadatos.ValidadorTexto", "PropiedadId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValorListaPlantilla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "Propiedad")
                        .WithMany("ValoresLista")
                        .HasForeignKey("PropiedadId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
