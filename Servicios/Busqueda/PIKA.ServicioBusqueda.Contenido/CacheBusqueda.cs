using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{
    class CacheBusqueda
    {
        public string Id { get; set; }
        public List<string> Unicos { get; set; }
        public List<string> UnicosElastic { get; set; }

    }
}
