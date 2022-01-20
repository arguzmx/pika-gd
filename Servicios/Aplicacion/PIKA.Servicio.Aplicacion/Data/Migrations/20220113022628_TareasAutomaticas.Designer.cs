﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.AplicacionPlugin;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    [DbContext(typeof(DbContextAplicacion))]
    [Migration("20220113022628_TareasAutomaticas")]
    partial class TareasAutomaticas
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.BitacoraTarea", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("BitacoraTareaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CodigoError")
                        .HasColumnType("varchar(250) CHARACTER SET utf8mb4")
                        .HasMaxLength(250);

                    b.Property<int>("Duracion")
                        .HasColumnType("int");

                    b.Property<bool>("Exito")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("UltimaEjecucion")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BitacoraTareaId");

                    b.ToTable("aplicacion$bitacoratarea");
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.Plugins.Plugin", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("Gratuito")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("aplicacion$plugin");
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.Plugins.PluginInstalado", b =>
                {
                    b.Property<string>("PLuginId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("VersionPLuginId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("FechaInstalacion")
                        .HasColumnType("datetime(6)");

                    b.HasKey("PLuginId", "VersionPLuginId");

                    b.HasIndex("VersionPLuginId");

                    b.ToTable("aplicacion$plugininstalado");
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.Plugins.VersionPlugin", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PluginId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("RequiereConfiguracion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("PluginId");

                    b.ToTable("aplicacion$versionplugin");
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.TareaAutomatica", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CodigoError")
                        .HasColumnType("varchar(250) CHARACTER SET utf8mb4")
                        .HasMaxLength(250);

                    b.Property<int?>("Duracion")
                        .HasColumnType("int");

                    b.Property<string>("Ensamblado")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool?>("Exito")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("FechaHoraEjecucion")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Intervalo")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OrigenId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<int>("Periodo")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ProximaEjecucion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TipoOrigenId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UltimaEjecucion")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("aplicacion$autotarea");
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.BitacoraTarea", b =>
                {
                    b.HasOne("PIKA.Modelo.Aplicacion.TareaAutomatica", "Tarea")
                        .WithMany("Bitacora")
                        .HasForeignKey("BitacoraTareaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.Plugins.PluginInstalado", b =>
                {
                    b.HasOne("PIKA.Modelo.Aplicacion.Plugins.Plugin", "Plugin")
                        .WithMany("PluginInstalados")
                        .HasForeignKey("PLuginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Aplicacion.Plugins.VersionPlugin", "VersionPlugin")
                        .WithMany("PluginInstalados")
                        .HasForeignKey("VersionPLuginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.Aplicacion.Plugins.VersionPlugin", b =>
                {
                    b.HasOne("PIKA.Modelo.Aplicacion.Plugins.Plugin", "Plugins")
                        .WithMany("versionPlugins")
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
