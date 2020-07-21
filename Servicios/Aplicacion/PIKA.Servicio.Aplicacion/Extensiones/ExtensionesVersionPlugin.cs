using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin
{
    public static partial class Extensiones
    {
        public static VersionPlugin Copia(this VersionPlugin d)
        {
            if (d == null) return null;
            return new VersionPlugin()
            {
                Id = d.Id,
                PluginId = d.PluginId,
                RequiereConfiguracion = d.RequiereConfiguracion,
                URL = d.URL
            };
        }
    }
}
