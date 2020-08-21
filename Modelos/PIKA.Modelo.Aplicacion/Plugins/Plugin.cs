using Newtonsoft.Json;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Aplicacion.Plugins
{


  
    public class Plugin : Entidad<string>, IEntidadNombrada,IEntidadEliminada
    {
        public Plugin()
        {

            this.PluginInstalados = new HashSet<PluginInstalado>();
            this.versionPlugins = new HashSet<VersionPlugin>();

        }

        public string Nombre { get; set; }

        public bool Gratuito { get; set; }
        //public string VersionPluiginid { get; set; }

        /// <summary>
        /// Establece si el Plugin ha sido eliminada
        /// </summary>
        public bool Eliminada { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public ICollection <VersionPlugin> versionPlugins  { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<PluginInstalado> PluginInstalados { get; set; }
        
    }




}
