using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Instancias
{
    /// <summary>
    /// Permite asociar uno mas documentos y plantollas a un objeto
    /// </summary>
    public class VinculoDocumentoPlantilla
    {
        /// <summary>
        /// Identificador único de la plantilla
        /// </summary>
        [Keyword]
        public string PlantillaId { get; set; }

        /// <summary>
        /// Identificador único del documento almacenado en e repositorio de documentos
        /// </summary>
        [Keyword]
        public string DocumentoId { get; set; }


        /// <summary>
        /// Nombre para la relación uno a uno
        /// </summary>
        [Keyword]
        public string Nombre { get; set; }
    }
}
