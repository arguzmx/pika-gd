using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental.Topologia;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static EspacioEstante Copia(this EspacioEstante ee)
        {
            if (ee == null) return null;
            return new EspacioEstante()
            {
                Id = ee.Id,
                Nombre = ee.Nombre,
                EstanteId = ee.EstanteId,
                CodigoElectronico = ee.CodigoElectronico,
                CodigoOptico = ee.CodigoOptico,
                Posicion = ee.Posicion
            };
        }
    }
}
