using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental.Topologia;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static Estante Copia(this Estante e)
        {
            if (e == null) return null;
            return new Estante()
            {
                Id = e.Id,
                Nombre = e.Nombre,
                CodigoElectronico = e.CodigoElectronico,
                CodigoOptico = e.CodigoOptico,
                AlmacenArchivoId = e.AlmacenArchivoId
            };
        }
    }
}
