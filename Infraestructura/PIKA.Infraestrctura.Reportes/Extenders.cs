using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PIKA.Infraestrctura.Reportes
{
    public static class Extenders
    {
        public static int CuentaChar(this string cadena, char c)
        {
            int conteo = 0;
            if( !string.IsNullOrEmpty(cadena))
            {
                foreach (char x in cadena)
                {
                    if (x == c) conteo++;
                }
            }
            return conteo;
        }


        public static bool EsPar(this int numero)
        {
            return numero % 2 == 0;
        }


        public static bool ConteoParChar(this string cadena, char c)
        {
            int conteo = CuentaChar(cadena, c);
            return EsPar(conteo);
        }

    }
}
