using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public interface IServicioCache
    {
        /// <summary>
        /// Obtiene el valore de la entrada del cache especificada por la Clave
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Clave"></param>
        /// <returns></returns>
        T Leer<T>(string Clave);


        /// <summary>
        /// Añade o actualiza una nueva entrada al caché, si la etrada ya existe genera un error
        /// </summary>
        /// <param name="Clave"></param>
        /// <param name="Contenido"></param>
        /// <param name="Duracion"></param>
        void Escribir<T>(string Clave , T Contenido, TimeSpan Duracion );


        /// <summary>
        /// Elimina una entrada del cache en base a su clave
        /// </summary>
        /// <param name="Clave"></param>
        void Eliminar(string Clave);

        /// <summary>
        /// Eliminar todos los elementos de del cache
        /// </summary>
        void Limpiar();

    }
}
