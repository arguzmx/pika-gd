using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public static class ExtensionesGestionDocumental
    {

        public static UnidadAdministrativaGuiaSimpleArchivo AUnidadGuiaSimple(this UnidadAdministrativaArchivo u , int expedientes)
        {
            return new UnidadAdministrativaGuiaSimpleArchivo()
            {
                ArchivoConcentracionId = u.ArchivoConcentracionId,
                ArchivoHistoricoId = u.ArchivoHistoricoId,
                ArchivoTramiteId = u.ArchivoTramiteId,
                AreaProcedenciaArchivo = u.AreaProcedenciaArchivo,
                Cargo = u.Cargo,
                Domicilio = u.Domicilio,
                Email = u.Email,
                Expedientes = expedientes,
                Id = u.Id,
                Responsable = u.Responsable,
                Telefono = u.Telefono,
                UbicacionFisica = u.UbicacionFisica,
                UnidadAdministrativa = u.UnidadAdministrativa,
                Secciones = new List<SeccionGuiaSimpleArchivo>()
            };
        }

        public static EstadisticaClasificacionAcervo AEstadistica(this Activo a)
        {
            return new EstadisticaClasificacionAcervo()
            {
                ArchivoId = a.ArchivoId,
                ConteoActivos = 0,
                ConteoActivosEliminados = 0,
                CuadroClasificacionId = a.CuadroClasificacionId,
                EntradaClasificacionId = a.EntradaClasificacionId,
                FechaMaxCierre = a.FechaCierre,
                FechaMinApertura = a.FechaApertura,
                UnidadAdministrativaArchivoId = a.UnidadAdministrativaArchivoId
            };
        }

        public static PermisosUnidadAdministrativaArchivo Copia(this PermisosUnidadAdministrativaArchivo ap)
        {
            return new PermisosUnidadAdministrativaArchivo()
            {
                ActualizarAcervo = ap.ActualizarAcervo,
                CrearAcervo = ap.CrearAcervo,
                DestinatarioId = ap.DestinatarioId,
                ElminarAcervo = ap.ElminarAcervo,
                Id = ap.Id,
                LeerAcervo = ap.LeerAcervo,
                UnidadAdministrativaArchivoId = ap.UnidadAdministrativaArchivoId
            };
        }

        public static UnidadAdministrativaArchivo Copia(this UnidadAdministrativaArchivo ap)
        {
            return new UnidadAdministrativaArchivo()
            {
                AreaProcedenciaArchivo = ap.AreaProcedenciaArchivo,
                Cargo = ap.Cargo,
                Domicilio = ap.Domicilio,
                Email = ap.Email,
                Id = ap.Id,
                Responsable = ap.Responsable,
                Telefono = ap.Telefono,
                UbicacionFisica = ap.UbicacionFisica,
                UnidadAdministrativa = ap.UnidadAdministrativa,
                ArchivoTramiteId = ap.ArchivoTramiteId,
                ArchivoConcentracionId = ap.ArchivoConcentracionId,
                ArchivoHistoricoId = ap.ArchivoHistoricoId,
                OrigenId = ap.OrigenId,
                TipoOrigenId = ap.TipoOrigenId
            };
        }
        public static ActivoPrestado CopiaActivoPrestamo(this Activo ap, int indice)
        {
            return new ActivoPrestado()
            {
                Id = ap.Id,
                Nombre = ap.Nombre,
                Asunto = ap.Asunto,
                Reservado = ap.Reservado,
                Confidencial = ap.Confidencial,
                Ampliado = ap.Ampliado,
                ArchivoActual = ap.ArchivoActual,
                ArchivoId = ap.ArchivoId,
                ArchivoOrigenId = ap.ArchivoOrigenId,
                CodigoElectronico = ap.CodigoElectronico,
                CodigoOptico = ap.CodigoOptico,
                CuadroClasificacionId = ap.CuadroClasificacionId,
                ElementoId = ap.ElementoId,
                Eliminada = ap.Eliminada,
                EnPrestamo = ap.EnPrestamo,
                EntradaClasificacionId = ap.EntradaClasificacionId,
                EsElectronico = ap.EsElectronico,
                FechaApertura = ap.FechaApertura,
                FechaCierre = ap.FechaCierre,
                FechaRetencionAC = ap.FechaRetencionAC,
                FechaRetencionAT = ap.FechaRetencionAT,
                TieneContenido = ap.TieneContenido,
                IDunico = ap.IDunico,
                Devuelto = ap.EnPrestamo ? "No" : "Si",
                Indice = indice.ToString()
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
