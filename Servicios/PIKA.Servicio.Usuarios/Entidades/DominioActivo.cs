using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Usuarios
{
    public class DominioActivo
    {
        public DominioActivo()
        {
            UnidadesOrganizacionales = new List<UnidadOrganizacionalActiva>();
        }

        public string Id { get; set; }
        public string Nombre { get; set; }

        public bool EsAdmin { get; set; }

        public List<UnidadOrganizacionalActiva> UnidadesOrganizacionales { get; set; }
    }
}
