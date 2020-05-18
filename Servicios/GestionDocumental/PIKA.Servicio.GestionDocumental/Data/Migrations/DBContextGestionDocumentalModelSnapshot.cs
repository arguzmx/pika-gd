﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PIKA.Servicio.GestionDocumental.Data;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    [DbContext(typeof(DBContextGestionDocumental))]
    partial class DBContextGestionDocumentalModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Activo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Ampliado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("ArchivoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("Asunto")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<string>("CodigoElectronico")
                        .HasColumnType("varchar(1024) CHARACTER SET utf8mb4")
                        .HasMaxLength(1024);

                    b.Property<string>("CodigoOptico")
                        .HasColumnType("varchar(1024) CHARACTER SET utf8mb4")
                        .HasMaxLength(1024);

                    b.Property<bool>("Confidencial")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("ElementoClasificacionId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EnPrestamo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("EsElectronio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("FechaApertura")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("FechaCierre")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("OrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Reservado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ArchivoId");

                    b.HasIndex("ElementoClasificacionId");

                    b.ToTable("gd$activo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ActivoDeclinado", b =>
                {
                    b.Property<string>("ActivoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TransferenciaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Motivo")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.HasKey("ActivoId", "TransferenciaId");

                    b.HasIndex("TransferenciaId");

                    b.ToTable("gd$activo_declinado");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ActivoPrestamo", b =>
                {
                    b.Property<string>("PrestamoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ActivoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Devuelto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("FechaDevolucion")
                        .HasColumnType("datetime(6)");

                    b.HasKey("PrestamoId", "ActivoId");

                    b.HasIndex("ActivoId");

                    b.ToTable("gd$activo_prestamo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ActivoTransferencia", b =>
                {
                    b.Property<string>("ActivoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TransferenciaId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("ActivoId", "TransferenciaId");

                    b.HasIndex("TransferenciaId");

                    b.ToTable("gd$activo_transferencia");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.AlmacenArchivo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ArchivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Clave")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ArchivoId");

                    b.ToTable("gd$almacen");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Ampliacion", b =>
                {
                    b.Property<string>("ActivoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Vigente")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<int?>("Anos")
                        .HasColumnType("int");

                    b.Property<int?>("Dias")
                        .HasColumnType("int");

                    b.Property<bool>("FechaFija")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("Fin")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FundamentoLegal")
                        .IsRequired()
                        .HasColumnType("varchar(2000) CHARACTER SET utf8mb4")
                        .HasMaxLength(2000);

                    b.Property<DateTime?>("Inicio")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Meses")
                        .HasColumnType("int");

                    b.Property<string>("TipoAmpliacionId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("ActivoId", "Vigente");

                    b.HasIndex("TipoAmpliacionId");

                    b.ToTable("gd$ampliacion");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Archivo", b =>
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

                    b.Property<string>("TipoArchivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("TipoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("TipoArchivoId");

                    b.ToTable("gd$archivo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Asunto", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ActivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("Contenido")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ActivoId")
                        .IsUnique();

                    b.ToTable("gd$asunto");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ComentarioPrestamo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Comentario")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PrestamoId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("PrestamoId");

                    b.ToTable("gd$comentario_prestamo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ComentarioTrasnferencia", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Comentario")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Publico")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("TransferenciaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("TransferenciaId");

                    b.ToTable("gd$comentario_transferencia");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.CuadroClasificacion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("EstadoCuadroClasificacionId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128)
                        .HasDefaultValue("on");

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

                    b.HasIndex("EstadoCuadroClasificacionId");

                    b.ToTable("gd$cuadroclasificacion");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ElementoClasificacion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Clave")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("CuadroClasifiacionId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ElementoClasificacionId")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int>("Posicion")
                        .HasColumnType("int")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("CuadroClasifiacionId");

                    b.HasIndex("ElementoClasificacionId");

                    b.ToTable("gd$elementoclasificacion");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.EstadoCuadroClasificacion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("gd$estadocuadroclasificacion");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.EstadoTransferencia", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("gd$estado_transferencia");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.EventoTransferencia", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Comentario")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<string>("EstadoTransferenciaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TransferenciaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EstadoTransferenciaId");

                    b.HasIndex("TransferenciaId");

                    b.ToTable("gd$evento_transferencia");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.FaseCicloVital", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("gd$faseciclovital");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.HistorialArchivoActivo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ActivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ArchivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("FechaEgreso")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("FechaIngreso")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("ActivoId");

                    b.HasIndex("ArchivoId");

                    b.ToTable("gd$historialarchivoactivo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Prestamo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ArchivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<bool>("Eliminada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("FechaDevolucion")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("FechaProgramadaDevolucion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Folio")
                        .IsRequired()
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<bool>("TieneDevolucionesParciales")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.HasKey("Id");

                    b.HasIndex("ArchivoId");

                    b.ToTable("gd$prestamo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.TipoAmpliacion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("gd$tipoampliacion");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.TipoArchivo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("FaseCicloVitalId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("FaseCicloVitalId");

                    b.ToTable("gd$tipoarchivo");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Topologia.EspacioEstante", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CodigoElectronico")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<string>("CodigoOptico")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<string>("EstanteId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int>("Posicion")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EstanteId");

                    b.ToTable("gd$espacio_estante");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Topologia.Estante", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("AlmacenArchivoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("CodigoElectronico")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<string>("CodigoOptico")
                        .HasColumnType("longtext CHARACTER SET utf8mb4")
                        .HasMaxLength(2048);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("AlmacenArchivoId");

                    b.ToTable("gd$estantes");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Transferencia", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ArchivoDestinoId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("ArchivoOrigenId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<string>("EstadoTransferenciaId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("ArchivoDestinoId");

                    b.HasIndex("ArchivoOrigenId");

                    b.HasIndex("EstadoTransferenciaId");

                    b.ToTable("gd$transferencia");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Activo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Archivo", "ArchivoActual")
                        .WithMany("Activos")
                        .HasForeignKey("ArchivoId");

                    b.HasOne("PIKA.Modelo.GestorDocumental.ElementoClasificacion", "ElementoClasificacion")
                        .WithMany("Activos")
                        .HasForeignKey("ElementoClasificacionId");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ActivoDeclinado", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Activo", "Activo")
                        .WithMany("DeclinadosTransferenciaRelacionados")
                        .HasForeignKey("ActivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.Transferencia", "Transferencia")
                        .WithMany("ActivosDeclinados")
                        .HasForeignKey("TransferenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ActivoPrestamo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Activo", "Activo")
                        .WithMany("PrestamosRelacionados")
                        .HasForeignKey("ActivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.Prestamo", "Prestamo")
                        .WithMany("ActivosRelacionados")
                        .HasForeignKey("PrestamoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ActivoTransferencia", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Activo", "Activo")
                        .WithMany("TransferenciasRelacionados")
                        .HasForeignKey("ActivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.Transferencia", "Transferencia")
                        .WithMany("ActivosIncluidos")
                        .HasForeignKey("TransferenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.AlmacenArchivo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Archivo", "Archivo")
                        .WithMany("Almacenes")
                        .HasForeignKey("ArchivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Ampliacion", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Activo", "activo")
                        .WithMany("Ampliaciones")
                        .HasForeignKey("ActivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.TipoAmpliacion", "TipoAmpliacion")
                        .WithMany("Ampliaciones")
                        .HasForeignKey("TipoAmpliacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Archivo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.TipoArchivo", "Tipo")
                        .WithMany("Archivos")
                        .HasForeignKey("TipoArchivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Asunto", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Activo", "Activo")
                        .WithOne("oAsunto")
                        .HasForeignKey("PIKA.Modelo.GestorDocumental.Asunto", "ActivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ComentarioPrestamo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Prestamo", "Prestamo")
                        .WithMany("Comentarios")
                        .HasForeignKey("PrestamoId");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ComentarioTrasnferencia", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Transferencia", "Transferencia")
                        .WithMany("Comentarios")
                        .HasForeignKey("TransferenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.CuadroClasificacion", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.EstadoCuadroClasificacion", "Estado")
                        .WithMany("Cuadros")
                        .HasForeignKey("EstadoCuadroClasificacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.ElementoClasificacion", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.CuadroClasificacion", "CuadroClasificacion")
                        .WithMany("Elementos")
                        .HasForeignKey("CuadroClasifiacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.ElementoClasificacion", "Padre")
                        .WithMany("Hijos")
                        .HasForeignKey("ElementoClasificacionId");
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.EventoTransferencia", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.EstadoTransferencia", "Estado")
                        .WithMany("Eventos")
                        .HasForeignKey("EstadoTransferenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.Transferencia", "Transferencia")
                        .WithMany("Eventos")
                        .HasForeignKey("TransferenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.HistorialArchivoActivo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Activo", "Activo")
                        .WithMany("HistorialArchivosActivo")
                        .HasForeignKey("ActivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.Archivo", "Archivo")
                        .WithMany("HistorialArchivosActivo")
                        .HasForeignKey("ArchivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Prestamo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Archivo", "Archivo")
                        .WithMany()
                        .HasForeignKey("ArchivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.TipoArchivo", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.FaseCicloVital", "Fase")
                        .WithMany("TiposArchivo")
                        .HasForeignKey("FaseCicloVitalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Topologia.EspacioEstante", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Topologia.Estante", "Estante")
                        .WithMany("Espacios")
                        .HasForeignKey("EstanteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Topologia.Estante", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.AlmacenArchivo", "Almacen")
                        .WithMany("Estantes")
                        .HasForeignKey("AlmacenArchivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PIKA.Modelo.GestorDocumental.Transferencia", b =>
                {
                    b.HasOne("PIKA.Modelo.GestorDocumental.Archivo", "ArchivoDestino")
                        .WithMany("TransferenciasDestino")
                        .HasForeignKey("ArchivoDestinoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.Archivo", "ArchivoOrigen")
                        .WithMany("TransferenciasOrigen")
                        .HasForeignKey("ArchivoOrigenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PIKA.Modelo.GestorDocumental.EstadoTransferencia", "Estado")
                        .WithMany("Transferencias")
                        .HasForeignKey("EstadoTransferenciaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
