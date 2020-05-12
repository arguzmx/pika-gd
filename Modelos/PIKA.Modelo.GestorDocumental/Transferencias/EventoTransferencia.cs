using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class EventoTransferencia: Entidad<string>
    {
        /// <summary>
        /// Identificado único de la trásnferencia
        /// </summary>
        public string TransferenciaId { get; set; }

        /// <summary>
        /// Fecha de registro del estado UTC
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// IDentificador único del estado para el evento
        /// </summary>
        public string EstadoTransferenciaId { get; set; }

        /// <summary>
        /// Comentarios relacionados con el estado
        /// </summary>
        public string Comentario { get; set; }
        // opcional, 2048 bytes


        public EstadoTransferencia Estado { get; set; }
        public Transferencia Transferencia { get; set; }
    }
}
