﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.Organizacion;

namespace PIKA.Servicio.Organizacion.Data.Migrations
{
    [DbContext(typeof(DbContextOrganizacion))]
    [Migration("20200227225040_UOaDominio")]
    partial class UOaDominio
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Modelo.Organizacion.DireccionPostal", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CP")
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.Property<string>("Calle")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Colonia")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("EstadoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Municipio")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("NoExterno")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("NoInterno")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Nombre")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PaisId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EstadoId");

                    b.HasIndex("PaisId");

                    b.HasIndex("TipoOrigenId", "OrigenId");

                    b.ToTable("org$direccion_postal");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.Dominio", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

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

                    b.HasIndex("TipoOrigenId", "OrigenId");

                    b.ToTable("org$dominio");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.Estado", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("PaisId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("Valor")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("PaisId");

                    b.ToTable("org$estado");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.Pais", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Valor")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("org$pais");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.Rol", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Descripcion")
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("RolPadreId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("RolPadreId");

                    b.HasIndex("TipoOrigenId", "OrigenId");

                    b.ToTable("org$rol");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.UnidadOrganizacional", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("DominioId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("DominioId");

                    b.ToTable("org$ou");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.UsuariosRol", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("RolId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.HasKey("ApplicationUserId", "RolId");

                    b.HasIndex("RolId");

                    b.ToTable("org$usuarios_rol");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.DireccionPostal", b =>
                {
                    b.HasOne("PIKA.Modelo.Organizacion.Estado", "Estado")
                        .WithMany("Direcciones")
                        .HasForeignKey("EstadoId");

                    b.HasOne("PIKA.Modelo.Organizacion.Pais", "Pais")
                        .WithMany("Direcciones")
                        .HasForeignKey("PaisId");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.Estado", b =>
                {
                    b.HasOne("PIKA.Modelo.Organizacion.Pais", "Pais")
                        .WithMany("Estados")
                        .HasForeignKey("PaisId");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.Rol", b =>
                {
                    b.HasOne("PIKA.Modelo.Organizacion.Rol", "RolPadre")
                        .WithMany("SubRoles")
                        .HasForeignKey("RolPadreId");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.UnidadOrganizacional", b =>
                {
                    b.HasOne("PIKA.Modelo.Organizacion.Dominio", "Dominio")
                        .WithMany("UnidadesOrganizacionales")
                        .HasForeignKey("DominioId");
                });

            modelBuilder.Entity("PIKA.Modelo.Organizacion.UsuariosRol", b =>
                {
                    b.HasOne("PIKA.Modelo.Organizacion.Rol", "Rol")
                        .WithMany("UsuariosRol")
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
