using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Instancias
{
    /// <summary>
    /// Vinculo entre un objeto y una lista de documentos de plantilla
    /// </summary>
    public class VinculoListaPlantilla
    {
        /// <summary>
        /// Identificador único d ela plantilla
        /// </summary>
        [Keyword]
        public string PlantillaId { get; set; }

        /// <summary>
        /// Identificador único de la lista
        /// </summary>
        [Keyword]
        public string ListaId { get; set; }

        /// <summary>
        /// Nombre asociado con la lista de documentos
        /// </summary>
        [Keyword]
        public string Nombre { get; set; }
    }
}
