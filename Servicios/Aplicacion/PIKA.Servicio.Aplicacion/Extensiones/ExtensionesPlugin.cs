using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin
{
   public static partial class Extensiones
    {
        public static Plugin Copia(this Plugin d)
        {
            if (d == null) return null;

            return new Plugin()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Gratuito = d.Gratuito,
                Eliminada = d.Eliminada
            };
        }
    }
}
