﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Seguridad;

namespace PIKA.Servicio.Seguridad.data.Migrations
{
    [DbContext(typeof(DbContextSeguridad))]
    [Migration("20201013184838_CambioUsuarioDominioUO")]
    partial class CambioUsuarioDominioUO
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(10);

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
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<ulong>("PermisosDisponibles")
                        .HasColumnType("bigint unsigned")
                        .HasMaxLength(128);

                    b.Property<int>("Tipo")
                        .HasColumnType("int");

                    b.Property<string>("UICulture")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("AplicacionId");

                    b.HasIndex("ModuloPadreId");

                    b.ToTable("seguridad$moduloaplicacion");
                });

            modelBuilder.Entity("PIKA.Infraestructura.Comun.Seguridad.PermisoAplicacion", b =>
                {
                    b.Property<string>("DominioId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AplicacionId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ModuloId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("EntidadAccesoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoEntidadAcceso")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Admin")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Ejecutar")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Eliminar")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Escribir")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Leer")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("NegarAcceso")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("DominioId", "AplicacionId", "ModuloId", "EntidadAccesoId", "TipoEntidadAcceso");

                    b.ToTable("seguridad$permisosapl");
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

            modelBuilder.Entity("PIKA.Modelo.Seguridad.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Email")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("GlobalAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Inactiva")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("PIKA.Modelo.Seguridad.Base.UsuarioDominio", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("DominioId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("UnidadOrganizacionalId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("EsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("ApplicationUserId", "DominioId", "UnidadOrganizacionalId");

                    b.ToTable("seguridad$usuariosdominio");
                });

            modelBuilder.Entity("PIKA.Modelo.Seguridad.Genero", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("seguridad$generousuario");
                });

            modelBuilder.Entity("PIKA.Modelo.Seguridad.PropiedadesUsuario", b =>
                {
                    b.Property<string>("UsuarioId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<bool>("Eliminada")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Inactiva")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("Ultimoacceso")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("email")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<bool?>("email_verified")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("estadoid")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("family_name")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("generoid")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("given_name")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("gmt")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<float?>("gmt_offset")
                        .HasColumnType("float");

                    b.Property<string>("middle_name")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("name")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("nickname")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("paisid")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("updated_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("UsuarioId");

                    b.HasIndex("generoid");

                    b.ToTable("seguridad$usuarioprops");
                });

            modelBuilder.Entity("PIKA.Modelo.Seguridad.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
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

            modelBuilder.Entity("PIKA.Modelo.Seguridad.PropiedadesUsuario", b =>
                {
                    b.HasOne("PIKA.Modelo.Seguridad.ApplicationUser", "Usuario")
                        .WithOne("Propiedades")
                        .HasForeignKey("PIKA.Modelo.Seguridad.PropiedadesUsuario", "UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.Seguridad.Genero", "genero")
                        .WithMany("PropiedadesUsuario")
                        .HasForeignKey("generoid")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("PIKA.Modelo.Seguridad.UserClaim", b =>
                {
                    b.HasOne("PIKA.Modelo.Seguridad.ApplicationUser", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
