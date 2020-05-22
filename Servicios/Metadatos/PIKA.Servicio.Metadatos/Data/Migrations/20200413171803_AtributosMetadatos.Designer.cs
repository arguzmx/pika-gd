﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Metadatos.Data;

namespace PIKA.Servicio.Metadatos.Data.Migrations
{
    [DbContext(typeof(DbContextMetadatos))]
    [Migration("20200413171803_AtributosMetadatos")]
    partial class AtributosMetadatos
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoMetadato", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PropiedadId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PropiedadPlantillaId1")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("PropiedadPlantillaId1");

                    b.ToTable("metadatos$atributometadato");
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

                    b.ToTable("metadatos$atributotabla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.Plantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

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

                    b.ToTable("metadatos$plantilla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.PropiedadPlantilla", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AtributoTablaid")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<bool>("Autogenerado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("Buscable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<string>("ControlHTML")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("EsFiltroJerarquia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsIdClaveExterna")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsIdJerarquia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsIdPadreJerarquia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsIdRegistro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsIndice")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsTextoJerarquia")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<int>("IndiceOrdenamiento")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<bool>("Ordenable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("PlantillaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Requerido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("TipoDatoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoDatoProiedadPlantillaId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Visible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.HasKey("Id");

                    b.HasIndex("AtributoTablaid")
                        .IsUnique();

                    b.HasIndex("PlantillaId");

                    b.ToTable("metadatos$propiedadplantilla");
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

            modelBuilder.Entity("PIKA.Modelo.Metadatos.TipoDatoPropiedadPlantilla", b =>
                {
                    b.Property<string>("TipoDatoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PropiedadPlantillaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("TipoDatoId", "PropiedadPlantillaId");

                    b.HasIndex("PropiedadPlantillaId")
                        .IsUnique();

                    b.ToTable("metadatos$tipodatopropiedadplantilla");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorNumero", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<float>("max")
                        .HasColumnType("float");

                    b.Property<float>("min")
                        .HasColumnType("float");

                    b.Property<float>("valordefault")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("metadatos$validadornumero");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorTexto", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("PropiedadId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("longmax")
                        .HasColumnType("int");

                    b.Property<int>("longmin")
                        .HasColumnType("int");

                    b.Property<string>("regexp")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("valordefaulr")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("metadatos$validadortexto");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.AtributoMetadato", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "PropiedadPlantilla")
                        .WithMany("Atributos")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", null)
                        .WithMany("AtributosMetadatos")
                        .HasForeignKey("PropiedadPlantillaId1");
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.PropiedadPlantilla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.AtributoTabla", "Atributo")
                        .WithOne("propiedadplantilla")
                        .HasForeignKey("PIKA.Modelo.Metadatos.PropiedadPlantilla", "AtributoTablaid");

                    b.HasOne("PIKA.Modelo.Metadatos.Plantilla", "Plantilla")
                        .WithMany("Propiedades")
                        .HasForeignKey("PlantillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.TipoDatoPropiedadPlantilla", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "Propiedad")
                        .WithOne("TipoDatoPropiedad")
                        .HasForeignKey("PIKA.Modelo.Metadatos.TipoDatoPropiedadPlantilla", "PropiedadPlantillaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Metadatos.TipoDato", "Tipo")
                        .WithMany("PropiedadesPlantilla")
                        .HasForeignKey("TipoDatoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorNumero", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "PropiedadPlantilla")
                        .WithOne("ValNumero")
                        .HasForeignKey("PIKA.Modelo.Metadatos.ValidadorNumero", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Metadatos.ValidadorTexto", b =>
                {
                    b.HasOne("PIKA.Modelo.Metadatos.PropiedadPlantilla", "PropiedadPlantilla")
                        .WithOne("ValTexto")
                        .HasForeignKey("PIKA.Modelo.Metadatos.ValidadorTexto", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
