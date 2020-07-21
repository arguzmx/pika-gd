using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Archivo Copia(this Archivo a)
        {
            if (a == null) return null;
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
    }
}
