using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Excepciones
{
     public class ExErrorRelacional : Exception
    {

        public ExErrorRelacional() : base()
        {

        }

        public ExErrorRelacional(string Mensaje) : base(Mensaje)
        {

        }


    }
}
