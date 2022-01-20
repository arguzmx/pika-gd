using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public class UsuarioAPI
    {

        public UsuarioAPI()
        {
            Accesos = new List<Acceso>();
            Roles = new List<string>(); 
        }

        public string Id { get; set; }

        public bool AdminGlobal { get; set; }
        public List<string> Roles { get; set; }

        public List<Acceso> Accesos { get; set; }

        public int gmtOffset { get; set; }

    }


    public class Acceso {
        public string Dominio { get; set; }
        public string OU { get; set; }

        public bool Admin { get; set; }
    }

}
