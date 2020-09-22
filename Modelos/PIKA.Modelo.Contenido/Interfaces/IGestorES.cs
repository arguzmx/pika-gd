using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Contenido
{
    public interface IGestorES
    {
        /// <summary>
        /// Determina si la conexión es válida en base a los parámetros de configuración
        /// </summary>
        /// <returns></returns>
        bool ConexionValida();

        /// <summary>
        /// Escribe un arreglo de bytes en el medio de almacenamiento
        /// </summary>
        /// <param name="contenido">Contenido en forma de un arreglo de bytes</param>
        /// <param name="Id">Identificador único del contenido</param>
        /// <param name="informacion">Información del archivo original</param>
        /// <returns>El número total de bytes escritos</returns>
        Task<long> EscribeBytes(string Id, byte[] contenido, FileInfo informacion, bool sobreescribir); 



    }
}
