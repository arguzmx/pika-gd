using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PIKA.Servicio.GestionDocumental.Data.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gd$estadocuadroclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estadocuadroclasificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$estadotransferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estadotransferencia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$temasactivos",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$temasactivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipoampliacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipoampliacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipoarchivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipoarchivo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipodisposiciondocumental",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipodisposiciondocumental", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$tipovaloraciondocumental",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$tipovaloraciondocumental", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "gd$cuadroclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    EstadoCuadroClasificacionId = table.Column<string>(maxLength: 128, nullable: false, defaultValue: "on"),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$cuadroclasificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$cuadroclasificacion_gd$estadocuadroclasificacion_EstadoCu~",
                        column: x => x.EstadoCuadroClasificacionId,
                        principalTable: "gd$estadocuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$archivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    VolumenDefaultId = table.Column<string>(maxLength: 128, nullable: true),
                    PuntoMontajeId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$archivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$archivo_gd$tipoarchivo_TipoArchivoId",
                        column: x => x.TipoArchivoId,
                        principalTable: "gd$tipoarchivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$elementoclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    Posicion = table.Column<int>(maxLength: 10, nullable: false),
                    CuadroClasifiacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ElementoClasificacionId = table.Column<string>(nullable: true),
                    EsRaiz = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$elementoclasificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$elementoclasificacion_gd$cuadroclasificacion_CuadroClasif~",
                        column: x => x.CuadroClasifiacionId,
                        principalTable: "gd$cuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$elementoclasificacion_gd$elementoclasificacion_ElementoCl~",
                        column: x => x.ElementoClasificacionId,
                        principalTable: "gd$elementoclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$almacen",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    Clave = table.Column<string>(maxLength: 200, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$almacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$almacen_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$prestamo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    Folio = table.Column<string>(maxLength: 100, nullable: false),
                    CantidadActivos = table.Column<int>(nullable: false),
                    UsuarioDestinoId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaProgramadaDevolucion = table.Column<DateTime>(nullable: false),
                    FechaDevolucion = table.Column<DateTime>(nullable: true),
                    Descripcion = table.Column<string>(maxLength: 200, nullable: true),
                    Entregado = table.Column<bool>(nullable: false, defaultValue: false),
                    Devuelto = table.Column<bool>(nullable: false, defaultValue: false),
                    TieneDevolucionesParciales = table.Column<bool>(nullable: false, defaultValue: false),
                    UsuarioOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$prestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$prestamo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$transferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    FechaCreacion = table.Column<DateTime>(nullable: false),
                    EstadoTransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoDestinoId = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$transferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$transferencia_gd$archivo_ArchivoDestinoId",
                        column: x => x.ArchivoDestinoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$transferencia_gd$archivo_ArchivoOrigenId",
                        column: x => x.ArchivoOrigenId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$transferencia_gd$estadotransferencia_EstadoTransferenciaId",
                        column: x => x.EstadoTransferenciaId,
                        principalTable: "gd$estadotransferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$entradaclasificacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Clave = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    VigenciaTramite = table.Column<int>(nullable: false),
                    VigenciaConcentracion = table.Column<int>(nullable: false),
                    Descripcion = table.Column<string>(nullable: true),
                    TipoDisposicionDocumentalId = table.Column<string>(nullable: true),
                    ElementoClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    Posicion = table.Column<int>(nullable: false),
                    CuadroClasifiacionId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$entradaclasificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$entradaclasificacion_gd$elementoclasificacion_ElementoCla~",
                        column: x => x.ElementoClasificacionId,
                        principalTable: "gd$elementoclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$entradaclasificacion_gd$tipodisposiciondocumental_TipoDis~",
                        column: x => x.TipoDisposicionDocumentalId,
                        principalTable: "gd$tipodisposiciondocumental",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$estantes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    AlmacenArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    CodigoOptico = table.Column<string>(maxLength: 2048, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$estantes_gd$almacen_AlmacenArchivoId",
                        column: x => x.AlmacenArchivoId,
                        principalTable: "gd$almacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$comentarioprestamo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Comentario = table.Column<string>(maxLength: 2048, nullable: false),
                    PrestamoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$comentarioprestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$comentarioprestamo_gd$prestamo_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "gd$prestamo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$comentariotransferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Comentario = table.Column<string>(maxLength: 2048, nullable: false),
                    Publico = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$comentariotransferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$comentariotransferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$eventotransferencia",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    EstadoTransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Comentario = table.Column<string>(maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$eventotransferencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$eventotransferencia_gd$estadotransferencia_EstadoTransfer~",
                        column: x => x.EstadoTransferenciaId,
                        principalTable: "gd$estadotransferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$eventotransferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    CuadroClasificacionId = table.Column<string>(maxLength: 250, nullable: false),
                    EntradaClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    IDunico = table.Column<string>(maxLength: 250, nullable: true),
                    FechaApertura = table.Column<DateTime>(nullable: false),
                    FechaCierre = table.Column<DateTime>(nullable: true),
                    Asunto = table.Column<string>(maxLength: 2048, nullable: true),
                    CodigoOptico = table.Column<string>(maxLength: 512, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 512, nullable: true),
                    EsElectronico = table.Column<bool>(nullable: false),
                    Reservado = table.Column<bool>(nullable: false),
                    Confidencial = table.Column<bool>(nullable: false),
                    Eliminada = table.Column<bool>(nullable: false, defaultValue: false),
                    ArchivoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    EnPrestamo = table.Column<bool>(nullable: false),
                    Ampliado = table.Column<bool>(nullable: false),
                    FechaRetencionAT = table.Column<DateTime>(nullable: true),
                    FechaRetencionAC = table.Column<DateTime>(nullable: true),
                    TipoOrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    OrigenId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    TieneContenido = table.Column<bool>(nullable: false, defaultValue: false),
                    ElementoId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$archivo_ArchivoOrigenId",
                        column: x => x.ArchivoOrigenId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$cuadroclasificacion_CuadroClasificacionId",
                        column: x => x.CuadroClasificacionId,
                        principalTable: "gd$cuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$entradaclasificacion_EntradaClasificacionId",
                        column: x => x.EntradaClasificacionId,
                        principalTable: "gd$entradaclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$activo_gd$tipoarchivo_TipoArchivoId",
                        column: x => x.TipoArchivoId,
                        principalTable: "gd$tipoarchivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$estadisticaclasificacionacervo",
                columns: table => new
                {
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    CuadroClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    EntradaClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    ConteoActivos = table.Column<int>(nullable: false),
                    ConteoActivosEliminados = table.Column<int>(nullable: false),
                    FechaMinApertura = table.Column<DateTime>(nullable: true),
                    FechaMaxCierre = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$estadisticaclasificacionacervo", x => new { x.ArchivoId, x.CuadroClasificacionId, x.EntradaClasificacionId });
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$cuadroclasificacion_Cua~",
                        column: x => x.CuadroClasificacionId,
                        principalTable: "gd$cuadroclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$estadisticaclasificacionacervo_gd$entradaclasificacion_En~",
                        column: x => x.EntradaClasificacionId,
                        principalTable: "gd$entradaclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$valoracionentradaclasificacion",
                columns: table => new
                {
                    EntradaClasificacionId = table.Column<string>(maxLength: 128, nullable: false),
                    TipoValoracionDocumentalId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$valoracionentradaclasificacion", x => new { x.EntradaClasificacionId, x.TipoValoracionDocumentalId });
                    table.ForeignKey(
                        name: "FK_gd$valoracionentradaclasificacion_gd$entradaclasificacion_En~",
                        column: x => x.EntradaClasificacionId,
                        principalTable: "gd$entradaclasificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$valoracionentradaclasificacion_gd$tipovaloraciondocumenta~",
                        column: x => x.TipoValoracionDocumentalId,
                        principalTable: "gd$tipovaloraciondocumental",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$espacioestante",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 200, nullable: false),
                    CodigoOptico = table.Column<string>(maxLength: 2048, nullable: true),
                    CodigoElectronico = table.Column<string>(maxLength: 2048, nullable: true),
                    EstanteId = table.Column<string>(maxLength: 128, nullable: false),
                    Posicion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$espacioestante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$espacioestante_gd$estantes_EstanteId",
                        column: x => x.EstanteId,
                        principalTable: "gd$estantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activodeclinado",
                columns: table => new
                {
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false),
                    Motivo = table.Column<string>(maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activodeclinado", x => new { x.ActivoId, x.TransferenciaId });
                    table.ForeignKey(
                        name: "FK_gd$activodeclinado_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activodeclinado_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activoprestamo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    PrestamoId = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    Devuelto = table.Column<bool>(nullable: false, defaultValue: false),
                    FechaDevolucion = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activoprestamo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$activoprestamo_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activoprestamo_gd$prestamo_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "gd$prestamo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activoseleccionado",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    TemaId = table.Column<string>(maxLength: 128, nullable: false),
                    Id = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activoseleccionado", x => new { x.Id, x.UsuarioId, x.TemaId });
                    table.ForeignKey(
                        name: "FK_gd$activoseleccionado_gd$activo_Id",
                        column: x => x.Id,
                        principalTable: "gd$activo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_gd$activoseleccionado_gd$temasactivos_TemaId",
                        column: x => x.TemaId,
                        principalTable: "gd$temasactivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$activotransferencia",
                columns: table => new
                {
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    TransferenciaId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$activotransferencia", x => new { x.ActivoId, x.TransferenciaId });
                    table.ForeignKey(
                        name: "FK_gd$activotransferencia_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$activotransferencia_gd$transferencia_TransferenciaId",
                        column: x => x.TransferenciaId,
                        principalTable: "gd$transferencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$ampliacion",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    TipoAmpliacionId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaFija = table.Column<bool>(nullable: false, defaultValue: false),
                    Vigente = table.Column<bool>(nullable: false, defaultValue: false),
                    Inicio = table.Column<DateTime>(nullable: true),
                    Fin = table.Column<DateTime>(nullable: true),
                    FundamentoLegal = table.Column<string>(maxLength: 2000, nullable: false),
                    Anos = table.Column<int>(nullable: true),
                    Meses = table.Column<int>(nullable: true),
                    Dias = table.Column<int>(nullable: true),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$ampliacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$ampliacion_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gd$ampliacion_gd$tipoampliacion_TipoAmpliacionId",
                        column: x => x.TipoAmpliacionId,
                        principalTable: "gd$tipoampliacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gd$asunto",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(nullable: false),
                    Contenido = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$asunto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$asunto_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gd$historialarchivoactivo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 128, nullable: false),
                    ActivoId = table.Column<string>(maxLength: 128, nullable: false),
                    ArchivoId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaIngreso = table.Column<DateTime>(nullable: false),
                    FechaEgreso = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gd$historialarchivoactivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gd$historialarchivoactivo_gd$activo_ActivoId",
                        column: x => x.ActivoId,
                        principalTable: "gd$activo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gd$historialarchivoactivo_gd$archivo_ArchivoId",
                        column: x => x.ArchivoId,
                        principalTable: "gd$archivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_Ampliado",
                table: "gd$activo",
                column: "Ampliado");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_ArchivoId",
                table: "gd$activo",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_ArchivoOrigenId",
                table: "gd$activo",
                column: "ArchivoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_CodigoElectronico",
                table: "gd$activo",
                column: "CodigoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_CodigoOptico",
                table: "gd$activo",
                column: "CodigoOptico");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_CuadroClasificacionId",
                table: "gd$activo",
                column: "CuadroClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_Eliminada",
                table: "gd$activo",
                column: "Eliminada");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_EnPrestamo",
                table: "gd$activo",
                column: "EnPrestamo");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_EntradaClasificacionId",
                table: "gd$activo",
                column: "EntradaClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaApertura",
                table: "gd$activo",
                column: "FechaApertura");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaCierre",
                table: "gd$activo",
                column: "FechaCierre");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaRetencionAC",
                table: "gd$activo",
                column: "FechaRetencionAC");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_FechaRetencionAT",
                table: "gd$activo",
                column: "FechaRetencionAT");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_Nombre",
                table: "gd$activo",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_TieneContenido",
                table: "gd$activo",
                column: "TieneContenido");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activo_TipoArchivoId",
                table: "gd$activo",
                column: "TipoArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activodeclinado_TransferenciaId",
                table: "gd$activodeclinado",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activoprestamo_ActivoId",
                table: "gd$activoprestamo",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activoprestamo_PrestamoId",
                table: "gd$activoprestamo",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activoseleccionado_TemaId",
                table: "gd$activoseleccionado",
                column: "TemaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$activotransferencia_TransferenciaId",
                table: "gd$activotransferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$almacen_ArchivoId",
                table: "gd$almacen",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$ampliacion_ActivoId",
                table: "gd$ampliacion",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$ampliacion_TipoAmpliacionId",
                table: "gd$ampliacion",
                column: "TipoAmpliacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$archivo_TipoArchivoId",
                table: "gd$archivo",
                column: "TipoArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$asunto_ActivoId",
                table: "gd$asunto",
                column: "ActivoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gd$comentarioprestamo_PrestamoId",
                table: "gd$comentarioprestamo",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$comentariotransferencia_TransferenciaId",
                table: "gd$comentariotransferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$cuadroclasificacion_EstadoCuadroClasificacionId",
                table: "gd$cuadroclasificacion",
                column: "EstadoCuadroClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$elementoclasificacion_CuadroClasifiacionId",
                table: "gd$elementoclasificacion",
                column: "CuadroClasifiacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$elementoclasificacion_ElementoClasificacionId",
                table: "gd$elementoclasificacion",
                column: "ElementoClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$entradaclasificacion_ElementoClasificacionId",
                table: "gd$entradaclasificacion",
                column: "ElementoClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$entradaclasificacion_TipoDisposicionDocumentalId",
                table: "gd$entradaclasificacion",
                column: "TipoDisposicionDocumentalId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$espacioestante_EstanteId",
                table: "gd$espacioestante",
                column: "EstanteId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$estadisticaclasificacionacervo_CuadroClasificacionId",
                table: "gd$estadisticaclasificacionacervo",
                column: "CuadroClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$estadisticaclasificacionacervo_EntradaClasificacionId",
                table: "gd$estadisticaclasificacionacervo",
                column: "EntradaClasificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$estantes_AlmacenArchivoId",
                table: "gd$estantes",
                column: "AlmacenArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$eventotransferencia_EstadoTransferenciaId",
                table: "gd$eventotransferencia",
                column: "EstadoTransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$eventotransferencia_TransferenciaId",
                table: "gd$eventotransferencia",
                column: "TransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$historialarchivoactivo_ActivoId",
                table: "gd$historialarchivoactivo",
                column: "ActivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$historialarchivoactivo_ArchivoId",
                table: "gd$historialarchivoactivo",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_ArchivoId",
                table: "gd$prestamo",
                column: "ArchivoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_Devuelto",
                table: "gd$prestamo",
                column: "Devuelto");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_FechaProgramadaDevolucion",
                table: "gd$prestamo",
                column: "FechaProgramadaDevolucion");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_Folio",
                table: "gd$prestamo",
                column: "Folio");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_UsuarioDestinoId",
                table: "gd$prestamo",
                column: "UsuarioDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$prestamo_UsuarioOrigenId",
                table: "gd$prestamo",
                column: "UsuarioOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$temasactivos_UsuarioId",
                table: "gd$temasactivos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_ArchivoDestinoId",
                table: "gd$transferencia",
                column: "ArchivoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_ArchivoOrigenId",
                table: "gd$transferencia",
                column: "ArchivoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$transferencia_EstadoTransferenciaId",
                table: "gd$transferencia",
                column: "EstadoTransferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_gd$valoracionentradaclasificacion_TipoValoracionDocumentalId",
                table: "gd$valoracionentradaclasificacion",
                column: "TipoValoracionDocumentalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gd$activodeclinado");

            migrationBuilder.DropTable(
                name: "gd$activoprestamo");

            migrationBuilder.DropTable(
                name: "gd$activoseleccionado");

            migrationBuilder.DropTable(
                name: "gd$activotransferencia");

            migrationBuilder.DropTable(
                name: "gd$ampliacion");

            migrationBuilder.DropTable(
                name: "gd$asunto");

            migrationBuilder.DropTable(
                name: "gd$comentarioprestamo");

            migrationBuilder.DropTable(
                name: "gd$comentariotransferencia");

            migrationBuilder.DropTable(
                name: "gd$espacioestante");

            migrationBuilder.DropTable(
                name: "gd$estadisticaclasificacionacervo");

            migrationBuilder.DropTable(
                name: "gd$eventotransferencia");

            migrationBuilder.DropTable(
                name: "gd$historialarchivoactivo");

            migrationBuilder.DropTable(
                name: "gd$valoracionentradaclasificacion");

            migrationBuilder.DropTable(
                name: "gd$temasactivos");

            migrationBuilder.DropTable(
                name: "gd$tipoampliacion");

            migrationBuilder.DropTable(
                name: "gd$prestamo");

            migrationBuilder.DropTable(
                name: "gd$estantes");

            migrationBuilder.DropTable(
                name: "gd$transferencia");

            migrationBuilder.DropTable(
                name: "gd$activo");

            migrationBuilder.DropTable(
                name: "gd$tipovaloraciondocumental");

            migrationBuilder.DropTable(
                name: "gd$almacen");

            migrationBuilder.DropTable(
                name: "gd$estadotransferencia");

            migrationBuilder.DropTable(
                name: "gd$entradaclasificacion");

            migrationBuilder.DropTable(
                name: "gd$archivo");

            migrationBuilder.DropTable(
                name: "gd$elementoclasificacion");

            migrationBuilder.DropTable(
                name: "gd$tipodisposiciondocumental");

            migrationBuilder.DropTable(
                name: "gd$tipoarchivo");

            migrationBuilder.DropTable(
                name: "gd$cuadroclasificacion");

            migrationBuilder.DropTable(
                name: "gd$estadocuadroclasificacion");
        }
    }
}
