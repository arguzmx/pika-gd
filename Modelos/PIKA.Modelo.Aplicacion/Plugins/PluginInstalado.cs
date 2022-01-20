using Newtonsoft.Json;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PIKA.Modelo.Aplicacion.Plugins
{
    public class PluginInstalado
    {

        //La llave primaria de este objeto son las claves PluginID y VersionPLuginId
        public string PLuginId { get; set; }
        public string VersionPLuginId { get; set; }

        public bool Activo { get; set; }
        //Default false

        public DateTime? FechaInstalacion { get; set; }

        //Not null
        [XmlIgnore]
        [JsonIgnore]
        public virtual VersionPlugin VersionPlugin { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Plugin Plugin { get; set; }
    }
}
