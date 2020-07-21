using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin
{
    public static partial class Extensiones
    {
        public static PluginInstalado Copia(this PluginInstalado d)
        {
            if (d == null) return null;
            return new PluginInstalado()
            {
                PLuginId = d.PLuginId,
                VersionPLuginId = d.VersionPLuginId,
                Activo = d.Activo,
                FechaInstalacion = d.FechaInstalacion
            };
        }
    }
}
