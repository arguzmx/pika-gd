using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Excepciones
{
    public class ExElementoExistente: Exception
    {
   
        public ExElementoExistente() : base()
        {

        }

        public ExElementoExistente(string Mensaje) : base(Mensaje)
        {

        }


    }
}
