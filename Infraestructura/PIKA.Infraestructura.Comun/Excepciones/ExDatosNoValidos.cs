using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Excepciones
{
     public class ExDatosNoValidos : Exception
    {

        public ExDatosNoValidos() : base()
        {

        }

        public ExDatosNoValidos(string Mensaje) : base(Mensaje)
        {

        }


    }
}
