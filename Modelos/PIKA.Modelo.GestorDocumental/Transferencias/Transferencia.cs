using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{

    /// <summary>
    /// Una trasferencia es el proceso para enviar actuivos de un archivo a otro
    /// a trvés de una lista de activos seleccionados para ser incluidos en dicha tarsnferencia
    /// </summary>
    public class Transferencia : Entidad<string>, IEntidadNombrada, IEntidadUsuario
    {

        public Transferencia()
        {
            Eventos = new HashSet<EventoTransferencia>();
            Comentarios = new HashSet<ComentarioTrasnferencia>();
            ActivosDeclinados = new HashSet<ActivoDeclinado>();
            ActivosIncluidos = new HashSet<ActivoTransferencia>();
        }

        /// <summary>
        /// Identificador único interno para la trasnferencias
        /// </summary>
        public override string Id { get; set; }
        // Obligatorio

        /// <summary>
        /// Nombre asociado a la trasnferencia
        /// </summary>
        public string Nombre { get; set; }
        // Obligatorio

        /// <summary>
        /// Fechas de creación de la trasnfenrecia
        /// </summary>
        public DateTime FechaCreacion { get; set; }
        // Debe ser .Now en formato UTC

        /// <summary>
        /// Esatdo actual de la trasnferencia
        /// </summary>
        public string EstadoTransferenciaId { get; set; }
        //Obligatorios

        /// <summary>
        /// Identificador único del árchivo origen
        /// </summary>
        public string ArchivoOrigenId { get; set; }

        /// <summary>
        /// Identificador único del árchivo destino
        /// </summary>
        public string ArchivoDestinoId { get; set; }

        /// <summary>
        /// Identificador único del usuario que la creó
        /// </summary>
        public string UsuarioId { get; set; }

        public ICollection<EventoTransferencia> Eventos { get; set; }

        public ICollection<ActivoTransferencia> ActivosIncluidos { get; set; }

        public ICollection<ActivoDeclinado> ActivosDeclinados { get; set; }

        public ICollection<ComentarioTrasnferencia> Comentarios { get; set; }


        public EstadoTransferencia Estado { get; set; }

        public Archivo ArchivoOrigen { get; set; }

        public Archivo ArchivoDestino { get; set; }
        
    }
}
