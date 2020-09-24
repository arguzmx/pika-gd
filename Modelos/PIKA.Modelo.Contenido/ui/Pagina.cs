using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido.ui
{
    public class Pagina
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public int Indice { get; set; }
        public string Extension { get; set; }
        public bool EsImagen { get; set; }
        public string Url { get; set; }
        public string UrlThumbnail { get; set; }
        public long TamanoBytes { get; set; }
        public int Alto { get; set; }
        public int Ancho { get; set; }
        public int Rotacion { get; set; }
        /// <summary>
        /// Indica si la parte es del tipo audio
        /// </summary>
        public bool EsAudio { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo video
        /// </summary>
        public bool EsVideo { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo video
        /// </summary>
        public bool EsPDF { get; set; }

        /// <summary>
        /// Indica si la parte tiene una miniatura generada
        /// </summary>
        public bool TieneMiniatura { get; set; }

        /// <summary>
        /// Determina si el contenido ha sido indexado
        /// </summary>
        public bool Indexada { get; set; }

        /// <summary>
        /// Identificador único del volumen para la parte
        /// </summary>
        public string VolumenId { get; set; }
    }
}
