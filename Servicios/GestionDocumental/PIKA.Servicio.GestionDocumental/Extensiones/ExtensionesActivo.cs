using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;
namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Activo Copia(this Activo a)
        {
            if (a == null) return null;
            return new Activo()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                OrigenId = a.OrigenId,
                TipoOrigenId = a.TipoOrigenId,
                Asunto = a.Asunto,
                FechaApertura = a.FechaApertura,
                FechaCierre = a.FechaCierre,
                EsElectronico = a.EsElectronico,
                CodigoElectronico = a.CodigoElectronico,
                CodigoOptico = a.CodigoOptico,
                Reservado = a.Reservado,
                EnPrestamo = a.EnPrestamo,
                Confidencial = a.Confidencial,
                Ampliado = a.Ampliado,
                EntradaClasificacionId = a.EntradaClasificacionId,
                ArchivoId = a.ArchivoId,
                Eliminada = a.Eliminada,
                ArchivoOrigenId = a.ArchivoOrigenId,
                FechaRetencionAC = a.FechaRetencionAC,
                FechaRetencionAT = a.FechaRetencionAT,
                IDunico = a.IDunico,
                CuadroClasificacionId = a.CuadroClasificacionId,
                TipoArchivoId = a.TipoArchivoId,
                TieneContenido = a.TieneContenido,
                ElementoId = a.ElementoId,
                UnidadAdministrativaArchivoId = a.UnidadAdministrativaArchivoId ,
            };
        }
    }
}
