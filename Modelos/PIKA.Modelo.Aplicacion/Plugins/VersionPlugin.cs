using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Aplicacion.Plugins
{
    public class VersionPlugin: Entidad<string>
    {
        public string PluginId { get; set; }

        public string Version { get; set; }

        public string URL { get; set; }

        public bool RequiereConfiguracion { get; set; }

        public Plugin Plugin { get; set; }

    }
}
