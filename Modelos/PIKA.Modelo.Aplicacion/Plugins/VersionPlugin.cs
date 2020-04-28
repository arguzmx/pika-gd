using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Aplicacion.Plugins
{
    public class VersionPlugin: Entidad<string>
    {
        public VersionPlugin()
        {
            this.PluginInstalados = new HashSet<PluginInstalado>();
        }
        public string PluginId { get; set; }

        public string Version { get; set; }

        public string URL { get; set; }

        public bool RequiereConfiguracion { get; set; }

        public virtual Plugin Plugins { get; set; }
        public ICollection<PluginInstalado> PluginInstalados { get; set; }

    }
}
