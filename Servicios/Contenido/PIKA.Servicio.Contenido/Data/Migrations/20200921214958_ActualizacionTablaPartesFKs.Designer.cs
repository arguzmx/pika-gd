﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Contenido;

namespace PIKA.Servicio.Contenido.data.Migrations
{
    [DbContext(typeof(DbContextContenido))]
    [Migration("20200921214958_ActualizacionTablaPartesFKs")]
    partial class ActualizacionTablaPartesFKs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Modelo.Contenido.Carpeta", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CarpetaPadreId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CreadorId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsRaiz")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("PermisoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PuntoMontajeId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CarpetaPadreId");

                    b.HasIndex("PermisoId");

                    b.HasIndex("PuntoMontajeId");

                    b.ToTable("contenido$carpeta");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.DestinatarioPermiso", b =>
                {
                    b.Property<string>("PermisoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("DestinatarioId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("EsGrupo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.HasKey("PermisoId", "DestinatarioId");

                    b.ToTable("contenido$destinatariopermiso");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Elemento", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CarpetaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CreadorId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("PermisoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PuntoMontajeId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Versionado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("VolumenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("PermisoId");

                    b.HasIndex("PuntoMontajeId");

                    b.HasIndex("VolumenId");

                    b.ToTable("contenido$elemento");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.GestorAzureConfig", b =>
                {
                    b.Property<string>("VolumenId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Contrasena")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Endpoint")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("Usuario")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("VolumenId");

                    b.ToTable("contenido$gestorazure");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.GestorLocalConfig", b =>
                {
                    b.Property<string>("VolumenId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Ruta")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.HasKey("VolumenId");

                    b.ToTable("contenido$gestorlocal");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.GestorSMBConfig", b =>
                {
                    b.Property<string>("VolumenId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Contrasena")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Dominio")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Ruta")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("Usuario")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("VolumenId");

                    b.ToTable("contenido$gestorsmb");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Parte", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<long>("ConsecutivoVolumen")
                        .HasColumnType("bigint");

                    b.Property<string>("ElementoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<int>("Indice")
                        .HasColumnType("int");

                    b.Property<int>("LongitudBytes")
                        .HasColumnType("int");

                    b.Property<string>("NombreOriginal")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("TipoMime")
                        .IsRequired()
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<string>("VersionId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ElementoId");

                    b.HasIndex("VersionId");

                    b.ToTable("contenido$versionpartes");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Permiso", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Crear")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("Eliminar")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("Escribir")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("Leer")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.HasKey("Id");

                    b.ToTable("contenido$permiso");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.PuntoMontaje", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CreadorId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("VolumenDefaultId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("VolumenDefaultId");

                    b.ToTable("contenido$pmontaje");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.TipoGestorES", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("contenido$tipogestores");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Version", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Activa")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("ConteoPartes")
                        .HasColumnType("int");

                    b.Property<string>("CreadorId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ElementoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MaxIndicePartes")
                        .HasColumnType("int");

                    b.Property<long>("TamanoBytes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ElementoId");

                    b.ToTable("contenido$versionelemento");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Volumen", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Activo")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("CanidadElementos")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(0L);

                    b.Property<long>("CanidadPartes")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(0L);

                    b.Property<bool>("ConfiguracionValida")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<long>("ConsecutivoVolumen")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(0L);

                    b.Property<bool>("Eliminada")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("EscrituraHabilitada")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<long>("Tamano")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(0L);

                    b.Property<long>("TamanoMaximo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasDefaultValue(0L);

                    b.Property<string>("TipoGestorESId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("TipoGestorESId");

                    b.ToTable("contenido$volumen");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.VolumenPuntoMontaje", b =>
                {
                    b.Property<string>("VolumenId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PuntoMontajeId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("VolumenId", "PuntoMontajeId");

                    b.HasIndex("PuntoMontajeId");

                    b.ToTable("contenido$puntomontajevolumen");
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Carpeta", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Carpeta", "CarpetaPadre")
                        .WithMany("Subcarpetas")
                        .HasForeignKey("CarpetaPadreId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PIKA.Modelo.Contenido.Permiso", "Permiso")
                        .WithMany("Carpetas")
                        .HasForeignKey("PermisoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PIKA.Modelo.Contenido.PuntoMontaje", "PuntoMontaje")
                        .WithMany("Carpetas")
                        .HasForeignKey("PuntoMontajeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.DestinatarioPermiso", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Permiso", "Permiso")
                        .WithMany("Destinatarios")
                        .HasForeignKey("PermisoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Elemento", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Permiso", "Permiso")
                        .WithMany("Elementos")
                        .HasForeignKey("PermisoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("PIKA.Modelo.Contenido.PuntoMontaje", "PuntoMontaje")
                        .WithMany("Elementos")
                        .HasForeignKey("PuntoMontajeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Contenido.Volumen", "Volumen")
                        .WithMany("Elementos")
                        .HasForeignKey("VolumenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.GestorAzureConfig", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Volumen", "Volumen")
                        .WithOne("AxureConfig")
                        .HasForeignKey("PIKA.Modelo.Contenido.GestorAzureConfig", "VolumenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.GestorLocalConfig", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Volumen", "Volumen")
                        .WithOne("LocalConfig")
                        .HasForeignKey("PIKA.Modelo.Contenido.GestorLocalConfig", "VolumenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.GestorSMBConfig", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Volumen", "Volumen")
                        .WithOne("SMBConfig")
                        .HasForeignKey("PIKA.Modelo.Contenido.GestorSMBConfig", "VolumenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Parte", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Elemento", "Elemento")
                        .WithMany("Partes")
                        .HasForeignKey("ElementoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Contenido.Version", "Version")
                        .WithMany("Partes")
                        .HasForeignKey("VersionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.PuntoMontaje", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Volumen", "VolumenDefault")
                        .WithMany("PuntosMontaje")
                        .HasForeignKey("VolumenDefaultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Version", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.Elemento", "Elemento")
                        .WithMany("Versiones")
                        .HasForeignKey("ElementoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.Volumen", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.TipoGestorES", "TipoGestorES")
                        .WithMany("Volumenes")
                        .HasForeignKey("TipoGestorESId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Contenido.VolumenPuntoMontaje", b =>
                {
                    b.HasOne("PIKA.Modelo.Contenido.PuntoMontaje", "PuntoMontaje")
                        .WithMany("VolumenesPuntoMontaje")
                        .HasForeignKey("PuntoMontajeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Contenido.Volumen", "Volumen")
                        .WithMany("PuntosMontajeVolumen")
                        .HasForeignKey("VolumenId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
