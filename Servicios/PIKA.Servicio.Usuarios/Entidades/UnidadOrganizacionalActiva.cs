using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Usuarios
{
    public class UnidadOrganizacionalActiva
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string DominioId { get; set; }
        public bool EsAdmin { get; set; }
    }
}
