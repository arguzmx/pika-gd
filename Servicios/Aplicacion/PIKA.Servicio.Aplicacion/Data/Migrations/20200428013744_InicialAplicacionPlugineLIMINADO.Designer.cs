﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.AplicacionPlugin;

namespace PIKA.Servicio.AplicacionPlugin.Data.Migrations
{
    [DbContext(typeof(DbContextAplicacionPlugin))]
    [Migration("20200428013744_InicialAplicacionPlugineLIMINADO")]
    partial class InicialAplicacionPlugineLIMINADO
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

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

                    b.Property<DateTime>("FechaInstalacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValue(new DateTime(2020, 4, 27, 20, 37, 43, 722, DateTimeKind.Local).AddTicks(2141));

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
