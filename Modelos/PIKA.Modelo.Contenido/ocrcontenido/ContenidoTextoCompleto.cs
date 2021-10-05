using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    /// <summary>
    /// Entidad para la búsqueda de texto completo
    /// </summary>
    public class ContenidoTextoCompleto
    {

        /// <summary>
        /// Identificador del elemento al que pertenece la parte
        /// </summary>
        [Keyword(Name = "eid")]
        public string ElementoId { get; set; }

        /// <summary>
        /// Identificador de la versión a la que pertenece la parte
        /// </summary>
        [Keyword(Name = "vrid")]
        public string VersionId { get; set; }

        /// <summary>
        /// Identificador único de la parte
        /// </summary>
        [Keyword(Name = "pid")]
        public string ParteId { get; set; }


        /// <summary>
        /// Identificador del volumen al que pertenece la parte
        /// </summary>
        [Keyword(Name = "vlid")]
        public string VolumenId { get; set; }

        /// <summary>
        /// Punto de montaje al que se asocia el contenido
        /// </summary>
        [Keyword(Name = "pm")]
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Identificador único del folder donde se localiza el contenido
        /// </summary>
        [Keyword(Name = "fid")]
        public string CarpetaId { get; set; }

        /// <summary>
        /// Identificador único del documento asociado a  la version
        /// </summary>
        [Keyword(Name = "did")]
        public string DocumentoId { get; set; }

        /// <summary>
        /// Página de OCR para los documentos multi página, por ejemplo PDF
        /// </summary>
        [Number(NumberType.Integer, Name = "p")]
        public int Pagina { get; set; }

        /// <summary>
        /// Texto a indexar
        /// </summary>
        [Text(Name = "t")]
        public string Texto { get; set; }

        /// <summary>
        ///  Determina si elemento ha sido eliminado
        /// </summary>
        [Boolean(Store = false, Name = "el")]
        public bool Eliminado { get; set; }

        /// <summary>
        /// Especifica si el contenido se encuentr aatcivo para las busquedas
        /// </summary>
        [Boolean(Store = false, Name = "ac")]
        public bool Activo { get; set; }


    }

}
