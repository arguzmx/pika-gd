using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Excepciones
{
     public class ExLoginInfoInvalida : Exception
    {

        public ExLoginInfoInvalida() : base()
        {

        }

        public ExLoginInfoInvalida(string Mensaje) : base(Mensaje)
        {

        }


    }
}
