using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{
    public class TipoMedio: EntidadCatalogo<string, TipoMedio>
    {
        public const string Telefono = "tel";
        public const string Email = "email";
        public const string Facebook = "fbook";
        public const string Twitter = "twitter";


    }
}
