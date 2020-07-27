using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static GestorLocalConfig Copia(this GestorLocalConfig d)
        {
            if (d == null) return null;
            var c = new GestorLocalConfig()
            {
                VolumenId = d.VolumenId,
                Ruta = d.Ruta
            };

            if (d.Volumen != null)
            {
                c.Volumen = d.Volumen.Copia();
            }

            return d;
        }

    }
}
