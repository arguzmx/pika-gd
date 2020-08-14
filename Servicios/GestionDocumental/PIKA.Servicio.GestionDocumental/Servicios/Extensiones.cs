using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Topologia;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public static class ExtensionesGestionDocumental
    {
 
 
 

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

        #region Almacens


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
