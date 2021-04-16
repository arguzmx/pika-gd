using PIKA.Infraestructura.Comun.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API.Model
{
    public class RespuestaPermisos
    {

        public RespuestaPermisos()
        {
            this.Permisos = new List<PermisoAplicacion>();
        }

        public List<PermisoAplicacion> Permisos { get; set; }
        public bool EsAdmmin { get; set; }
        public string Id { get; set; }

    }
}
