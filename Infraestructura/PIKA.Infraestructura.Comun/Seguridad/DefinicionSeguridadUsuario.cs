using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class DefinicionSeguridadUsuario
    {
        public DefinicionSeguridadUsuario()
        {
            Permisos = new List<PermisoAplicacion>();
            EsAdmin = false;
        }
        public string UsuarioId { get; set; }

        public bool EsAdmin { get; set; }

        public string  DominioId { get; set; }

        public string OUId { get; set; }

        public List<PermisoAplicacion> Permisos { get; set; }
    }
}
