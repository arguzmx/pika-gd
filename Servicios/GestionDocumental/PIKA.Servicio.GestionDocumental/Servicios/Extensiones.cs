﻿using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Topologia;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public static class ExtensionesGestionDocumental
    {
        #region Cuadro clasificacion

        public static CuadroClasificacion CopiacuadroElemto(this CuadroClasificacion d)
        {
            if (d == null) return null;
            var c = new CuadroClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId,
                EstadoCuadroClasificacionId=d.EstadoCuadroClasificacionId,
                Eliminada=d.Eliminada
            };

            if (d.Estado != null)
            {
                c.Estado = new EstadoCuadroClasificacion() { Id = d.Estado.Id, Nombre = d.Estado.Nombre, Cuadros = null };
            }

            return c;
        }


        public static ElementoClasificacion CopiaElemento(this ElementoClasificacion d)
        {
            return new ElementoClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Eliminada = d.Eliminada
            };
        }

        public static EstadoCuadroClasificacion CopiaEstadoCuadro(this EstadoCuadroClasificacion d)
        {
            return new EstadoCuadroClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
            };
        }
        #endregion

        #region Archivos

        public static Archivo CopiaArchivo(this Archivo a)
        {
            return new Archivo()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                OrigenId = a.OrigenId,
                TipoOrigenId = a.TipoOrigenId,
                TipoArchivoId = a.TipoArchivoId,
                Eliminada = a.Eliminada
            };
        }

        public static TipoArchivo CopiaTipoArchivo(this TipoArchivo t)
        {
            return new TipoArchivo()
            {
                Id = t.Id,
                Nombre = t.Nombre,
            };
        }

        public static FaseCicloVital CopiaFase(this FaseCicloVital f)
        {
            return new FaseCicloVital()
            {
                Id = f.Id,
                Nombre = f.Nombre,
            };
        }
        #endregion

        #region Actvos

        public static Activo CopiaActivo(this Activo a)
        {
            return new Activo()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                OrigenId = a.OrigenId,
                TipoOrigenId = a.TipoOrigenId,
                Asunto = a.Asunto,
                FechaApertura= a.FechaApertura,
                FechaCierre = a.FechaCierre,
                EsElectronio = a.EsElectronio,
                CodigoElectronico = a.CodigoElectronico,
                CodigoOptico = a.CodigoOptico,
                Reservado = a.Reservado,
                EnPrestamo = a.EnPrestamo,
                Confidencial = a.Confidencial,
                Ampliado = a.Ampliado,
                ElementoClasificacionId = a.ElementoClasificacionId,
                ArchivoId = a.ArchivoId,
                Eliminada = a.Eliminada,
            };
        }

        public static Asunto CopiaAsunto(this Asunto a)
        {
            return new Asunto()
            {
                Id = a.Id,
                Contenido = a.Contenido
            };
        }
        #endregion

        #region Ampliacion

        public static Ampliacion CopiaAmpliacion(this Ampliacion a)
        {
            return new Ampliacion()
            {
                ActivoId = a.ActivoId,
                Vigente = a.Vigente,
                TipoAmpliacionId = a.TipoAmpliacionId,
                FechaFija = a.FechaFija,
                FundamentoLegal = a.FundamentoLegal,
                Inicio = a.Inicio,
                Fin = a.Fin,
                Anos = a.Anos,
                Meses = a.Meses,
                Dias = a.Dias

            };
        }


        public static TipoAmpliacion CopiaTipoAmpliacion(this TipoAmpliacion a)
        {
            return new TipoAmpliacion()
            {
                Id = a.Id,
                Nombre = a.Nombre
            };
        }



        #endregion

        #region Prestamos

        public static Prestamo CopiaPrestamo(this Prestamo p)
        {
            return new Prestamo()
            {
                Id = p.Id,
                Folio = p.Folio,
                Eliminada = p.Eliminada,
                FechaCreacion = p.FechaCreacion,
                FechaDevolucion = p.FechaDevolucion,
                Comentarios = p.Comentarios,
                FechaProgramadaDevolucion = p.FechaProgramadaDevolucion,
                TieneDevolucionesParciales = p.TieneDevolucionesParciales
            };
        }

        public static ActivoPrestamo CopiaActivoPrestamo(this ActivoPrestamo ap)
        {
            return new ActivoPrestamo()
            {
                PrestamoId = ap.PrestamoId,
                ActivoId = ap.ActivoId,
                Devuelto = ap.Devuelto,
                FechaDevolucion = ap.FechaDevolucion
            };
        }

        public static ComentarioPrestamo CopiaComentarioPrestamo(this ComentarioPrestamo cc)
        {
            return new ComentarioPrestamo()
            {
                Id = cc.Id,
                Comentario = cc.Comentario,
                Fecha = cc.Fecha,
                PrestamoId = cc.PrestamoId
            };
        }
        #endregion

        #region Almacens

        public static AlmacenArchivo CopiaAlamacen(this AlmacenArchivo a)
        {
            return new AlmacenArchivo()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Clave = a.Clave,
                ArchivoId = a.ArchivoId
            };
        }

        public static Estante CopiaEstante(this Estante e)
        {
            return new Estante()
            {
                Id = e.Id,
                Nombre = e.Nombre,
                CodigoElectronico = e.CodigoElectronico,
                CodigoOptico = e.CodigoOptico,
                AlmacenArchivoId = e.AlmacenArchivoId
            };
        }

        public static EspacioEstante CopiaEspaciosEstante(this EspacioEstante ee)
        {
            return new EspacioEstante()
            {
                Id = ee.Id,
                Nombre = ee.Nombre,
                EstanteId = ee.EstanteId,
                CodigoElectronico = ee.CodigoElectronico,
                CodigoOptico = ee. CodigoOptico,
                Posicion = ee.Posicion
            };
        }
        #endregion

        #region TRansferencias
        public static Transferencia CopiaTransferencia(this Transferencia t)
        {
            return new Transferencia()
            {
                Id = t.Id,
                Nombre = t.Nombre,
                FechaCreacion = t.FechaCreacion,
                ArchivoOrigen = t.ArchivoOrigen,
                ArchivoDestino = t.ArchivoDestino,
                EstadoTransferenciaId = t.EstadoTransferenciaId,
                UsuarioId = t.UsuarioId
            };
        }

        public static EstadoTransferencia CopiaEstadoTransferencia(this EstadoTransferencia e)
        {
            return new EstadoTransferencia()
            {
                Id = e.Id,
                Nombre = e.Nombre,
            };
        }

        public static EventoTransferencia CopiaEventoTransferencia(this EventoTransferencia e)
        {
            return new EventoTransferencia()
            {
                Id = e.Id,
                TransferenciaId = e.TransferenciaId,
                Fecha = e.Fecha,
                EstadoTransferenciaId = e.EstadoTransferenciaId,
                Comentario = e.Comentario
            };
        }

        public static ComentarioTransferencia CopiaComentarioTransferencia(this ComentarioTransferencia c)
        {
            return new ComentarioTransferencia()
            {
                Id = c.Id,
                TransferenciaId = c.TransferenciaId,
                UsuarioId = c.UsuarioId,
                Fecha = c.Fecha,
                Comentario = c.Comentario,
                Publico = c.Publico,
            };
        }

        public static ActivoTransferencia CopiaActivoTransferencia(this ActivoTransferencia c)
        {
            return new ActivoTransferencia()
            {
                ActivoId = c.ActivoId,
                TransferenciaId = c.TransferenciaId,
            };
        }

        public static ActivoDeclinado CopiaActivoDeclinado(this ActivoDeclinado c)
        {
            return new ActivoDeclinado()
            {
                ActivoId = c.ActivoId,
                TransferenciaId = c.TransferenciaId,
                Motivo = c.Motivo,
            };
        }




        #endregion

    }
}
