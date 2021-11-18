using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class InstanciaVinculoExpediente : Entidad<Guid>, IEntidadRegistroCreacion
    {
        /// <summary>
        /// Identificador único de la instancia del expediente, cuando se trata de una liga simbolica 
        /// todo el contenido es duplicado desde la instancia del expediente asociada a InstanciaExpedienteId
        /// </summary>
        public override Guid Id { get; set; }

        /// <summary>
        /// Nombre asociado a la carpeta
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Define si el expediente se encuentra completo
        /// </summary>
        public bool Completo { get; set; }


        /// <summary>
        /// Identificador único del expediente modelo en la carpeta inteligente, propiedad Id de ContenidoBase
        /// </summary>
        public Guid VinculoExpedienteId { get; set; }

        /// <summary>
        /// Identificador único de la instancia del expediente cuando se trata de una liga simbólica
        /// </summary>
        public Guid? InstanciaExpedienteId { get; set; }

        /// <summary>
        /// Identificador de la instancia de carpeta inteligente a la que perteneece el expediente,
        /// Un expediente existir fuera de una carpeta 
        /// </summary>
        public Guid? InstanciaCarpetaInteligenteId { get; set; }

        /// <summary>
        /// Identificaodr único del punto de montaje donde será desplegada la instancia
        /// en el explorador de contenido, puede ser nulo si no requiere despliegue
        /// </summary>
        public string PuntoMontajeId { get; set; }

        /// <summary>
        /// Identificador único de la carpeta en el punto de montaje que contendrá la instancia
        /// Puede ser nulo si no requiere despliegue
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
        /// Determina si el documento se encuentra bloqueado para edición
        /// </summary>
        public bool SoloLectura { get; set; }

        /// <summary>
        /// Tamaño en bytes de todas las páginas
        /// </summary>
        public long TamanoTotal { get; set; }


        /// <summary>
        /// Fecha de caducidad del documento
        /// </summary>
        public DateTime? Caducidad { get; set; }


        /// <summary>
        /// Documentos vinculados al expediente, cuando se trata de una liga simbolica 
        /// estas colecciones son copiadas de la instancia apuntada por InstanciaExpedienteId
        /// </summary>
        public List<InstanciaDocumento> Documentos { get; set; }


        /// <summary>
        /// Expedientes vinculados al expediente, cuando se trata de una liga simbolica 
        /// estas colecciones son copiadas de la instancia apuntada por InstanciaExpedienteId
        /// </summary>
        public List<InstanciaVinculoExpediente> Expedientes { get; set; }

    }
}
