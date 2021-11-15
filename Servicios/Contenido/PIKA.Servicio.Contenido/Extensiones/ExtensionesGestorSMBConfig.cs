using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static GestorSMBConfig Copia(this GestorSMBConfig d)
        {
            if (d == null) return null;
            var c = new GestorSMBConfig()
            {
                VolumenId = d.VolumenId,
                Ruta = d.Ruta,
                Contrasena = d.Contrasena,
                Dominio = d.Dominio,
                Usuario = d.Usuario
            };

            if (d.Volumen != null)
            {
                c.Volumen = d.Volumen.Copia();
            }

            return d;
        }

        public static GestorLaserficheConfig Copia(this GestorLaserficheConfig d)
        {
            if (d == null) return null;
            var c = new GestorSMBConfig()
            {
                VolumenId = d.VolumenId,
                Ruta = d.Ruta,
            };

            if (d.Volumen != null)
            {
                c.Volumen = d.Volumen.Copia();
            }
            return d;
        }

    }
}
