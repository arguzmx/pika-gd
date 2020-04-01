﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Seguridad;

namespace PIKA.Servicio.Seguridad.Data.Migrations
{
    [DbContext(typeof(DbContextSeguridad))]
    partial class DbContextSeguridadModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Infraestructura.Comun.Aplicacion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int>("ReleaseIndex")
                        .HasColumnType("int");

                    b.Property<string>("UICulture")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("seguridad$aplicacion");
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.ModuloAplicacion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AplicacionId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AplicacionPadreId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Asegurable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("Icono")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("ModuloPadreId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<ulong>("PermisosDisponibles")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("UICulture")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("AplicacionId");

                    b.HasIndex("ModuloPadreId");

                    b.ToTable("seguridad$moduloaplicacion");
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.TipoAdministradorModulo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AplicacionId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ModuloId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ModuloId");

                    b.ToTable("seguridad$tipoadministradormodulo");
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.TraduccionAplicacionModulo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AplicacionId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("ModuloId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("UICulture")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("AplicacionId");

                    b.HasIndex("ModuloId");

                    b.ToTable("seguridad$traduccionaplicacionmodulo");
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.ModuloAplicacion", b =>
                {
                    b.HasOne("PIKA.Infraestructura.Comun.Aplicacion", "Aplicacion")
                        .WithMany("Modulos")
                        .HasForeignKey("AplicacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Infraestructura.Comun.ModuloAplicacion", "ModuloPadre")
                        .WithMany("Modulos")
                        .HasForeignKey("ModuloPadreId");
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.TipoAdministradorModulo", b =>
                {
                    b.HasOne("PIKA.Infraestructura.Comun.ModuloAplicacion", "ModuloApp")
                        .WithMany("TiposAdministrados")
                        .HasForeignKey("ModuloId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.TraduccionAplicacionModulo", b =>
                {
                    b.HasOne("PIKA.Infraestructura.Comun.Aplicacion", "Aplicacion")
                        .WithMany("Traducciones")
                        .HasForeignKey("AplicacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Infraestructura.Comun.ModuloAplicacion", "ModuloApp")
                        .WithMany("Traducciones")
                        .HasForeignKey("ModuloId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}