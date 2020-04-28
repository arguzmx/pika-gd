using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin.Servicios
{
    public static class ExtensionesAplicacion
    {
        public static Plugin CopiaPlugin(this Plugin d)
        {
            return new Plugin()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Gratuito=d.Gratuito,
                Eliminada=d.Eliminada
            };
        }
        public static VersionPlugin CopiaVersionPlugin(this VersionPlugin d)
        {
            return new VersionPlugin()
            {
                Id = d.Id,
                PluginId=d.PluginId,
                RequiereConfiguracion=d.RequiereConfiguracion,
                URL=d.URL
            };
        }
        public static PluginInstalado CopiaPluginInstalado(this PluginInstalado d)
        {
            return new PluginInstalado()
            {
                PLuginId=d.PLuginId,
                VersionPLuginId=d.VersionPLuginId,
                Activo=d.Activo,
                FechaInstalacion=d.FechaInstalacion
            };
        }
      
    }
}
