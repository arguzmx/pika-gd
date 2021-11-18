using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class Secreto: ContenidoBase
    {
        public Secreto()
        {

        }

        /// <summary>
        /// Deermina el tipo de contenido del secreto
        /// </summary>
        public TipoContenidoSecreto TipoContenido { get; set; }
    }
}
