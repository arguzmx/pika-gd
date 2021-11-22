using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{

    /// <summary>
    /// Define un documento 
    /// </summary>
    public class Documento : ContenidoBase, ICaducible
    {

        public Documento()
        {
            ExtensionesValidas = new List<string>();
        }

        /// <summary>
        /// Especifica el tipo de caducidad
        /// </summary>
        public TipoPeriodicidad TipoCaducidad { get; set; }

        /// <summary>
        /// Unidad de medida para el intevalo de caducidad
        /// </summary>
        public UnidadesTiempo UnidadesTiempo { get; set; }

        /// <summary>
        /// Número de unidades del intervalo para calcular la caducidad
        /// </summary>
        public int IntervaloCaducidad { get; set; }

        /// <summary>
        /// Especifica que en cada aactualización de caducidad, el intervalo comienza de nuevo
        /// </summary>
        public bool CaducidadCiclica { get; set; }

        /// <summary>
        /// Mínimo de cardinalidad para completez, debe ser mayor o igual a cero.
        /// El mínimo de cardinalidad determina la condición de completez de la entidad
        /// 0 = Indica que elemento es opcional
        /// </summary>
        public int MinimoCardinalidad { get; set; }

        /// <summary>
        /// Máximo de cardinalidad para completez, debe ser mayor al MinimoCardinalidad.
        /// Establece un límite para la cantidad de documentos de este tipo en la carpeta
        /// 0 = Ilimitado
        /// </summary>
        public int MaximoCardinalidad { get; set; }

        /// <summary>
        /// Lista de extensione válidas asociadas a los archivos que forman las páginas del documento
        /// </summary>
        public List<string> ExtensionesValidas { get; set; }

        /// <summary>
        /// Establece el tamaño máximo en bytes que debe tener una página
        /// </summary>
        public long TamanoMaximoPagina { get; set; }

        /// <summary>
        /// Establece el tamaño máximo en bytes que puede tener el documento sumando todas las páginas
        /// </summary>
        public long TamanoMaximoTotal { get; set; }

        /// <summary>
        /// Identificador de la plantilla de metadatos aplicable al documento
        /// </summary>
        public string PlantillaMetadatosId { get; set; }

    }
}
