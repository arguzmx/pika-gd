﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Metadatos.Data;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    [DbContext(typeof(DbContextMetadatos))]
    [Migration("20210304024200_CampoPlantillaGenerada")]
    partial class CampoPlantillaGenerada
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

            modelBuilder.Entity("PIKA.Modelo.Metadatos.Plantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AlmacenDatosId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<bool>("Eliminada")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Generada")
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

                    b.Property<int>("IdNumericoPlantilla")
                        .HasColumnType("int");

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
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("PropiedadId");

                    b.ToTable("metadatos$valoreslistapropiedad");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AlmacenDatos", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.TipoAlmacenMetadatos", "TipoAlmacen")
                        .WithMany("AlmacensDatos")
                        .HasForeignKey("TipoAlmacenMetadatosId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
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
