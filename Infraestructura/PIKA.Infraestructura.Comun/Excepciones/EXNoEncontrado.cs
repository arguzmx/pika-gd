using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Excepciones
{
     public class EXNoEncontrado : Exception
    {

        public EXNoEncontrado() : base()
        {

        }

        public EXNoEncontrado(string Mensaje) : base(Mensaje)
        {

        }


    }
}
