using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class InstanciaSecreto : Entidad<Guid>, IEntidadRegistroCreacion
    {
        /// <summary>
        /// Identificador único de la instancia del secreto
        /// </summary>
        public override Guid Id { get; set; }

        /// <summary>
        /// Identificador único del secreto modelo en la carpeta inteligente, propiedad Id de ContenidoBase
        /// </summary>
        public Guid SecretoId { get; set; }

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
        /// Los secretos mantienen un historial y esta bandera especifica la versión activa del secreto
        /// </summary>
        public bool VersionActiva { get; set; }


        /// <summary>
        /// En el caso de secretos con contenido de archivo guarda el nombre original del archivo ingresado
        /// para utilziarlo en la descarga
        /// </summary>
        public string NombreOriginal { get; set; }
    }
}
