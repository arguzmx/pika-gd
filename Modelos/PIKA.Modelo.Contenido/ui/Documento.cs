using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido.ui
{
    public class Documento
    {
        public string Id { get; set; }
        public string VersionId { get; set; }
        public string Nombre { get; set; }
        public List<Pagina> Paginas { get; set; }
    }
}
