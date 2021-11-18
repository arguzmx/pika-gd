using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    /// <summary>
    /// Instancia de la carpeta inteligente
    /// </summary>
    public class InstanciaCarpetaInteligente: Entidad<Guid>, IEntidadRegistroCreacion
    {
        /// <summary>
        /// Identificador único de la instancia
        /// </summary>
        public override Guid Id { get; set; }

        /// <summary>
        /// Identificador de la carpeta inteligente de la cual hereda la instancia
        /// </summary>
        public string CarpetaInteligenteId { get; set; }

        /// <summary>
        /// Identificaodr único del punto de montaje donde será desplegada la instancia
        /// en el explorador de contenido
        /// </summary>
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Identificador único de la carpeta en el punto de montaje que contendrá la instancia
        /// </summary>
        public string CarpetaPuntoMontajeId { get; set; }

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
        /// Documentos vinculados a la carpeta
        /// </summary>
        public List<InstanciaDocumento> Documentos { get; set; }

        /// <summary>
        /// Expedientes vinculados a la carpeta
        /// </summary>
        public List<InstanciaVinculoExpediente> Expedientes { get; set; }

    }
}
