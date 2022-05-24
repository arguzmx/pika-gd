using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static AlmacenArchivo Copia(this AlmacenArchivo a)
        {
            if (a == null) return null;
            return new AlmacenArchivo()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Clave = a.Clave,
                ArchivoId = a.ArchivoId,
                HabilitarFoliado = a.HabilitarFoliado,
                FolioActualContenedor = a.FolioActualContenedor,
                MacroFolioContenedor = a.MacroFolioContenedor,
                Ubicacion = a.Ubicacion

            };
        }

        public static ZonaAlmacen Copia(this ZonaAlmacen a)
        {
            if (a == null) return null;
            return new ZonaAlmacen()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                Clave = a.Clave,
                ArchivoId = a.ArchivoId,
                AlmacenArchivoId = a.AlmacenArchivoId,
            };
        }
    }

}
