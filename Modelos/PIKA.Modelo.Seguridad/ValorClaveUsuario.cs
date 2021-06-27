using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Seguridad
{
    public class ValorClaveUsuario
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Clave { get; set; }
        public string Valor { get; set; }

        public ApplicationUser Usuario { get; set; }
    }
}
