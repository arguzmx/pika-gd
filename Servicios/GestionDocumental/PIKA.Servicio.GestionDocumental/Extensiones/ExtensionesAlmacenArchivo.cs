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
                ArchivoId = a.ArchivoId,
                AlmacenArchivoId = a.AlmacenArchivoId,
            };
        }


        public static PosicionAlmacen Copia(this PosicionAlmacen a)
        {
            if (a == null) return null;
            return new PosicionAlmacen()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                ArchivoId = a.ArchivoId,
                AlmacenArchivoId = a.AlmacenArchivoId,
                CodigoBarras = a.CodigoBarras,
                CodigoElectronico = a.CodigoElectronico,
                Indice = a.Indice,
                PosicionPadreId = a.PosicionPadreId,
                ZonaAlmacenId = a.ZonaAlmacenId,
                Ocupacion = a.Ocupacion,

            };
        }

        public static ContenedorAlmacen Copia(this ContenedorAlmacen a)
        {
            if (a == null) return null;
            return new ContenedorAlmacen()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                ArchivoId = a.ArchivoId,
                AlmacenArchivoId = a.AlmacenArchivoId,
                CodigoBarras = a.CodigoBarras,
                CodigoElectronico = a.CodigoElectronico,
                ZonaAlmacenId = a.ZonaAlmacenId,
                Ocupacion = a.Ocupacion,
                PosicionAlmacenId = a.PosicionAlmacenId,
            };
        }

    }

}
