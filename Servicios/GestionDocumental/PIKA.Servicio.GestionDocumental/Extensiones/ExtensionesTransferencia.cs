using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Transferencia Copia(this Transferencia t)
        {
            if (t == null) return null;
            return new Transferencia()
            {
                Id = t.Id,
                Nombre = t.Nombre,
                FechaCreacion = t.FechaCreacion,
                ArchivoOrigenId = t.ArchivoOrigenId,
                ArchivoDestinoId = t.ArchivoDestinoId,
                EstadoTransferenciaId = t.EstadoTransferenciaId,
                UsuarioId = t.UsuarioId,
                CuadroClasificacionId = t.CuadroClasificacionId,
                EntradaClasificacionId = t.EntradaClasificacionId,
                Folio = t.Folio,
                CantidadActivos = t.CantidadActivos,
                RangoDias = t.RangoDias,
                FechaCorte = t.FechaCorte
            };
        }
    }
}
