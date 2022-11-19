using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class ContextoRegistroActividad
    {

        public string DominioId { get; set; }
        public string UnidadOrgId { get; set; }
        public string UsuarioId { get; set; }
        public string DireccionInternet { get; set; }
        public string IdConexion { get; set; }
        public DateTime FechaUTC { get; set; }

    }
}
