using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public interface IGestorES
    {
        /// <summary>
        ///  Identificador del tipo de gestor documental de acuerdo a TipoGestorES.
        /// </summary>
        public string TipoGestorId { get; }
        
        /// <summary>
        ///  aplica una configuración desde una entidad serializada
        /// </summary>
        /// <param name="entidadSerializada"></param>
        /// <returns></returns>
        bool AplicaConfiguracion(string entidadSerializada);

        /// <summary>
        /// Obtiene la serialización de una configuracion
        /// </summary>
        /// <returns></returns>
        string ObtieneConfiguracion();

        /// <summary>
        /// Determina si la conexión es válida en base a los parámetros de configuración
        /// </summary>
        /// <returns></returns>
        bool ConexionValida();

    }
}
