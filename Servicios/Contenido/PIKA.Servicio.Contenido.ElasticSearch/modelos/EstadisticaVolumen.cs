using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido.ElasticSearch.modelos
{
    public class EstadisticaVolumen
    {
        public EstadisticaVolumen()
        {
            ConteoElementos = 0;
            ConteoPartes = 0;
            TamanoBytes = 0;
        }

        public string Id { get; set; }
        public long ConteoElementos { get; set; }
        public long ConteoPartes { get; set; }
        public long TamanoBytes { get; set; }
    }
}
