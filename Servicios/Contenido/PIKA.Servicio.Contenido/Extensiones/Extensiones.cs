using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static string[] LimpiaIds(this string[] ids)
        {
            for(int i = 0; i < ids.Length; i++)
            {
                ids[i] = ids[i].TrimStart().TrimEnd();
            }

            return ids;
        }
    }
}
