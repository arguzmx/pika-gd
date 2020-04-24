using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Aplicacion.Plugins
{
    public class PluginInstalado
    {

        //La llave primaria de este objeto son las claves PluginID y VersionPLuginId
        public string PLuginId { get; set; }
        public string VersionPLuginId { get; set; }

        public bool Activo { get; set; }
        //Default false

        public DateTime FechaInstalacion { get; set; }
        //Not null


    }
}
