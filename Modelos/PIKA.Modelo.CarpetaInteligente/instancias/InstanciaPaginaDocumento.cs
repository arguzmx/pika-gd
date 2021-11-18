using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    /// <summary>
    /// Define el contenido asociado a un documento
    /// </summary>
    public class InstanciaPaginaDocumento
    {

        /// <summary>
        /// Identificador único de la página, se utiliza además para vincular la página con el contenido almacenado 
        /// en el sistema de archivos
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Extensión del documento
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Clave del visor asociado al elemento
        /// </summary>
        public string Visor { get; set; }

        /// <summary>
        /// Tamaño en bytes de la página
        /// </summary>
        public long Tamano { get; set; }

        /// <summary>
        /// Determina el orden de despliegue de las páginas al interior del documento
        /// </summary>
        public int Indice { get; set; }

    }
}
