﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace PIKA.Infraestructura.Comun.Menus
{
    public class ElementoMenu
    {
        public ElementoMenu()
        {
            Parametros = new List<ParametroMenu>();
            Hijos = new List<ElementoMenu>();
            Indice = 0;
        }


        /// <summary>
        /// POsicion relativa de los elementos
        /// </summary>
        public int Indice { get; set; }

        /// <summary>
        /// Tísulo del menú
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Icono a mostrar 
        /// </summary>
        public string Icono { get; set; }

        /// <summary>
        /// Url relativa para la aplciación
        /// </summary>
        public string URL { get; set; }
        
        /// <summary>
        /// Indentifica si el menú es unn grupo de elementos
        /// </summary>
        public bool EsGrupo { get; set; }

        /// <summary>
        /// Descendianetes del meno
        /// </summary>
        public List<ElementoMenu> Hijos { get; set; }
        
        /// <summary>
        /// Parametros de ruteo 
        /// </summary>
        public List<ParametroMenu> Parametros { get; set; }

        
        /// <summary>
        /// Token de acceso o permisos para el acceso
        /// </summary>
        public string TokenSeguridad { get; set; }

    }
}
