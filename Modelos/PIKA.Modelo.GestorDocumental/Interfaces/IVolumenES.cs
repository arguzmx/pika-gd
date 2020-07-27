using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public interface IVolumenES
    {
        /// <summary>
        /// Crea una entarda en el volumen en base a un arreglo de bytes
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Consecutivo"></param>
        /// <param name="Contenido"></param>
        /// <returns></returns>
        bool Crear(string Id, int Consecutivo, byte[] Contenido );

        /// <summary>
        /// Crea una entarda en el volumen en base a una ruta del contenido
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Consecutivo"></param>
        /// <param name="RutaContenid"></param>
        /// <returns></returns>
        bool Crear(string Id, int Consecutivo, string RutaContenido);


        /// <summary>
        /// Elimina una entarda edl volumen
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Consecutivo"></param>
        /// <returns></returns>
        bool Eliminar(string Id, int Consecutivo);

        /// <summary>
        /// Número de elementos resguistardos en el volumen
        /// </summary>
        /// <returns></returns>
        long Canidadelementos();

        /// <summary>
        /// Obtiene el tamaño del conteido almacenado en bytes
        /// </summary>
        /// <returns></returns>
        long TamanoVolumen();

    }
}
