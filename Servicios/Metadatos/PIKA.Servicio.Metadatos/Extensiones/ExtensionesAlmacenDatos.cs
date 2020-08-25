using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static AlmacenDatos Copia(this AlmacenDatos d)
        {
            if (d == null) return null;
            return new AlmacenDatos()
            {
                Id = d.Id,
                Nombre=d.Nombre,
                Contrasena=d.Contrasena,
                Direccion=d.Direccion,
                Protocolo=d.Protocolo,
                Puerto=d.Puerto,
                TipoAlmacenMetadatosId=d.TipoAlmacenMetadatosId,
            };
        }
    }
}


