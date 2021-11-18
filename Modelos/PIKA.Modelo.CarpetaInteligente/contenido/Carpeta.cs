using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class Carpeta: ContenidoBase
    {
        public Carpeta()
        {
            Contenido = new List<ContenidoBase>();
        }

        /// <summary>
        /// Contenido del expediente
        /// </summary>
        public List<ContenidoBase> Contenido { get; set; }
    }
}
