using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Excepciones
{
     public class ExErrorDatosSesion : Exception
    {

        public ExErrorDatosSesion() : base()
        {

        }

        public ExErrorDatosSesion(string Mensaje) : base(Mensaje)
        {

        }


    }
}
