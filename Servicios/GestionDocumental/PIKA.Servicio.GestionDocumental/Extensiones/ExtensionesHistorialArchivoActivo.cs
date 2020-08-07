using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static HistorialArchivoActivo Copia(this HistorialArchivoActivo a)
        {
            if (a == null) return null;
            return new HistorialArchivoActivo()
            {
                Id = a.Id,
                ArchivoId = a.ArchivoId,
                FechaEgreso = a.FechaEgreso,
                FechaIngreso = a.FechaIngreso,
                ActivoId = a.ActivoId
                 
            };
        }
    }
}

