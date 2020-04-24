using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Aplicacion.Plugins
{


  
    public class Plugin : Entidad<string>, IEntidadNombrada
    {
        public string Nombre { get; set; }

        public bool Gratuito { get; set; }

    }




}
