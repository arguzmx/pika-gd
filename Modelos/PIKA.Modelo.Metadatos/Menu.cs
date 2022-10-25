using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{

    public class Menu
    {
        

        /// <summary>
        /// POsición del elemento en el menú
        /// </summary>
        public virtual int MenuIndex
        {
            get; set;
        }

        /// <summary>
        /// Identificador único del menú al que pertenece la opcion
        /// </summary>
        public virtual string MenuId
        {
            get; set;
        }

        public virtual string Condicion
        {
            get; set;
        }

        public virtual string Icono
        {
            get; set;
        }

        public virtual string Titulo
        {
            get; set;
        }

    }
}
