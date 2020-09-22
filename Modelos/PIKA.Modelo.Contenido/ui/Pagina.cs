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
        public int TamanoBytes { get; set; }
        public int Alto { get; set; }
        public int Ancho { get; set; }
        public int Rotacion { get; set; }
    }
}
