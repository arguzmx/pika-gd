using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido.Servicios.Busqueda
{
    public class CacheBusqueda
    {
        public string Id { get; set; }
        public List<string> Unicos { get; set; }
        public List<string> UnicosElastic { get; set; }
        public string  sort_col { get; set; }
        public string sort_dir { get; set; }

    }
}
