using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class InstanciaDocumento : Entidad<Guid>, IEntidadRegistroCreacion
    {
        /// <summary>
        /// Identificador único de la instancia del documento
        /// </summary>
        public override Guid Id { get; set; }

        /// <summary>
        /// Nombre asociado a la carpeta
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Identificador único del documento modelo en la carpeta inteligente, propiedad Id de ContenidoBase
        /// </summary>
        public Guid DocumentoId { get; set; }

        /// <summary>
        /// Identificador de la instancia de carpeta inteligente a la que perteneece el documento
        /// </summary>
        public string InstanciaCarpetaInteligenteId { get; set; }

        /// <summary>
        /// Identificador del creador de la carpeta
        /// </summary>
        public string CreadorId { get; set; }
        // Del sistema vía el valor en el JWT

        /// <summary>
        /// Fecha de creación de la carpeta
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        // Del sistema en UTC

        /// <summary>
        /// Contiene el Id de almacenamiento de los metadatos asociados a la instancia
        /// </summary>
        public string MetadatosId { get; set; }

        /// <summary>
        /// Determina si el documento se encuentra bloqueado para edición
        /// </summary>
        public bool SoloLectura { get; set; }

        /// <summary>
        /// Tamaño en bytes de todas las páginas
        /// </summary>
        public long TamanoTotal { get; set; }

        /// <summary>
        /// Permite conocer si el documento tiene paginas
        /// </summary>
        public bool TienePaginas { get; set; }

        /// <summary>
        /// Fecha de caducidad del documento
        /// </summary>
        public DateTime? Caducidad { get; set; }

        /// <summary>
        /// Páginas de contenido electrónico asociadas al documento
        /// </summary>
        public List<InstanciaPaginaDocumento> Paginas { get; set; }

    }
}
