using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

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
            Comentarios = new HashSet<ComentarioTransferencia>();
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
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<EventoTransferencia> Eventos { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoTransferencia> ActivosIncluidos { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<ActivoDeclinado> ActivosDeclinados { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual  ICollection<ComentarioTransferencia> Comentarios { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public virtual EstadoTransferencia Estado { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Archivo ArchivoOrigen { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public virtual Archivo ArchivoDestino { get; set; }
        
    }
}
