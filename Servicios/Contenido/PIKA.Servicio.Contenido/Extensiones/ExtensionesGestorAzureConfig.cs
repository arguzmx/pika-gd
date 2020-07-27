using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static GestorAzureConfig Copia(this GestorAzureConfig d)
        {
            if (d == null) return null;
            var c = new GestorAzureConfig()
            {
                VolumenId = d.VolumenId,
                Contrasena = d.Contrasena,
                Usuario = d.Usuario,  
                Endpoint = d.Endpoint
            };

            if (d.Volumen != null)
            {
                c.Volumen = d.Volumen.Copia();
            }

            return d;
        }

    }
}
